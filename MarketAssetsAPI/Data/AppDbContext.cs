using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketAssetsAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MarketAssetsAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Asset> Assets { get; set; }
        public DbSet<AssetPrice> AssetPrices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Asset>()
            .HasIndex(a => a.Symbol)
            .IsUnique();

            modelBuilder.Entity<AssetPrice>()
                .HasOne(p => p.Asset)
                .WithOne(a => a.LatestPrice)
                .HasForeignKey<AssetPrice>(p => p.AssetId);
        }
    }
}