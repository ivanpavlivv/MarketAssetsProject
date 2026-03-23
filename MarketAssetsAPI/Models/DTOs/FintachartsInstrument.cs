using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MarketAssetsAPI.Models.DTOs
{
    public class FintachartsInstrument
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("symbol")]
        public string Symbol { get; set; } = string.Empty;

        [JsonPropertyName("kind")]
        public string Kind { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("currency")]
        public string Currency { get; set; } = string.Empty;

        [JsonPropertyName("baseCurrency")]
        public string BaseCurrency { get; set; } = string.Empty;

        [JsonPropertyName("mappings")]
        public Dictionary<string, FintachartsMapping> Mappings { get; set; } = new();
    }
}