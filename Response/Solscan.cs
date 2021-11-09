using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dm.Shibalana.Response
{
    public class SolscanTokenAmount
    {
        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("decimals")]
        public int Decimals { get; set; }

        [JsonProperty("uiAmount")]
        public long UiAmount { get; set; }

        [JsonProperty("uiAmountString")]
        public string UiAmountString { get; set; }
    }

    public class SolscanTokenInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("decimals")]
        public int Decimals { get; set; }

        [JsonProperty("tokenAuthority")]
        public object TokenAuthority { get; set; }

        [JsonProperty("supply")]
        public string Supply { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("tokenAddress")]
        public string TokenAddress { get; set; }

        [JsonProperty("authority")]
        public string Authority { get; set; }

        [JsonProperty("tokenAmount")]
        public SolscanTokenAmount TokenAmount { get; set; }
    }

    public class SolscanData
    {
        [JsonProperty("lamports")]
        public int Lamports { get; set; }

        [JsonProperty("ownerProgram")]
        public string OwnerProgram { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("rentEpoch")]
        public int RentEpoch { get; set; }

        [JsonProperty("account")]
        public string Account { get; set; }

        [JsonProperty("tokenInfo")]
        public SolscanTokenInfo TokenInfo { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("website")]
        public string Website { get; set; }

        [JsonProperty("twitter")]
        public string Twitter { get; set; }

        [JsonProperty("tag")]
        public List<object> Tag { get; set; }

        [JsonProperty("decimals")]
        public int Decimals { get; set; }

        [JsonProperty("coingeckoId")]
        public string CoingeckoId { get; set; }

        [JsonProperty("holder")]
        public int Holder { get; set; }
    }

    public class Solscan
    {
        [JsonProperty("succcess")]
        public bool Succcess { get; set; }

        [JsonProperty("data")]
        public SolscanData Data { get; set; }
    }
}
