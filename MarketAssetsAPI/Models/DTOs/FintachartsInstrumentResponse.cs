using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MarketAssetsAPI.Models.DTOs
{
    public class FintachartsInstrumentResponse
    {
        [JsonPropertyName("data")]
        public List<FintachartsInstrument> Data { get; set; } = new();

        [JsonPropertyName("paging")]
        public FintachartsPaging Paging { get; set; } = new();

    }
}