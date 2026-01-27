using AutoPartesApp.Core.Application.DTOs.AdminDTOs;
using AutoPartesApp.Core.Application.Inventory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AutoPartesApp.API.Controllers.v1.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class InventoryController : ControllerBase
    {
        private readonly GetInventoryStatsUseCase _getInventoryStatsUseCase;

        public InventoryController(GetInventoryStatsUseCase getInventoryStatsUseCase)
        {
            _getInventoryStatsUseCase = getInventoryStatsUseCase;
        }

        /// <summary>
        /// Obtener estadísticas del inventario
        /// </summary>
        [HttpGet("stats")]
        [ProducesResponseType(typeof(InventoryStatsDto), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetStats()
        {
            try
            {
                var stats = await _getInventoryStatsUseCase.Execute();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener estadísticas del inventario", error = ex.Message });
            }
        }
    }
}
