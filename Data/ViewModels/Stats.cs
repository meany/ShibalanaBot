using dm.Shibalana.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace dm.Shibalana.Data.ViewModels
{
    public class AllInfo
    {
        public Price Price { get; set; }
        public Stat Stat { get; set; }

        public bool IsOutOfSync()
        {
            var oosStat = Stat.Date.AddMinutes(30) <= DateTime.UtcNow;
            var oosPrice = Price.Date.AddMinutes(30) <= DateTime.UtcNow;
            return (oosStat || oosPrice);
        }
    }
}
