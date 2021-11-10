using dm.Shibalana.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace dm.Shibalana.Data
{
    public static class Common
    {
        public static async Task<ViewModels.AllInfo> GetAllInfo(AppDbContext db)
        {
            var vm = new ViewModels.AllInfo();
            vm.Stat = await GetStats(db);
            vm.Prices = await GetPrices(db, vm.Stat.Group);

            if (vm.Prices == null || vm.Prices.Count == 0)
            {
                vm.Prices = new List<Price>();
                vm.Prices.Add(await db.Prices
                    .AsNoTracking()
                    .OrderByDescending(x => x.Date)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false));
            }

            vm.FinalPrice = new Price
            {
                Date = vm.Prices[0].Date,
                CircMarketCapUSD = vm.Prices[0].CircMarketCapUSD,
                FullMarketCapUSD = vm.Prices[0].FullMarketCapUSD,
                PriceSHIBAForOneUSDC = vm.Prices.WeightedAverage(x => x.PriceSHIBAForOneUSDC, x => x.VolumeUSD),
                PriceUSD = vm.Prices.WeightedAverage(x => x.PriceUSD, x => x.VolumeUSD),
                PriceUSDCForOneSHIBA = vm.Prices.WeightedAverage(x => x.PriceUSDCForOneSHIBA, x => x.VolumeUSD),
                VolumeUSD = vm.Prices.Sum(x => x.VolumeUSD)
            };

            return vm;
        }

        public static async Task<Stat> GetStats(AppDbContext db)
        {
            return await db.Stats
                .AsNoTracking()
                .OrderByDescending(x => x.Date)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
        }

        public static async Task<List<Price>> GetPrices(AppDbContext db, Guid group = new Guid())
        {
            return await db.Prices
                .AsNoTracking()
                .Where(x => group == new Guid() || x.Group == group)
                .ToListAsync()
                .ConfigureAwait(false);
        }
    }
}
