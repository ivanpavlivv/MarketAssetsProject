using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketAssetsAPI.Data;
using MarketAssetsAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MarketAssetsAPI.Services
{
    public class PriceService
    {
        private readonly IDbContextFactory<AppDbContext> _dbFactory;

        public PriceService(IDbContextFactory<AppDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task UpsertPriceAsync(string instrumentId, decimal? bid, decimal? ask, decimal? last)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            var asset = await db.Assets
                .FirstOrDefaultAsync(a => a.InstrumentId == instrumentId);

            if (asset == null) return;

            var price = await db.AssetPrices
                .FirstOrDefaultAsync(p => p.AssetId == asset.Id);

            if (price == null)
            {
                db.AssetPrices.Add(new AssetPrice
                {
                    AssetId = asset.Id,
                    Bid = bid,
                    Ask = ask,
                    Last = last,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            else
            {
                if (bid != null) price.Bid = bid;
                if (ask != null) price.Ask = ask;
                if (last != null) price.Last = last;
                price.UpdatedAt = DateTime.UtcNow;
            }

            await db.SaveChangesAsync();
        }

        public async Task<List<AssetWithPrice>> GetPricesAsync(List<string> symbols)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            var query = db.Assets
                .Include(a => a.LatestPrice)
                .AsQueryable();

            if (symbols.Any())
                query = query.Where(a => symbols.Contains(a.Symbol));

            return await query
                .Select(a => new AssetWithPrice
                {
                    Symbol = a.Symbol,
                    Description = a.Description,
                    Kind = a.Kind,
                    Bid = a.LatestPrice != null ? a.LatestPrice.Bid : null,
                    Ask = a.LatestPrice != null ? a.LatestPrice.Ask : null,
                    Last = a.LatestPrice != null ? a.LatestPrice.Last : null,
                    UpdatedAt = a.LatestPrice != null ? a.LatestPrice.UpdatedAt : null
                })
                .ToListAsync();
        }
    }

    public class AssetWithPrice
    {
        public string Symbol { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Kind { get; set; } = string.Empty;
        public decimal? Bid { get; set; }
        public decimal? Ask { get; set; }
        public decimal? Last { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}