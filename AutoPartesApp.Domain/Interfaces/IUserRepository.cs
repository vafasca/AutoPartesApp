using AutoPartesApp.Domain.Entities;
using AutoPartesApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(string id);
        Task<User?> GetByEmailAsync(string email);
        Task<List<User>> GetAllAsync();
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(User user);
        Task<bool> DeleteAsync(string id);

        Task<(List<User> Users, int TotalCount)> GetPagedAsync(
            string? searchQuery = null,
            RoleType? roleType = null,
            bool? isActive = null,
            DateTime? createdFrom = null,
            DateTime? createdTo = null,
            int pageNumber = 1,
            int pageSize = 20
        );

        Task<List<User>> GetByRoleAsync(RoleType roleType);
        Task<User?> GetUserWithOrdersAsync(string userId);
        Task<User?> GetUserWithDeliveriesAsync(string userId);
        Task<bool> ToggleUserStatusAsync(string userId);
        Task<bool> ChangeUserRoleAsync(string userId, RoleType newRole);
        Task<List<User>> SearchAsync(string query);
        Task<(int Total, int Clients, int Deliveries, int Admins, int Active, int Inactive)> GetUserStatsAsync();
        Task<bool> EmailExistsAsync(string email, string? excludeUserId = null);

        /// <summary>
        /// Obtener top clientes por total gastado
        /// </summary>
        Task<List<(string UserId, string FullName, string Email, int TotalOrders, decimal TotalSpent, DateTime? LastOrderDate)>>
            GetTopCustomersAsync(
                DateTime? dateFrom = null,
                DateTime? dateTo = null,
                int topN = 10);

        /// <summary>
        /// Obtener clientes inactivos (sin órdenes en el período)
        /// </summary>
        Task<List<User>> GetInactiveCustomersAsync(
            int inactiveDays = 90);

        /// <summary>
        /// Obtener nuevos usuarios registrados por período
        /// </summary>
        Task<List<(DateTime Period, int Count)>> GetNewUsersByPeriodAsync(
            DateTime dateFrom,
            DateTime dateTo,
            string periodType = "day"); // "day", "week", "month"

        /// <summary>
        /// Obtener distribución geográfica de clientes
        /// </summary>
        Task<List<(string City, string State, string Country, int TotalCustomers)>>
            GetUsersByGeographicDistributionAsync(RoleType? roleType = null);

        /// <summary>
        /// Obtener tasa de retención de clientes
        /// </summary>
        Task<decimal> GetRetentionRateAsync(
            DateTime periodStart,
            DateTime periodEnd);

        /// <summary>
        /// Obtener clientes con más de N órdenes
        /// </summary>
        Task<List<User>> GetFrequentCustomersAsync(
            int minOrders = 5,
            DateTime? dateFrom = null,
            DateTime? dateTo = null);

        /// <summary>
        /// Obtener estadísticas de clientes por período
        /// </summary>
        Task<(int TotalCustomers, int NewCustomers, int ActiveCustomers, int InactiveCustomers)>
            GetCustomerStatsByPeriodAsync(
                DateTime dateFrom,
                DateTime dateTo);
    }
}
