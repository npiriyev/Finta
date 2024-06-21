using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services.FintaChartsResponse;
using WebApplication1.Services;
using System.Collections;
using WebApplication1.Data.Entities;
using WebApplication1.Data;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FintaChartsController : ControllerBase
    {
        public static WebSocketService _webSocketService = new WebSocketService();
         


        [HttpGet("Auth")]
        public string GetAuth()
        {
            var x = FintaChartsService.GetToken().Result;

            return x.access_token;
        }

        [HttpGet("Exchanges")]
        public ExchangesResponse GetExchanges()
        {
            var x = FintaChartsService.GetExchanges().Result;

            return x;
        }

        [HttpGet("Providers")]
        public ProvidersResponse GetProviders()
        {
            var x = FintaChartsService.GetProviders().Result;

            return x;
        }

        [HttpGet("AddOrUpdateInstruments")]
        public InstrumentsResponse AddOrUpdateInstrumentsFromService(string provider = "oanda", string kind = "forex")
        {
            var x = FintaChartsService.GetInsturments(provider, kind).Result;

            return x;
        }

        [HttpGet("GetInsturments")]
        public async Task<IEnumerable> GetInstruments()
        {
            var insList = new List<InstrumentsEntity>();

            using(var db = new DataContext())
            {
                insList = await db.Instruments.ToListAsync();
            }

            return insList;
        }

        [HttpGet("GetInsturmentLastById")]
        public LastEntity? GetInstrumentLastById(Guid id)
        {
            LastEntity ins = null;

            using (var db = new DataContext())
            {
                ins = db.Lasts
                .Include(x => x.Instrument)
                .FirstOrDefault(x => x.InstrumentId == id);
            }

            return ins;
        }

        [HttpGet("GetInsturmentLastBySymbol")]
        public LastEntity? GetInstrumentLastBySymbol(string symbol)
        {
            LastEntity ins = null;

            using (var db = new DataContext())
            {
                ins = db.Lasts.Include(x => x.Instrument)
                .FirstOrDefault(x => x.Instrument.Symbol == symbol);
            }

            return ins;
        }

        [HttpGet("GetInsturmentAskById")]
        public AskEntity? GetInstrumentAskById(Guid id)
        {
            AskEntity ins = null;

            using (var db = new DataContext())
            {
               

                ins = db.Asks
                .Include(x => x.Instrument)
                .FirstOrDefault(x => x.InstrumentId == id);
            }

            return ins;
        }

        [HttpGet("GetInsturmentAskBySymbol")]
        public AskEntity? GetInstrumentAskBySymbol(string symbol)
        {
            AskEntity ins = null;

            using (var db = new DataContext())
            {
                ins = db.Asks.Include(x => x.Instrument)
                .FirstOrDefault(x => x.Instrument.Symbol == symbol);
            }

            return ins;
        }

        [HttpGet("GetInsturmentBidById")]
        public BidEntity? GetInstrumentBidById(Guid id)
        {
            BidEntity ins = null;

            using (var db = new DataContext())
            {
                

                ins = db.Bids
                .Include(x => x.Instrument)
                .FirstOrDefault(x => x.InstrumentId == id);
            }

            return ins;
        }

        [HttpGet("GetInsturmentBidBySymbol")]
        public BidEntity? GetInstrumentBidBySymbol(string symbol)
        {
            BidEntity ins = null;

            using (var db = new DataContext())
            {
                ins = db.Bids.Include(x => x.Instrument)
                .FirstOrDefault(x => x.Instrument.Symbol == symbol);
            }

            return ins;
        }

        //[HttpGet("StartWebSocketService")]
        //public async Task<string> StartService()
        //{
        //    _webSocketService.StartWebSocketClient();
        //    return "Service Started";
        //}

        //[HttpGet("StopWebSocketService")]
        //public async Task<string> StopService()
        //{
        //    _webSocketService.StopWebSocketClient();
        //    return "Service Stopped";

        //}

    }
}
