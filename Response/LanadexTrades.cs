using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dm.Shibalana.Response
{
    public class LanadexTradesDatum
    {
        [JsonProperty("market")]
        public string Market { get; set; }

        [JsonProperty("marketAddress")]
        public string MarketAddress { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("side")]
        public string Side { get; set; }

        [JsonProperty("time")]
        public ulong Time { get; set; }

        [JsonProperty("orderId")]
        public string OrderId { get; set; }

        [JsonProperty("feeCost")]
        public int FeeCost { get; set; }
    }

    public class LanadexTrades
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("data")]
        public List<LanadexTradesDatum> Data { get; set; }
    }
}
