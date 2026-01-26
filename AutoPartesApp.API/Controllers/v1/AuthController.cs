using AutoPartesApp.Domain.Entities;
using AutoPartesApp.Domain.Enums;
using AutoPartesApp.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AutoPartesApp.Core.Application.DTOs;

namespace AutoPartesApp.API.Controllers.v1
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IUserRepository userRepository,
            ILogger<AuthController> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Email) ||
                    string.IsNullOrWhiteSpace(request.Password))
                {
                    return BadRequest(new LoginResponse
                    {
                        Success = false,
                        Message = "Email y contraseña son requeridos"
                    });
                }

                // Buscar usuario por email
                var user = await _userRepository.GetByEmailAsync(request.Email);

                if (user == null || !user.IsActive)
                {
                    return Unauthorized(new LoginResponse
                    {
                        Success = false,
                        Message = "Credenciales inválidas"
                    });
                }

                // NOTA: En producción, deberías verificar el hash de la contraseña
                // Por ahora, simulamos validación exitosa
                // TODO: Implementar verificación de password hash con BCrypt

                // Actualizar último login
                user.LastLoginAt = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);

                return Ok(new LoginResponse
                {
                    Success = true,
                    Message = "Login exitoso",
                    User = new UserDto
                    {
                        Id = user.Id,
                        Email = user.Email,
                        FullName = user.FullName,
                        Role = user.RoleType.ToFriendlyString(),
                        RoleType = user.RoleType,
                        Phone = user.Phone,
                        AvatarUrl = user.AvatarUrl,
                        IsActive = user.IsActive
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en login para {Email}", request.Email);
                return StatusCode(500, new LoginResponse
                {
                    Success = false,
                    Message = "Error interno del servidor"
                });
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest request)
        {
            try
            {
                // Validaciones
                if (string.IsNullOrWhiteSpace(request.Email) ||
                    string.IsNullOrWhiteSpace(request.Password) ||
                    string.IsNullOrWhiteSpace(request.FullName))
                {
                    return BadRequest(new RegisterResponse
                    {
                        Success = false,
                        Message = "Todos los campos son requeridos"
                    });
                }

                // Verificar si el email ya existe
                var existingUser = await _userRepository.GetByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    return Conflict(new RegisterResponse
                    {
                        Success = false,
                        Message = "El email ya está registrado"
                    });
                }

                // Crear nuevo usuario
                var newUser = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = request.Email,
                    FullName = request.FullName,
                    Phone = request.Phone ?? "",
                    // TODO: Hashear password con BCrypt
                    PasswordHash = request.Password, // TEMPORAL - hashear en producción
                    RoleType = RoleType.Client, // Por defecto, nuevos usuarios son clientes
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _userRepository.CreateAsync(newUser);

                return Ok(new RegisterResponse
                {
                    Success = true,
                    Message = "Usuario registrado exitosamente",
                    UserId = newUser.Id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en registro para {Email}", request.Email);
                return StatusCode(500, new RegisterResponse
                {
                    Success = false,
                    Message = "Error interno del servidor"
                });
            }
        }
    }
}
