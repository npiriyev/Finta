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
        private static WebSocketService _webSocketService = new WebSocketService();

        [HttpGet("Auth")]
        public async Task<ActionResult<string>> GetAuth()
        {
            try
            {
                var tokenResponse = await FintaChartsService.GetToken();
                return tokenResponse?.access_token;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpGet("Exchanges")]
        public async Task<ActionResult<ExchangesResponse>> GetExchanges()
        {
            try
            {
                var exchangesResponse = await FintaChartsService.GetExchanges();
                return exchangesResponse;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpGet("Providers")]
        public async Task<ActionResult<ProvidersResponse>> GetProviders()
        {
            try
            {
                var providersResponse = await FintaChartsService.GetProviders();
                return providersResponse;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpGet("AddOrUpdateInstruments")]
        public async Task<ActionResult<InstrumentsResponse>> AddOrUpdateInstrumentsFromService(string provider = "oanda", string kind = "forex")
        {
            try
            {
                var instrumentsResponse = await FintaChartsService.GetInsturments(provider, kind);
                return instrumentsResponse;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpGet("GetInstruments")]
        public async Task<ActionResult<IEnumerable<InstrumentsEntity>>> GetInstruments()
        {
            try
            {
                using (var db = new DataContext())
                {
                    var instrumentsList = await db.Instruments.ToListAsync();
                    return instrumentsList;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpGet("GetInstrumentLastById")]
        public async Task<ActionResult<LastEntity>> GetInstrumentLastById(Guid id)
        {
            try
            {
                using (var db = new DataContext())
                {
                    var lastEntity = await db.Lasts
                        .Include(x => x.Instrument)
                        .FirstOrDefaultAsync(x => x.InstrumentId == id);

                    if (lastEntity == null)
                        return NotFound();

                    return lastEntity;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpGet("GetInstrumentLastBySymbol")]
        public async Task<ActionResult<LastEntity>> GetInstrumentLastBySymbol(string symbol)
        {
            try
            {
                using (var db = new DataContext())
                {
                    var lastEntity = await db.Lasts
                        .Include(x => x.Instrument)
                        .FirstOrDefaultAsync(x => x.Instrument.Symbol == symbol);

                    if (lastEntity == null)
                        return NotFound();

                    return lastEntity;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpGet("GetInstrumentAskById")]
        public async Task<ActionResult<AskEntity>> GetInstrumentAskById(Guid id)
        {
            try
            {
                using (var db = new DataContext())
                {
                    var askEntity = await db.Asks
                        .Include(x => x.Instrument)
                        .FirstOrDefaultAsync(x => x.InstrumentId == id);

                    if (askEntity == null)
                        return NotFound();

                    return askEntity;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpGet("GetInstrumentAskBySymbol")]
        public async Task<ActionResult<AskEntity>> GetInstrumentAskBySymbol(string symbol)
        {
            try
            {
                using (var db = new DataContext())
                {
                    var askEntity = await db.Asks
                        .Include(x => x.Instrument)
                        .FirstOrDefaultAsync(x => x.Instrument.Symbol == symbol);

                    if (askEntity == null)
                        return NotFound();

                    return askEntity;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpGet("GetInstrumentBidById")]
        public async Task<ActionResult<BidEntity>> GetInstrumentBidById(Guid id)
        {
            try
            {
                using (var db = new DataContext())
                {
                    var bidEntity = await db.Bids
                        .Include(x => x.Instrument)
                        .FirstOrDefaultAsync(x => x.InstrumentId == id);

                    if (bidEntity == null)
                        return NotFound();

                    return bidEntity;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpGet("GetInstrumentBidBySymbol")]
        public async Task<ActionResult<BidEntity>> GetInstrumentBidBySymbol(string symbol)
        {
            try
            {
                using (var db = new DataContext())
                {
                    var bidEntity = await db.Bids
                        .Include(x => x.Instrument)
                        .FirstOrDefaultAsync(x => x.Instrument.Symbol == symbol);

                    if (bidEntity == null)
                        return NotFound();

                    return bidEntity;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        //[HttpGet("StartWebSocketService")]
        //public async Task<ActionResult<string>> StartService()
        //{
        //    _webSocketService.StartWebSocketClient();
        //    return "Service Started";
        //}

        //[HttpGet("StopWebSocketService")]
        //public async Task<ActionResult<string>> StopService()
        //{
        //    _webSocketService.StopWebSocketClient();
        //    return "Service Stopped";

        //}

    }
}
