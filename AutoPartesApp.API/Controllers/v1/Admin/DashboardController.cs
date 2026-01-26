using AutoPartesApp.Core.Application.Admin.Dashboard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AutoPartesApp.API.Controllers.v1.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly GetAdminDashboardUseCase _getDashboardUseCase;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(
            GetAdminDashboardUseCase getDashboardUseCase,
            ILogger<DashboardController> logger)
        {
            _getDashboardUseCase = getDashboardUseCase;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene las estadísticas y datos del dashboard del administrador
        /// </summary>
        [HttpGet("admin")]
        public async Task<IActionResult> GetAdminDashboard()
        {
            try
            {
                var dashboardData = await _getDashboardUseCase.Execute();
                return Ok(dashboardData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener datos del dashboard");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor"
                });
            }
        }
    }
}
