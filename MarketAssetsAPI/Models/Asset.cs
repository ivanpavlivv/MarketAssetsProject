using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAssetsAPI.Models
{
    public class Asset
    {
        public int Id { get; set; }
        public string InstrumentId { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public string Kind { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public string BaseCurrency { get; set; } = string.Empty;

        public AssetPrice? LatestPrice { get; set; }
    }
}