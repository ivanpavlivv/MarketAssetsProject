using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MarketAssetsAPI.Models.DTOs
{
    public class WebSocketSubscribeMessage
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "l1-subscription";

        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("instrumentId")]
        public string InstrumentId { get; set; } = string.Empty;

        [JsonPropertyName("provider")]
        public string Provider { get; set; } = "simulation";

        [JsonPropertyName("subscribe")]
        public bool Subscribe { get; set; } = true;

        [JsonPropertyName("kinds")]
        public List<string> Kinds { get; set; } = new() { "ask", "bid", "last" };
    }
}