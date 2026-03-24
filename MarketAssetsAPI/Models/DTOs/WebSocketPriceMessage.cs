using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MarketAssetsAPI.Models.DTOs
{
    public class WebSocketPriceMessage
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("instrumentId")]
        public string InstrumentId { get; set; } = string.Empty;

        [JsonPropertyName("provider")]
        public string Provider { get; set; } = string.Empty;

        [JsonPropertyName("last")]
        public WebSocketPriceTick? Last { get; set; }

        [JsonPropertyName("ask")]
        public WebSocketPriceTick? Ask { get; set; }

        [JsonPropertyName("bid")]
        public WebSocketPriceTick? Bid { get; set; }
    }
}