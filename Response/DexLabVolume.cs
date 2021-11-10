using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dm.Shibalana.Response
{
    public class DexLabVolumeSummary
    {
        [JsonProperty("totalVolume")]
        public decimal TotalVolume { get; set; }

        [JsonProperty("sellVolume")]
        public decimal SellVolume { get; set; }

        [JsonProperty("buyVolume")]
        public decimal BuyVolume { get; set; }

        [JsonProperty("highPrice")]
        public decimal HighPrice { get; set; }

        [JsonProperty("lowPrice")]
        public decimal LowPrice { get; set; }
    }

    public class DexLabVolumeLast24hOrder
    {
        [JsonProperty("exist")]
        public bool Exist { get; set; }

        [JsonProperty("time")]
        public DateTime Time { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("percent")]
        public string Percent { get; set; }
    }

    public class DexLabVolumeData
    {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("programId")]
        public string ProgramId { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("summary")]
        public DexLabVolumeSummary Summary { get; set; }

        [JsonProperty("last24hOrder")]
        public DexLabVolumeLast24hOrder Last24hOrder { get; set; }
    }

    public class DexLabVolume
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("data")]
        public DexLabVolumeData Data { get; set; }
    }
}
