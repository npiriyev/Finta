using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Security.Principal;
using System.Text;
using System.Threading.Channels;
using WebApplication1.Data;
using WebApplication1.Data.Entities;
using WebApplication1.Services.FintaChartsResponse;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace WebApplication1.Services
{
    public class FintaChartsService
    {
        public static string BaseUrl = "https://platform.fintacharts.com/";
        public static string Username = "r_test@fintatech.com";
        public static string Password = "kisfiz-vUnvy9-sopnyv";

        public static async Task<AuthResponse?> GetToken()
        {
            var url = BaseUrl + "identity/realms/fintatech/protocol/openid-connect/token";
            var client = new HttpClient();

            // Set up the request content
            var content = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("grant_type", "password"),
            new KeyValuePair<string, string>("client_id", "app-cli"),
            new KeyValuePair<string, string>("username", Username),
            new KeyValuePair<string, string>("password", Password)
        });

            // Set up the request headers
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "{{TOKEN}}");

            // Send the POST request

            var response = await client.PostAsync(url, content);

            // Check the response status code
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                AuthResponse myDeserializedClass = JsonConvert.DeserializeObject<AuthResponse>(responseBody);
                return myDeserializedClass;
            }
            return null;
         
        }

        public static async Task<ProvidersResponse> GetProviders()
        {
            var token = await GetToken();
            var url = BaseUrl + "api/instruments/v1/providers";
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.access_token);

            // Send the POST request

            var response = await client.GetAsync(url);

            // Check the response status code
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                ProvidersResponse myDeserializedClass = JsonConvert.DeserializeObject<ProvidersResponse>(responseBody);
                return myDeserializedClass;
            }
            return null;
        }

        public static async Task<ExchangesResponse> GetExchanges()
        {
            var token = GetToken().Result.access_token;
            var url = BaseUrl + "api/instruments/v1/exchanges";
            var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Send the POST request

            var response = await client.GetAsync(url);

            // Check the response status code
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                ExchangesResponse myDeserializedClass = JsonConvert.DeserializeObject<ExchangesResponse>(responseBody);
                return myDeserializedClass;
            }
            return null;

        }

        public static async Task<InstrumentsResponse> GetInsturments(string provider, string kind)
        {
            var token = GetToken().Result.access_token;
            var url = BaseUrl + $"api/instruments/v1/instruments?provider={provider}&kind={kind}";
            var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Send the POST request

            var response = await client.GetAsync(url);

            // Check the response status code
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                InstrumentsResponse myDeserializedClass = JsonConvert.DeserializeObject<InstrumentsResponse>(responseBody);

                await MapInstruments(myDeserializedClass);

                return myDeserializedClass;
            }
            return null;

        }


        private static async Task MapInstruments(InstrumentsResponse resp)
        {
            if (resp != null)
            {
                using (var db = new DataContext())
                {
                    foreach (var inst in resp.data)
                    {
                        var instrumentId = Guid.Parse(inst.id);
                        var instrument = db.Instruments.SingleOrDefault(i => i.Id == instrumentId);

                        if (instrument == null)
                        {
                            // Instrument does not exist, so create a new one
                            instrument = new InstrumentsEntity
                            {
                                Id = instrumentId,
                                Symbol = inst.symbol,
                                Kind = inst.kind,
                                Description = inst.description,
                                TickSize = inst.tickSize,
                                Currency = inst.currency,
                                BaseCurrency = inst.baseCurrency
                            };

                            db.Instruments.Add(instrument);
                        }
                        else
                        {
                            // Instrument exists, so update its properties
                            instrument.Symbol = inst.symbol;
                            instrument.Kind = inst.kind;
                            instrument.Description = inst.description;
                            instrument.TickSize = inst.tickSize;
                            instrument.Currency = inst.currency;
                            instrument.BaseCurrency = inst.baseCurrency;

                            db.Instruments.Update(instrument);
                        }

                        var instrumentMappings = new List<InsturmentMappingEntity>();

                        // Add or update mappings
                        if (inst.mappings.dxfeed != null)
                        {
                            AddOrUpdateMapping(db, instrument, "dxfeed", inst.mappings.dxfeed.symbol, inst.mappings.dxfeed.exchange, inst.mappings.dxfeed.defaultOrderSize, instrumentMappings);
                        }

                        if (inst.mappings.oanda != null)
                        {
                            AddOrUpdateMapping(db, instrument, "oanda", inst.mappings.oanda.symbol, inst.mappings.oanda.exchange, inst.mappings.oanda.defaultOrderSize, instrumentMappings);
                        }

                        if (inst.mappings.simulation != null)
                        {
                            AddOrUpdateMapping(db, instrument, "simulation", inst.mappings.simulation.symbol, inst.mappings.simulation.exchange, inst.mappings.simulation.defaultOrderSize, instrumentMappings);
                        }

                        if (inst.mappings.activetick != null)
                        {
                            AddOrUpdateMapping(db, instrument, "activetick", inst.mappings.activetick.symbol, inst.mappings.activetick.exchange, inst.mappings.activetick.defaultOrderSize, instrumentMappings);
                        }

                        await db.InstrumentMappings.AddRangeAsync(instrumentMappings);
                        await db.SaveChangesAsync();
                    }
                }
            }
        }

            private static void AddOrUpdateMapping(DataContext db, InstrumentsEntity instrument, string mappingType, string symbol, string exchange, double defaultOrderSize, List<InsturmentMappingEntity> instrumentMappings)
            {
                var mapping = db.InstrumentMappings.SingleOrDefault(m => m.Instrument.Id == instrument.Id && m.MappingType == mappingType);

                if (mapping == null)
                {
                    // Mapping does not exist, so create a new one
                    mapping = new InsturmentMappingEntity
                    {
                        Instrument = instrument,
                        MappingType = mappingType,
                        Symbol = symbol,
                        Exhange = exchange,
                        DefaultOrderSize = defaultOrderSize
                    };

                    instrumentMappings.Add(mapping);
                }
                else
                {
                    // Mapping exists, so update its properties
                    mapping.Symbol = symbol;
                    mapping.Exhange = exchange;
                    mapping.DefaultOrderSize = defaultOrderSize;

                    db.InstrumentMappings.Update(mapping);
                }
            }



    }
}
