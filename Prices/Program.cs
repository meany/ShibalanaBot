using CoinGecko.Clients;
using CoinGecko.Entities.Response.Coins;
using dm.Shibalana.Data;
using dm.Shibalana.Data.Models;
using dm.Shibalana.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NLog;
using RestSharp;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace dm.Shibalana.Prices
{
    class Program
    {
        private static IServiceProvider services;
        private static IConfigurationRoot configuration;
        private static AppDbContext db;
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private static LanadexTradesDatum marketDataSerum;
        private static CoinFullDataById marketDataCG;
        private static CoinGecko.Entities.Response.Simple.Price priceData;
        private static BigInteger shibaSupply;
        private static decimal shibaCirc;
        private static int shibaHolders;
        private static int shibaDecimals;
        private static decimal serumVolume;

        private static Price prices24hAgo;

        private static readonly string tokenAddress = "Dhg9XnzJWzSQqH2aAnhPTEJHGQAkALDfD98MA499A7pa";
        private static readonly string serumSHIBA_USDC_1dot0 = "3M8uZhLZMxFUedsEgPzywZr9qbGTv3kKNMCEfAmg8iyK";
        private static string serumCurrent = serumSHIBA_USDC_1dot0;

        public static void Main(string[] args)
            => new Program().MainAsync(args).GetAwaiter().GetResult();

        public async Task MainAsync(string[] args)
        {
            try
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("Config.Prices.json", optional: true, reloadOnChange: true)
                    .AddJsonFile("Config.Prices.Local.json", optional: true, reloadOnChange: true);

                configuration = builder.Build();

                services = new ServiceCollection()
                    .AddDatabase<AppDbContext>(configuration.GetConnectionString("Database"))
                    .BuildServiceProvider();
                db = services.GetService<AppDbContext>();

                if (db.Database.GetPendingMigrations().Any())
                {
                    log.Info("Migrating database");
                    db.Database.Migrate();
                }

                await Start();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private async Task Start()
        {
            try
            {
                log.Info("Getting info");
                //var stat = db.Stats
                //    .AsNoTracking()
                //    .OrderByDescending(x => x.Date)
                //    .FirstOrDefault();

                await GetInfo();
                var group = Guid.NewGuid();

                // SERUM

                // prices
                decimal serum_priceOneShiba = marketDataSerum.Price;
                decimal serum_priceOneUsd = 1 / serum_priceOneShiba;

                // market cap
                decimal supply = shibaSupply.ToToken(shibaDecimals);
                decimal serum_fullMktCapUsd = serum_priceOneShiba * supply;
                decimal serum_circMktCapUsd = serum_priceOneShiba * shibaCirc;
                int serum_volumeUsd = (int)Math.Round(serumVolume);

                decimal serum_mktCapUsdChgAmt = serum_fullMktCapUsd - prices24hAgo.FullMarketCapUSD;
                Change serum_mktCapUsdChg = (serum_mktCapUsdChgAmt > 0) ?
                    Change.Up : (serum_mktCapUsdChgAmt < 0) ? Change.Down : Change.None;
                decimal serum_mktCapUsdChgPct = (Math.Abs(serum_mktCapUsdChgAmt) / prices24hAgo.FullMarketCapUSD) * 100;

                // changes
                decimal serum_changeUsd = serum_priceOneShiba - prices24hAgo.PriceUSDCForOneSHIBA;
                Change serum_priceUsdChg = (serum_changeUsd > 0) ?
                    Change.Up : (serum_changeUsd < 0) ? Change.Down : Change.None;
                decimal serum_changeUsdPct = (Math.Abs(serum_changeUsd) / prices24hAgo.PriceUSDCForOneSHIBA) * 100;

                var serum_item = new Price
                {
                    Date = DateTime.UtcNow,
                    Group = group,
                    PriceSHIBAForOneUSDC = serum_priceOneUsd,
                    PriceUSDCForOneSHIBA = serum_priceOneShiba,
                    Source = PriceSource.LanaDex,

                    FullMarketCapUSD = int.Parse(Math.Round(serum_fullMktCapUsd).ToString()),
                    CircMarketCapUSD = int.Parse(Math.Round(serum_circMktCapUsd).ToString()),

                    PriceUSD = serum_priceOneShiba,

                    MarketCapUSDChange = serum_mktCapUsdChg,
                    MarketCapUSDChangePct = Math.Round(serum_mktCapUsdChgPct, 8) > 999 ? 999 : Math.Round(serum_mktCapUsdChgPct, 8),
                    PriceUSDChange = serum_priceUsdChg,
                    PriceUSDChangePct = serum_changeUsdPct,

                    VolumeUSD = serum_volumeUsd
                };

                db.Add(serum_item);

                log.Info($"Saving {serum_item.Source} prices to database {group}");
                db.SaveChanges();

                // COING

                // prices
                decimal coinG_priceUsd = decimal.Parse(marketDataCG.MarketData.CurrentPrice["usd"].Value.ToString(), NumberStyles.Any);
                decimal coinG_priceOneUsd = 1 / coinG_priceUsd;

                // market cap
                decimal coinG_mktCapUsd = decimal.Parse(marketDataCG.MarketData.MarketCap["usd"].Value.ToString());
                decimal coinG_mktCapUsdChgAmt = (marketDataCG.MarketData.MarketCapChange24HInCurrency.Count == 0) ?
                    0 : decimal.Parse(marketDataCG.MarketData.MarketCapChange24HInCurrency["usd"].ToString(), NumberStyles.Any);
                Change coinG_mktCapUsdChg = (coinG_mktCapUsdChgAmt > 0) ?
                    Change.Up : (coinG_mktCapUsdChgAmt < 0) ? Change.Down : Change.None;
                decimal coinG_mktCapUsdChgPct = (marketDataCG.MarketData.MarketCapChangePercentage24HInCurrency.Count == 0) ?
                    0 : Math.Abs(decimal.Parse(marketDataCG.MarketData.MarketCapChangePercentage24HInCurrency["usd"].ToString(), NumberStyles.Any));
                
                // volume
                int coinG_volumeUsd = (int)Math.Round(marketDataCG.MarketData.TotalVolume["usd"].Value);

                // changes
                string coinG_changeUsd = "0";
                string coinG_changeUsdPct = "0";
                if (marketDataCG.MarketData.PriceChange24HInCurrency.Count > 0 &&
                    marketDataCG.MarketData.PriceChangePercentage24HInCurrency.Count > 0)
                {
                    coinG_changeUsd = marketDataCG.MarketData.PriceChange24HInCurrency["usd"].ToString();
                    coinG_changeUsdPct = marketDataCG.MarketData.PriceChangePercentage24HInCurrency["usd"].ToString();
                }
                decimal coinG_priceUsdChgAmt = decimal.Parse(coinG_changeUsd, NumberStyles.Any);
                Change coinG_priceUsdChg = (coinG_priceUsdChgAmt > 0) ?
                    Change.Up : (coinG_priceUsdChgAmt < 0) ? Change.Down : Change.None;
                decimal coinG_priceUsdChgPct = Math.Abs(decimal.Parse(coinG_changeUsdPct, NumberStyles.Any));

                var coinG_item = new Price
                {
                    Date = DateTime.UtcNow,
                    Group = group,
                    PriceSHIBAForOneUSDC = coinG_priceOneUsd,
                    PriceUSDCForOneSHIBA = coinG_priceUsd,
                    Source = PriceSource.CoinGecko,

                    FullMarketCapUSD = int.Parse(Math.Round(serum_fullMktCapUsd).ToString()),
                    CircMarketCapUSD = int.Parse(Math.Round(serum_circMktCapUsd).ToString()),

                    PriceUSD = coinG_priceUsd,

                    MarketCapUSDChange = coinG_mktCapUsdChg,
                    MarketCapUSDChangePct = Math.Round(coinG_mktCapUsdChgPct, 8),
                    PriceUSDChange = coinG_priceUsdChg,
                    PriceUSDChangePct = coinG_priceUsdChgPct,

                    VolumeUSD = coinG_volumeUsd
                };

                db.Add(coinG_item);

                log.Info($"Saving {coinG_item.Source} prices to database {group}");
                db.SaveChanges();

                // STATS

                var stat = new Stat
                {
                    Date = DateTime.UtcNow,
                    Group = group,
                    Supply = supply,
                    Circulation = shibaCirc,
                    Holders = shibaHolders
                };

                db.Add(stat);

                log.Info($"Saving stats to database {group}");
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private async Task GetInfo()
        {
            GetSerumMarketData();
            GetSerumVolumeData();
            GetCGMarketData();
            GetShibaCirc();
            //GetSimplePrices();
            GetPrices24hAgo();

            while (prices24hAgo == null || 
                marketDataSerum == null || 
                marketDataCG == null || 
                serumVolume == 0 || 
                shibaCirc == 0)
                await Task.Delay(200);
        }

        private async void GetPrices24hAgo()
        {
            try
            {
                prices24hAgo = await db.Prices
                    .AsNoTracking()
                    .OrderByDescending(x => x.Date)
                    .FirstOrDefaultAsync(x => x.Date <= DateTime.UtcNow.AddHours(-24));
                if (prices24hAgo == null)
                    prices24hAgo = new Price();
                log.Info($"GetPrices24hAgo: OK");
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        //private async void GetSimplePrices()
        //{
        //    try
        //    {
        //        var client = CoinGeckoClient.Instance;
        //        var ids = new string[] { "bitcoin", "ethereum", "solana", "usdc" };
        //        var curs = new string[] { "usd" };
        //        priceData = await client.SimpleClient.GetSimplePrice(ids, curs);
        //        log.Info($"GetSimplePrices: OK");
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex);
        //    }
        //}

        private async void GetShibaCirc()
        {
            try
            {
                var client = new RestClient("https://api.solscan.io");
                var req1 = new RestRequest($"account?address={tokenAddress}", DataFormat.Json);
                var res1 = await client.GetAsync<Solscan>(req1);
                var req3 = new RestRequest($"token/meta?token={tokenAddress}", DataFormat.Json);
                var res3 = await client.GetAsync<Solscan>(req3);

                shibaDecimals = res1.Data.TokenInfo.Decimals;
                shibaSupply = BigInteger.Parse(res1.Data.TokenInfo.Supply);

                decimal teamAmts = await db.AddressInfos
                    .Where(x => x.IsPartOfTeam)
                    .SumAsync(x => x.Amount);
                shibaCirc = shibaSupply.ToToken(shibaDecimals) - teamAmts;
                shibaHolders = res3.Data.Holder;

                log.Info($"GetShibaCirc: OK");
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private async void GetSerumMarketData()
        {
            try
            {
                var client = new RestClient("https://www.lanadex.com");
                var req = new RestRequest($"/api/trades/address/{serumCurrent}", DataFormat.Json);
                var res = await client.GetAsync<LanadexTrades>(req);
                marketDataSerum = res.Data.OrderByDescending(x => x.Time).First();

                log.Info($"GetSerumMarketData: OK");
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private async void GetSerumVolumeData()
        {
            try
            {
                var client = new RestClient("https://api.dexlab.space");
                var req = new RestRequest($"v1/volumes/{serumCurrent}", DataFormat.Json);
                var res = await client.GetAsync<DexLabVolume>(req);
                serumVolume = res.Data.Summary.TotalVolume;

                log.Info($"GetSerumVolumeData: OK");
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private async void GetCGMarketData()
        {
            try
            {
                var client = CoinGeckoClient.Instance;
                marketDataCG = await client.CoinsClient
                    .GetAllCoinDataWithId("shibalana", "false", true, true, false, false, false);

                log.Info($"GetCGMarketData: OK");
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
    }
}