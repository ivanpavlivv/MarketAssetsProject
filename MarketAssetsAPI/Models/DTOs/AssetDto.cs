using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAssetsAPI.Models.DTOs
{
    public class AssetDto
    {
        public string Symbol { get; set; } = string.Empty;
        public string Kind { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public string BaseCurrency { get; set; } = string.Empty;
    }
}