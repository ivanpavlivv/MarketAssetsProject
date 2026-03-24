using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MarketAssetsAPI.Models.DTOs
{
    public class BarsResponse
    {
        [JsonPropertyName("data")]
        public List<Bar> Data { get; set; } = new();
    }

    public class Bar
    {
        [JsonPropertyName("t")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("o")]
        public decimal Open { get; set; }

        [JsonPropertyName("h")]
        public decimal High { get; set; }

        [JsonPropertyName("l")]
        public decimal Low { get; set; }

        [JsonPropertyName("c")]
        public decimal Close { get; set; }

        [JsonPropertyName("v")]
        public decimal Volume { get; set; }
    }
}