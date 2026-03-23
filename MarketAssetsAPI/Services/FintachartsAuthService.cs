using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MarketAssetsAPI.Models;
using Microsoft.Extensions.Options;

namespace MarketAssetsAPI.Services
{
    public class FintachartsAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly FintachartsSettings _settings;
        private string? _accessToken;
        private string? _refreshToken;
        private DateTime _tokenExpiresAt = DateTime.MinValue;

        public FintachartsAuthService(HttpClient httpClient, IOptions<FintachartsSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            if (_accessToken != null && DateTime.UtcNow < _tokenExpiresAt.AddSeconds(-30))
                return _accessToken;

            if (_refreshToken != null)
            {
                var refreshed = await TryRefreshTokenAsync();
                if (refreshed) return _accessToken!;
            }

            await LoginAsync();
            return _accessToken!;
        }

        private async Task LoginAsync()
        {
            var url = $"{_settings.BaseUrl}/identity/realms/{_settings.Realm}/protocol/openid-connect/token";

            var form = new Dictionary<string, string>
            {
                ["grant_type"] = "password",
                ["client_id"] = _settings.ClientId,
                ["username"] = _settings.Username,
                ["password"] = _settings.Password
            };

            var response = await _httpClient.PostAsync(url, new FormUrlEncodedContent(form));
            response.EnsureSuccessStatusCode();

            await ParseTokenResponse(response);
        }

        private async Task<bool> TryRefreshTokenAsync()
        {
            try
            {
                var url = $"{_settings.BaseUrl}/identity/realms/{_settings.Realm}/protocol/openid-connect/token";

                var form = new Dictionary<string, string>
                {
                    ["grant_type"] = "refresh_token",
                    ["client_id"] = _settings.ClientId,
                    ["refresh_token"] = _refreshToken!
                };

                var response = await _httpClient.PostAsync(url, new FormUrlEncodedContent(form));
                if (!response.IsSuccessStatusCode) return false;

                await ParseTokenResponse(response);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task ParseTokenResponse(HttpResponseMessage response)
        {
            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);

            _accessToken = doc.RootElement.GetProperty("access_token").GetString()!;
            _refreshToken = doc.RootElement.GetProperty("refresh_token").GetString()!;

            var expiresIn = doc.RootElement.GetProperty("expires_in").GetInt32();
            _tokenExpiresAt = DateTime.UtcNow.AddSeconds(expiresIn);
        }
    }
}