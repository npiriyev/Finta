using Newtonsoft.Json;

namespace WebApplication1.Services.FintaChartsResponse
{
    public class Data
    {
        public List<string> oanda { get; set; }
        public List<string> dxfeed { get; set; }
        public List<string> simulation { get; set; }
        public List<string> alpaca { get; set; }
        public List<string> cryptoquote { get; set; }

        [JsonProperty("active-tick")]
        public List<string> activetick { get; set; }
    }

    public class ExchangesResponse
    {
        public Data data { get; set; }
    }
}
