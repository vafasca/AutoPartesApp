using AutoPartesApp.Domain.Entities;
using AutoPartesApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Domain.Interfaces
{
    public interface IUserRepository
    {
        // Métodos existentes
        Task<User?> GetByIdAsync(string id);
        Task<User?> GetByEmailAsync(string email);
        Task<List<User>> GetAllAsync();
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(User user);
        Task<bool> DeleteAsync(string id);

        // 🆕 NUEVOS MÉTODOS

        /// <summary>
        /// Obtener usuarios paginados con filtros
        /// </summary>
        Task<(List<User> Users, int TotalCount)> GetPagedAsync(
            string? searchQuery = null,
            RoleType? roleType = null,
            bool? isActive = null,
            DateTime? createdFrom = null,
            DateTime? createdTo = null,
            int pageNumber = 1,
            int pageSize = 20
        );

        /// <summary>
        /// Obtener usuarios por rol específico
        /// </summary>
        Task<List<User>> GetByRoleAsync(RoleType roleType);

        /// <summary>
        /// Obtener usuario con sus órdenes
        /// </summary>
        Task<User?> GetUserWithOrdersAsync(string userId);

        /// <summary>
        /// Obtener usuario (delivery) con sus entregas
        /// </summary>
        Task<User?> GetUserWithDeliveriesAsync(string userId);

        /// <summary>
        /// Cambiar estado activo/inactivo
        /// </summary>
        Task<bool> ToggleUserStatusAsync(string userId);

        /// <summary>
        /// Cambiar rol de usuario
        /// </summary>
        Task<bool> ChangeUserRoleAsync(string userId, RoleType newRole);

        /// <summary>
        /// Buscar usuarios por nombre o email
        /// </summary>
        Task<List<User>> SearchAsync(string query);

        /// <summary>
        /// Obtener estadísticas de usuarios
        /// </summary>
        Task<(int Total, int Clients, int Deliveries, int Admins, int Active, int Inactive)> GetUserStatsAsync();

        /// <summary>
        /// Verificar si existe un email (excepto el userId dado)
        /// </summary>
        Task<bool> EmailExistsAsync(string email, string? excludeUserId = null);
    }
}
