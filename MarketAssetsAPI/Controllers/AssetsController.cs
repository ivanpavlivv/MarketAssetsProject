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

        public AssetsController(InstrumentService instrumentService)
        {
            _instrumentService = instrumentService;
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
        /// Fetches instruments from Fintacharts and syncs them into the database.
        /// </summary>
        [HttpPost("sync")]
        public async Task<IActionResult> SyncAssets(
            [FromQuery] string provider = "oanda",
            [FromQuery] string kind = "forex")
        {
            await _instrumentService.SyncInstrumentsAsync(provider, kind);
            return Ok(new { message = $"Sync complete for provider={provider}, kind={kind}" });
        }
    }
}