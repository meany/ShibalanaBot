using dm.Shibalana.Data;
using dm.Shibalana.Data.Models;
using dm.Shibalana.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace dm.Shibalana.Infos
{
    class Program
    {
        private static IServiceProvider services;
        private static IConfigurationRoot configuration;
        private static AppDbContext db;
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
            => new Program().MainAsync(args).GetAwaiter().GetResult();

        public async Task MainAsync(string[] args)
        {
            try
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("Config.Infos.json", optional: true, reloadOnChange: true)
                    .AddJsonFile("Config.Infos.Local.json", optional: true, reloadOnChange: true);

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

                var client = new RestClient("https://api.solscan.io");
                var infos = await db.AddressInfos.ToListAsync();
                foreach (var info in infos)
                {
                    var req = new RestRequest($"account?address={info.Address}", DataFormat.Json);
                    var res = await client.GetAsync<Solscan>(req);
                    decimal newAmt = decimal.Parse(res.Data.TokenInfo.TokenAmount.UiAmountString);

                    string chgInd = (newAmt > info.Amount) ? "+" : string.Empty;
                    decimal chgAmt = newAmt - info.Amount;

                    log.Info($"{info.Label}: {newAmt} ({chgInd}{chgAmt})");

                    info.Amount = newAmt;
                }

                log.Info($"Saving AddressInfos to database");
                db.UpdateRange(infos);
                db.SaveChanges();

            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
    }
}