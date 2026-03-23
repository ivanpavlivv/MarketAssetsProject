using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketAssetsAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePriceData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Close",
                table: "AssetPrices");

            migrationBuilder.DropColumn(
                name: "High",
                table: "AssetPrices");

            migrationBuilder.DropColumn(
                name: "Low",
                table: "AssetPrices");

            migrationBuilder.DropColumn(
                name: "Open",
                table: "AssetPrices");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "AssetPrices");

            migrationBuilder.DropColumn(
                name: "Volume",
                table: "AssetPrices");

            migrationBuilder.AddColumn<decimal>(
                name: "Ask",
                table: "AssetPrices",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Bid",
                table: "AssetPrices",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Last",
                table: "AssetPrices",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ask",
                table: "AssetPrices");

            migrationBuilder.DropColumn(
                name: "Bid",
                table: "AssetPrices");

            migrationBuilder.DropColumn(
                name: "Last",
                table: "AssetPrices");

            migrationBuilder.AddColumn<decimal>(
                name: "Close",
                table: "AssetPrices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "High",
                table: "AssetPrices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Low",
                table: "AssetPrices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Open",
                table: "AssetPrices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "Timestamp",
                table: "AssetPrices",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "Volume",
                table: "AssetPrices",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
