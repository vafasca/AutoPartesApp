using AutoPartesApp.Core.Application.DTOs.UserDTOs;
using AutoPartesApp.Core.Application.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AutoPartesApp.API.Controllers.v1.Admin
{
    [Route("api/v1/admin/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class UserStatsController : ControllerBase
    {
        private readonly GetUserStatsUseCase _getUserStatsUseCase;

        public UserStatsController(GetUserStatsUseCase getUserStatsUseCase)
        {
            _getUserStatsUseCase = getUserStatsUseCase;
        }

        /// <summary>
        /// Obtener estadísticas globales de usuarios
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(UserStatsDto), 200)]
        public async Task<ActionResult<UserStatsDto>> GetStats()
        {
            try
            {
                var stats = await _getUserStatsUseCase.ExecuteAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener estadísticas", error = ex.Message });
            }
        }
    }
}
