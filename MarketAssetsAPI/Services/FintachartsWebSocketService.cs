using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MarketAssetsAPI.Data;
using MarketAssetsAPI.Models;
using MarketAssetsAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MarketAssetsAPI.Services
{
    public class FintachartsWebSocketService : BackgroundService
    {
        private readonly ILogger<FintachartsWebSocketService> _logger;
        private readonly IDbContextFactory<AppDbContext> _dbFactory;
        private readonly FintachartsAuthService _authService;
        private readonly PriceService _priceService;
        private readonly FintachartsSettings _settings;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public FintachartsWebSocketService(
            ILogger<FintachartsWebSocketService> logger,
            IDbContextFactory<AppDbContext> dbFactory,
            FintachartsAuthService authService,
            PriceService priceService,
            IOptions<FintachartsSettings> settings)
        {
            _logger = logger;
            _dbFactory = dbFactory;
            _authService = authService;
            _priceService = priceService;
            _settings = settings.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ConnectAndListenAsync(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "WebSocket connection failed. Reconnecting in 5 seconds...");
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }
        }

        private async Task ConnectAndListenAsync(CancellationToken stoppingToken)
        {
            var token = await _authService.GetAccessTokenAsync();
            var uri = new Uri($"{_settings.WssUrl}/api/streaming/ws/v1/realtime?token={token}");

            using var ws = new ClientWebSocket();
            await ws.ConnectAsync(uri, stoppingToken);
            _logger.LogInformation("WebSocket connected to Fintacharts");

            await SubscribeToAllAssetsAsync(ws, stoppingToken);

            await SeedInitialPricesAsync(stoppingToken);

            await ReceiveMessagesAsync(ws, stoppingToken);
        }

        private async Task SubscribeToAllAssetsAsync(ClientWebSocket ws, CancellationToken stoppingToken)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            var assets = await db.Assets.ToListAsync(stoppingToken);

            foreach (var asset in assets)
            {
                var message = new WebSocketSubscribeMessage
                {
                    Id = Guid.NewGuid().ToString(),
                    InstrumentId = asset.InstrumentId,
                    Provider = "simulation"
                };

                var json = JsonSerializer.Serialize(message);
                var bytes = Encoding.UTF8.GetBytes(json);

                await ws.SendAsync(
                    new ArraySegment<byte>(bytes),
                    WebSocketMessageType.Text,
                    endOfMessage: true,
                    stoppingToken);

                // _logger.LogInformation("Subscribed to {Symbol}", asset.Symbol);
            }
        }

        private async Task SeedInitialPricesAsync(CancellationToken stoppingToken)
        {
            try
            {
                await using var db = await _dbFactory.CreateDbContextAsync();
                var assets = await db.Assets.ToListAsync(stoppingToken);

                var token = await _authService.GetAccessTokenAsync();

                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                foreach (var asset in assets)
                {
                    try
                    {
                        var alreadySeeded = await db.AssetPrices
                            .AnyAsync(p => p.AssetId == asset.Id, stoppingToken);

                        if (alreadySeeded) continue;

                        var url = $"{_settings.BaseUrl}/api/bars/v1/bars/count-back" +
                                  $"?instrumentId={asset.InstrumentId}" +
                                  $"&provider={asset.Provider}" +
                                  $"&interval=1&periodicity=minute&barsCount=1";

                        var response = await httpClient.GetAsync(url, stoppingToken);
                        if (!response.IsSuccessStatusCode) continue;

                        var json = await response.Content.ReadAsStringAsync(stoppingToken);
                        var result = JsonSerializer.Deserialize<BarsResponse>(json, _jsonOptions);
                        var bar = result?.Data?.FirstOrDefault();

                        if (bar == null) continue;

                        await _priceService.UpsertPriceAsync(
                            asset.InstrumentId,
                            bid: null,
                            ask: null,
                            last: bar.Close);

                        _logger.LogInformation("Seeded initial price for {Symbol}: {Price}",
                            asset.Symbol, bar.Close);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to seed price for {Symbol}", asset.Symbol);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to seed initial prices");
            }
        }

        private async Task ReceiveMessagesAsync(ClientWebSocket ws, CancellationToken stoppingToken)
        {
            var buffer = new byte[4096];

            while (ws.State == WebSocketState.Open && !stoppingToken.IsCancellationRequested)
            {
                using var ms = new MemoryStream();

                WebSocketReceiveResult result;

                do
                {
                    result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), stoppingToken);
                    ms.Write(buffer, 0, result.Count);
                }
                while (!result.EndOfMessage);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    _logger.LogWarning("WebSocket closed by server");
                    break;
                }

                var json = Encoding.UTF8.GetString(ms.ToArray());
                await HandleMessageAsync(json);
            }
        }

        private async Task HandleMessageAsync(string json)
        {
            try
            {
                var message = JsonSerializer.Deserialize<WebSocketPriceMessage>(json, _jsonOptions);

                if (message == null || string.IsNullOrEmpty(message.InstrumentId))
                    return;

                if (message.Type != "l1-update" && message.Type != "l1-subscription")
                    return;

                if (message.Bid == null && message.Ask == null && message.Last == null)
                    return;

                await _priceService.UpsertPriceAsync(
                    message.InstrumentId,
                    bid: message.Bid?.Price,
                    ask: message.Ask?.Price,
                    last: message.Last?.Price);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to handle WebSocket message: {Json}", json);
            }
        }
    }
}