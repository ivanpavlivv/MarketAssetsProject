using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketAssetsAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace MarketAssetsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssetsController : ControllerBase
    {
        private readonly InstrumentService _instrumentService;
        private readonly PriceService _priceService;

        public AssetsController(InstrumentService instrumentService, PriceService priceService)
        {
            _instrumentService = instrumentService;
            _priceService = priceService;
        }

        /// <summary>
        /// Returns list of all supported market assets stored in the database.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAssets()
        {
            var assets = await _instrumentService.GetAllAssetsAsync();
            return Ok(assets);
        }

        /// <summary>
        /// Returns price information for one or more assets.
        /// </summary>
        [HttpGet("prices")]
        public async Task<IActionResult> GetPrices([FromQuery] string? symbols)
        {
            var symbolList = string.IsNullOrWhiteSpace(symbols)
                ? new List<string>()
                : symbols.Split(',').Select(s => s.Trim()).ToList();

            var prices = await _priceService.GetPricesAsync(symbolList);
            return Ok(prices);
        }
    }
}