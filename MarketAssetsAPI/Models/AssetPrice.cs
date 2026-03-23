using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAssetsAPI.Models
{
    public class AssetPrice
    {
        public int Id { get; set; }
        public int AssetId { get; set; }

        // OHLCV fields
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public long Volume { get; set; }
        public DateTime Timestamp { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Asset Asset { get; set; } = null!;
    }
}