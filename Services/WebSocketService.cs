using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;
using WebApplication1.Data.Entities;
using WebApplication1.Data;

namespace WebApplication1.Services
{
    public class WebSocketService
    {
        private CancellationTokenSource _cancellationTokenSource;
        private Task _webSocketTask;

        public static void StartWebSocketService(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var webSocketService = services.GetRequiredService<WebSocketService>();
                webSocketService.StartWebSocketClient();
            }
        }

        public void StartWebSocketClient()
        {
            if (_webSocketTask == null || _webSocketTask.IsCompleted)
            {
                _cancellationTokenSource = new CancellationTokenSource();
                _webSocketTask = GetRealTimeData(_cancellationTokenSource.Token);
            }
        }

        public void StopWebSocketClient()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
            }
        }

        private async Task GetRealTimeData(CancellationToken cancellationToken)
        {
            byte[] buffer = new byte[1024];
            var client = new ClientWebSocket();
            var token = FintaChartsService.GetToken().Result.access_token;

            Uri uri = new Uri($"wss://platform.fintacharts.com/api/streaming/ws/v1/realtime?token={token}");

            await client.ConnectAsync(uri, cancellationToken);

            while (client.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            {
                var subscriptionMessage = new
                {
                    type = "l1-subscription",
                    id = "1",
                    instrumentId = "ad9e5345-4c3b-41fc-9437-1d253f62db52",
                    provider = "simulation",
                    subscribe = true,
                    kinds = new[] { "ask", "bid", "last" }
                };

                var messageJson = JsonConvert.SerializeObject(subscriptionMessage);

                await SendMessage(client, messageJson, cancellationToken);

                await ReceiveMessages(client, cancellationToken);

                if (cancellationToken.IsCancellationRequested)
                {
                    await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", cancellationToken);
                }
            }
        }

        private async Task SendMessage(ClientWebSocket client, string message, CancellationToken cancellationToken)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            await client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, cancellationToken);
        }

        private async Task<string> ReceiveMessages(ClientWebSocket client, CancellationToken cancellationToken)
        {
            byte[] buffer = new byte[1024];
            string message = "";
            while (client.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            {
                var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var json = JObject.Parse(message);
                    await ProcessUpdate(json);
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    break;
                }
            }
            return message;
        }

        private async Task ProcessUpdate(JObject json)
        {
            if (json["last"] != null)
            {
                var lastEntity = new LastEntity
                {
                    Timestamp = json["last"]["timestamp"]?.ToObject<DateTime>() ?? DateTime.MinValue,
                    Price = json["last"]["price"]?.ToObject<float>() ?? 0,
                    Volume = json["last"]["volume"]?.ToObject<int>() ?? 0,
                    Change = json["last"]["change"]?.ToObject<float>() ?? 0,
                    ChangePct = json["last"]["changePct"]?.ToObject<float>() ?? 0,
                    InstrumentId = json["instrumentId"]?.ToObject<Guid>() ?? Guid.Empty,
                    Provider = json["provider"]?.ToObject<string>(),
                    Type = json["type"]?.ToObject<string>()
                };

                using (var db = new DataContext())
                {
                    db.Lasts.Add(lastEntity);
                    db.SaveChanges();
                }
            }
            else if (json["ask"] != null)
            {
                var askEntity = new AskEntity
                {
                    Timestamp = json["ask"]["timestamp"]?.ToObject<DateTime>() ?? DateTime.MinValue,
                    Price = json["ask"]["price"]?.ToObject<float>() ?? 0,
                    Volume = json["ask"]["volume"]?.ToObject<int>() ?? 0,
                    InstrumentId = json["instrumentId"]?.ToObject<Guid>() ?? Guid.Empty,
                    Provider = json["provider"]?.ToObject<string>(),
                    Type = json["type"]?.ToObject<string>()
                };

                using (var db = new DataContext())
                {
                    db.Asks.Add(askEntity);
                    db.SaveChanges();
                }
            }
            else if (json["bid"] != null)
            {
                var bidEntity = new BidEntity
                {
                    Timestamp = json["bid"]["timestamp"]?.ToObject<DateTime>() ?? DateTime.MinValue,
                    Price = json["bid"]["price"]?.ToObject<float>() ?? 0,
                    Volume = json["bid"]["volume"]?.ToObject<int>() ?? 0,
                    InstrumentId = json["instrumentId"]?.ToObject<Guid>() ?? Guid.Empty,
                    Provider = json["provider"]?.ToObject<string>(),
                    Type = json["type"]?.ToObject<string>()
                };

                using (var db = new DataContext())
                {
                    db.Bids.Add(bidEntity);
                    db.SaveChanges();
                }
            }
        }

    }

}
