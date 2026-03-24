using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MarketAssetsAPI.Models.DTOs
{
    public class WebSocketPriceTick
    {
        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }
    }
}