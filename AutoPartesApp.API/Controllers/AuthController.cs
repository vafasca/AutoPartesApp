using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace AutoPartesApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new
            {
                message = "API funcionando correctamente",
                timestamp = DateTime.UtcNow,
                version = "1.0"
            });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // TODO: Implementar lógica real
            return Ok(new
            {
                token = "jwt-token-temporal",
                role = "Cliente",
                email = request.Email
            });
        }
    }
    // DTO temporal
    public record LoginRequest(string Email, string Password);
}
