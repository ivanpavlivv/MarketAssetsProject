using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using MarketAssetsAPI.Data;
using MarketAssetsAPI.Models;
using MarketAssetsAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MarketAssetsAPI.Services
{
    public class InstrumentService
    {
        private readonly HttpClient _httpClient;
        private readonly FintachartsAuthService _authService;
        private readonly AppDbContext _db;
        private readonly FintachartsSettings _settings;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public InstrumentService(
            HttpClient httpClient,
            FintachartsAuthService authService,
            AppDbContext db,
            IOptions<FintachartsSettings> settings)
        {
            _httpClient = httpClient;
            _authService = authService;
            _db = db;
            _settings = settings.Value;
        }

        public async Task SyncInstrumentsAsync(string provider = "oanda", string kind = "forex")
        {
            var token = await _authService.GetAccessTokenAsync();
            int page = 1;
            int totalPages = 1;

            while (page <= totalPages)
            {
                var url = $"{_settings.BaseUrl}/api/instruments/v1/instruments" +
                          $"?provider={provider}&kind={kind}&page={page}&size=100";

                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<FintachartsInstrumentResponse>(json, _jsonOptions);

                if (result?.Data == null) break;

                totalPages = result.Paging.Pages;

                foreach (var instrument in result.Data)
                {
                    var existing = await _db.Assets
                        .FirstOrDefaultAsync(a => a.InstrumentId == instrument.Id);

                    if (existing == null)
                    {
                        _db.Assets.Add(new Asset
                        {
                            InstrumentId = instrument.Id,
                            Symbol = instrument.Symbol,
                            Kind = instrument.Kind,
                            Description = instrument.Description,
                            Currency = instrument.Currency,
                            BaseCurrency = instrument.BaseCurrency,
                            Provider = provider
                        });
                    }
                    else
                    {
                        existing.Symbol = instrument.Symbol;
                        existing.Description = instrument.Description;
                        existing.Currency = instrument.Currency;
                        existing.BaseCurrency = instrument.BaseCurrency;
                    }
                }

                await _db.SaveChangesAsync();
                page++;
            }
        }

        public async Task<List<Asset>> GetAllAssetsAsync()
        {
            return await _db.Assets.ToListAsync();
        }
    }
}