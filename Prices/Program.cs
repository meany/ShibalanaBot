using CoinGecko.Clients;
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

        private static LanadexTradesDatum marketData;
        private static CoinGecko.Entities.Response.Simple.Price priceData;
        private static BigInteger shibaSupply;
        private static BigInteger shibaMain;
        private static BigInteger shibaCirc;
        private static int shibaHolders;
        private static int shibaDecimals;

        private static Price prices24hAgo;

        private static readonly string tokenAddress = "Dhg9XnzJWzSQqH2aAnhPTEJHGQAkALDfD98MA499A7pa";
        private static readonly string mainAddress = "2vQBVYD6fn1Z4iA2JWo3qY1tMBanspkHY4TfLe51hj9b";
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

                if (db.Database.GetPendingMigrations().Count() > 0)
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

                // shiba prices
                decimal priceOneShiba = marketData.Price;
                decimal priceOneUsd = 1 / priceOneShiba;

                // market cap
                decimal supply = shibaSupply.ToToken(shibaDecimals);
                decimal circ = shibaCirc.ToToken(shibaDecimals);
                decimal fullMktCapUsd = priceOneShiba * supply;
                decimal circMktCapUsd = priceOneShiba * circ;

                decimal mktCapUsdChgAmt = fullMktCapUsd - prices24hAgo.FullMarketCapUSD;
                Change mktCapUsdChg = (mktCapUsdChgAmt > 0) ? Change.Up : (mktCapUsdChgAmt < 0) ? Change.Down : Change.None;
                decimal mktCapUsdChgPct = (Math.Abs(mktCapUsdChgAmt) / (prices24hAgo.FullMarketCapUSD + 1));

                //// volume
                //int volumeUsd = (int)Math.Round(data.MarketData.TotalVolume["usd"].Value);

                // price changes
                decimal changeUsd = priceOneShiba - prices24hAgo.PriceUSDCForOneSHIBA;
                Change priceUsdChg = (changeUsd > 0) ? Change.Up : (changeUsd < 0) ? Change.Down : Change.None;
                decimal changeUsdPct = (Math.Abs(changeUsd) / (prices24hAgo.PriceUSDCForOneSHIBA + 1));

                var group = Guid.NewGuid();
                var item = new Price
                {
                    Date = DateTime.UtcNow,
                    Group = group,
                    PriceSHIBAForOneUSDC = priceOneUsd,
                    PriceUSDCForOneSHIBA = priceOneShiba,
                    Source = PriceSource.Lanadex,

                    FullMarketCapUSD = int.Parse(Math.Round(fullMktCapUsd).ToString()),
                    CircMarketCapUSD = int.Parse(Math.Round(circMktCapUsd).ToString()),

                    PriceUSD = priceOneShiba,

                    MarketCapUSDChange = mktCapUsdChg,
                    MarketCapUSDChangePct = Math.Round(mktCapUsdChgPct, 8) > 999 ? 999 : Math.Round(mktCapUsdChgPct, 8),
                    PriceUSDChange = priceUsdChg,
                    PriceUSDChangePct = changeUsdPct,

                    //VolumeUSD = volumeUsd
                };

                db.Add(item);

                log.Info($"Saving prices to database {group}");
                db.SaveChanges();

                var item2 = new Stat
                {
                    Date = DateTime.UtcNow,
                    Group = group,
                    Supply = supply,
                    Circulation = circ,
                    Holders = shibaHolders
                };

                db.Add(item2);

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
            GetShibaMarketData();
            GetShibaStats();
            //GetSimplePrices();
            GetPrices24hAgo();

            while (prices24hAgo == null || marketData == null || shibaCirc == 0)
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

        private async void GetShibaStats()
        {
            try
            {
                var client = new RestClient("https://api.solscan.io");
                var req1 = new RestRequest($"account?address={tokenAddress}", DataFormat.Json);
                var res1 = await client.GetAsync<Solscan>(req1);
                var req2 = new RestRequest($"account?address={mainAddress}", DataFormat.Json);
                var res2 = await client.GetAsync<Solscan>(req2);
                var req3 = new RestRequest($"token/meta?token={tokenAddress}", DataFormat.Json);
                var res3 = await client.GetAsync<Solscan>(req3);

                shibaDecimals = res1.Data.TokenInfo.Decimals;

                shibaSupply = BigInteger.Parse(res1.Data.TokenInfo.Supply);
                shibaMain = BigInteger.Parse(res2.Data.TokenInfo.TokenAmount.Amount);
                shibaCirc = shibaSupply - shibaMain;
                shibaHolders = res3.Data.Holder;

                log.Info($"GetShibaCirc: OK");
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private async void GetShibaMarketData()
        {
            try
            {
                var client = new RestClient("https://www.lanadex.com");
                var req = new RestRequest($"/api/trades/address/{serumCurrent}", DataFormat.Json);
                var res = await client.GetAsync<LanadexTrades>(req);
                marketData = res.Data.OrderByDescending(x => x.Time).First();

                log.Info($"GetShibaMarketData: OK");
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
    }
}