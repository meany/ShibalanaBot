using dm.Shibalana.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace dm.Shibalana.Data.ViewModels
{
    public class AllInfo
    {
        public List<Price> Prices { get; set; }
        public Price FinalPrice { get; set; }
        public Stat Stat { get; set; }

        public bool IsOutOfSync()
        {
            var oosStat = Stat.Date.AddMinutes(30) <= DateTime.UtcNow;
            var oosPrice = FinalPrice.Date.AddMinutes(30) <= DateTime.UtcNow;
            return (oosStat || oosPrice);
        }
    }
}
