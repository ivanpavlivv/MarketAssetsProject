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

        public decimal? Bid { get; set; }
        public decimal? Ask { get; set; }
        public decimal? Last { get; set; }

        public DateTime UpdatedAt { get; set; }

        public Asset Asset { get; set; } = null!;
    }
}