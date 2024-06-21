using Newtonsoft.Json;

namespace WebApplication1.Services.FintaChartsResponse
{
    public class ActiveTick
    {
        public string symbol { get; set; }
        public string exchange { get; set; }
        public int defaultOrderSize { get; set; }
    }

    public class Datum
    {
        public string id { get; set; }
        public string symbol { get; set; }
        public string kind { get; set; }
        public string description { get; set; }
        public double tickSize { get; set; }
        public string currency { get; set; }
        public string baseCurrency { get; set; }
        public Mappings mappings { get; set; }
    }

    public class Dxfeed
    {
        public string symbol { get; set; }
        public string exchange { get; set; }
        public int defaultOrderSize { get; set; }
    }

    public class Mappings
    {
        [JsonProperty("active-tick")]
        public ActiveTick activetick { get; set; }
        public Simulation simulation { get; set; }
        public Oanda oanda { get; set; }
        public Dxfeed dxfeed { get; set; }
    }

    public class Oanda
    {
        public string symbol { get; set; }
        public string exchange { get; set; }
        public int defaultOrderSize { get; set; }
    }

    public class Paging
    {
        public int page { get; set; }
        public int pages { get; set; }
        public int items { get; set; }
    }

    public class InstrumentsResponse
    {
        public Paging paging { get; set; }
        public List<Datum> data { get; set; }
    }

    public class Simulation
    {
        public string symbol { get; set; }
        public string exchange { get; set; }
        public int defaultOrderSize { get; set; }
    }

}
