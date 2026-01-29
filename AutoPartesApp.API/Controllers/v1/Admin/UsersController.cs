using AutoPartesApp.Core.Application.DTOs.Common;
using AutoPartesApp.Core.Application.DTOs.UserDTOs;
using AutoPartesApp.Core.Application.Users;
using AutoPartesApp.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AutoPartesApp.API.Controllers.v1.Admin
{
    [Route("api/v1/admin/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly GetAllUsersUseCase _getAllUsersUseCase;
        private readonly GetUserByIdUseCase _getUserByIdUseCase;
        private readonly GetUsersByRoleUseCase _getUsersByRoleUseCase;
        private readonly CreateUserUseCase _createUserUseCase;
        private readonly UpdateUserUseCase _updateUserUseCase;
        private readonly BlockUserUseCase _blockUserUseCase;
        private readonly DeleteUserUseCase _deleteUserUseCase;
        private readonly ChangeUserRoleUseCase _changeUserRoleUseCase;
        private readonly SearchUsersUseCase _searchUsersUseCase;
        private readonly ResetPasswordUseCase _resetPasswordUseCase;
        private readonly GetCustomersUseCase _getCustomersUseCase;
        private readonly GetDeliveriesUsersUseCase _getDeliveriesUsersUseCase;
        private readonly GetUserStatsUseCase _getUserStatsUseCase;

        public UsersController(
            GetAllUsersUseCase getAllUsersUseCase,
            GetUserByIdUseCase getUserByIdUseCase,
            GetUsersByRoleUseCase getUsersByRoleUseCase,
            CreateUserUseCase createUserUseCase,
            GetUserStatsUseCase getUserStatsUseCase,
            UpdateUserUseCase updateUserUseCase,
            BlockUserUseCase blockUserUseCase,
            DeleteUserUseCase deleteUserUseCase,
            ChangeUserRoleUseCase changeUserRoleUseCase,
            SearchUsersUseCase searchUsersUseCase,
            ResetPasswordUseCase resetPasswordUseCase,
            GetCustomersUseCase getCustomersUseCase,
            GetDeliveriesUsersUseCase getDeliveriesUsersUseCase)
        {
            _getAllUsersUseCase = getAllUsersUseCase;
            _getUserByIdUseCase = getUserByIdUseCase;
            _getUsersByRoleUseCase = getUsersByRoleUseCase;
            _createUserUseCase = createUserUseCase;
            _updateUserUseCase = updateUserUseCase;
            _blockUserUseCase = blockUserUseCase;
            _deleteUserUseCase = deleteUserUseCase;
            _changeUserRoleUseCase = changeUserRoleUseCase;
            _searchUsersUseCase = searchUsersUseCase;
            _resetPasswordUseCase = resetPasswordUseCase;
            _getCustomersUseCase = getCustomersUseCase;
            _getDeliveriesUsersUseCase = getDeliveriesUsersUseCase;
            _getUserStatsUseCase = getUserStatsUseCase;
        }

        /// <summary>
        /// Obtener todos los usuarios con filtros y paginación
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResultDto<UserListItemDto>), 200)]
        public async Task<ActionResult<PagedResultDto<UserListItemDto>>> GetAll([FromQuery] UserFilterDto filters)
        {
            try
            {
                var result = await _getAllUsersUseCase.ExecuteAsync(filters);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener usuarios", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener usuario por ID con detalles completos
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserDetailDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<UserDetailDto>> GetById(string id)
        {
            try
            {
                var user = await _getUserByIdUseCase.ExecuteAsync(id);

                if (user == null)
                    return NotFound(new { message = "Usuario no encontrado" });

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener usuario", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener usuarios por rol específico
        /// </summary>
        [HttpGet("by-role/{roleType}")]
        [ProducesResponseType(typeof(List<UserListItemDto>), 200)]
        public async Task<ActionResult<List<UserListItemDto>>> GetByRole(RoleType roleType)
        {
            try
            {
                var users = await _getUsersByRoleUseCase.ExecuteAsync(roleType);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener usuarios por rol", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener solo clientes
        /// </summary>
        [HttpGet("customers")]
        [ProducesResponseType(typeof(List<UserListItemDto>), 200)]
        public async Task<ActionResult<List<UserListItemDto>>> GetCustomers()
        {
            try
            {
                var customers = await _getCustomersUseCase.ExecuteAsync();
                return Ok(customers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener clientes", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener solo repartidores
        /// </summary>
        [HttpGet("deliveries")]
        [ProducesResponseType(typeof(List<UserListItemDto>), 200)]
        public async Task<ActionResult<List<UserListItemDto>>> GetDeliveries()
        {
            try
            {
                var deliveries = await _getDeliveriesUsersUseCase.ExecuteAsync();
                return Ok(deliveries);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener repartidores", error = ex.Message });
            }
        }

        /// <summary>
        /// Buscar usuarios por nombre o email
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(typeof(List<UserListItemDto>), 200)]
        public async Task<ActionResult<List<UserListItemDto>>> Search([FromQuery] string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                    return BadRequest(new { message = "El parámetro 'query' es requerido" });

                var users = await _searchUsersUseCase.ExecuteAsync(query);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al buscar usuarios", error = ex.Message });
            }
        }

        /// <summary>
        /// Crear nuevo usuario
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(UserDetailDto), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<UserDetailDto>> Create([FromBody] CreateUserDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _createUserUseCase.ExecuteAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al crear usuario", error = ex.Message });
            }
        }

        /// <summary>
        /// Actualizar usuario existente
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(UserDetailDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<UserDetailDto>> Update(string id, [FromBody] UpdateUserDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _updateUserUseCase.ExecuteAsync(id, dto);

                if (user == null)
                    return NotFound(new { message = "Usuario no encontrado" });

                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar usuario", error = ex.Message });
            }
        }

        /// <summary>
        /// Bloquear/Desbloquear usuario (toggle IsActive)
        /// </summary>
        [HttpPatch("{id}/toggle-status")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            try
            {
                var result = await _blockUserUseCase.ExecuteAsync(id);

                if (!result)
                    return NotFound(new { message = "Usuario no encontrado" });

                return Ok(new { message = "Estado del usuario actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al cambiar estado del usuario", error = ex.Message });
            }
        }

        /// <summary>
        /// Cambiar rol del usuario
        /// </summary>
        [HttpPatch("{id}/change-role")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ChangeRole(string id, [FromBody] ChangeRoleDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _changeUserRoleUseCase.ExecuteAsync(id, dto);

                if (!result)
                    return NotFound(new { message = "Usuario no encontrado" });

                return Ok(new { message = "Rol actualizado correctamente" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al cambiar rol", error = ex.Message });
            }
        }

        /// <summary>
        /// Restablecer contraseña del usuario
        /// </summary>
        [HttpPost("{id}/reset-password")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ResetPassword(string id)
        {
            try
            {
                var temporaryPassword = await _resetPasswordUseCase.ExecuteAsync(id);

                if (temporaryPassword == null)
                    return NotFound(new { message = "Usuario no encontrado" });

                return Ok(new
                {
                    message = "Contraseña restablecida correctamente",
                    temporaryPassword = temporaryPassword
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al restablecer contraseña", error = ex.Message });
            }
        }

        /// <summary>
        /// Eliminar usuario (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var result = await _deleteUserUseCase.ExecuteAsync(id);

                if (!result)
                    return NotFound(new { message = "Usuario no encontrado" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar usuario", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener estadísticas de usuarios
        /// </summary>
        [HttpGet("stats")]
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
