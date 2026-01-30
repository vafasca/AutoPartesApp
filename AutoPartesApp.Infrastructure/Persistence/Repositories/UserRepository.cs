using AutoPartesApp.Domain.Entities;
using AutoPartesApp.Domain.Enums;
using AutoPartesApp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AutoPartesDbContext _context;

        public UserRepository(AutoPartesDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users
                .Where(u => u.IsActive)
                .OrderBy(u => u.FullName)
                .ToListAsync();
        }

        public async Task<User> CreateAsync(User user)
        {
            user.Id = Guid.NewGuid().ToString();
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(List<User> Users, int TotalCount)> GetPagedAsync(
            string? searchQuery = null,
            RoleType? roleType = null,
            bool? isActive = null,
            DateTime? createdFrom = null,
            DateTime? createdTo = null,
            int pageNumber = 1,
            int pageSize = 20)
        {
            var query = _context.Users.AsQueryable();

            // Filtro por búsqueda (nombre o email)
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                query = query.Where(u =>
                    u.FullName.Contains(searchQuery) ||
                    u.Email.Contains(searchQuery));
            }

            // Filtro por rol
            if (roleType.HasValue)
            {
                query = query.Where(u => u.RoleType == roleType.Value);
            }

            // Filtro por estado activo
            if (isActive.HasValue)
            {
                query = query.Where(u => u.IsActive == isActive.Value);
            }

            // Filtro por rango de fechas
            if (createdFrom.HasValue)
            {
                query = query.Where(u => u.CreatedAt >= createdFrom.Value);
            }

            if (createdTo.HasValue)
            {
                query = query.Where(u => u.CreatedAt <= createdTo.Value);
            }

            // Contar total antes de paginar
            var totalCount = await query.CountAsync();

            // Aplicar paginación
            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (users, totalCount);
        }

        public async Task<List<User>> GetByRoleAsync(RoleType roleType)
        {
            return await _context.Users
                .Where(u => u.RoleType == roleType && u.IsActive)
                .OrderBy(u => u.FullName)
                .ToListAsync();
        }

        public async Task<User?> GetUserWithOrdersAsync(string userId)
        {
            return await _context.Users
                .Include(u => u.Orders.OrderByDescending(o => o.CreatedAt).Take(10))
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User?> GetUserWithDeliveriesAsync(string userId)
        {
            return await _context.Users
                .Include(u => u.Orders)
                    .ThenInclude(o => o.Delivery)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<bool> ToggleUserStatusAsync(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.IsActive = !user.IsActive;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangeUserRoleAsync(string userId, RoleType newRole)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.RoleType = newRole;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<User>> SearchAsync(string query)
        {
            return await _context.Users
                .Where(u => u.IsActive &&
                    (u.FullName.Contains(query) || u.Email.Contains(query)))
                .Take(20)
                .ToListAsync();
        }

        public async Task<(int Total, int Clients, int Deliveries, int Admins, int Active, int Inactive)> GetUserStatsAsync()
        {
            var total = await _context.Users.CountAsync();
            var clients = await _context.Users.CountAsync(u => u.RoleType == RoleType.Client);
            var deliveries = await _context.Users.CountAsync(u => u.RoleType == RoleType.Delivery);
            var admins = await _context.Users.CountAsync(u => u.RoleType == RoleType.Admin);
            var active = await _context.Users.CountAsync(u => u.IsActive);
            var inactive = await _context.Users.CountAsync(u => !u.IsActive);

            return (total, clients, deliveries, admins, active, inactive);
        }

        public async Task<bool> EmailExistsAsync(string email, string? excludeUserId = null)
        {
            var query = _context.Users.Where(u => u.Email == email);

            if (!string.IsNullOrEmpty(excludeUserId))
            {
                query = query.Where(u => u.Id != excludeUserId);
            }

            return await query.AnyAsync();
        }

        public async Task<List<(string UserId, string FullName, string Email, int TotalOrders, decimal TotalSpent, DateTime? LastOrderDate)>>
            GetTopCustomersAsync(
                DateTime? dateFrom = null,
                DateTime? dateTo = null,
                int topN = 10)
        {
            var query = _context.Users
                .Where(u => u.RoleType == RoleType.Client && u.IsActive)
                .Select(u => new
                {
                    UserId = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Orders = dateFrom.HasValue && dateTo.HasValue
                        ? u.Orders.Where(o => o.CreatedAt >= dateFrom.Value && o.CreatedAt <= dateTo.Value)
                        : u.Orders
                });

            var result = await query
                .Select(x => new
                {
                    x.UserId,
                    x.FullName,
                    x.Email,
                    TotalOrders = x.Orders.Count(),
                    TotalSpent = x.Orders.Sum(o => o.Total.Amount),
                    LastOrderDate = x.Orders.Max(o => (DateTime?)o.CreatedAt)
                })
                .Where(x => x.TotalOrders > 0)
                .OrderByDescending(x => x.TotalSpent)
                .Take(topN)
                .ToListAsync();

            return result.Select(x => (
                x.UserId,
                x.FullName,
                x.Email,
                x.TotalOrders,
                x.TotalSpent,
                x.LastOrderDate
            )).ToList();
        }

        public async Task<List<User>> GetInactiveCustomersAsync(int inactiveDays = 90)
        {
            var thresholdDate = DateTime.UtcNow.AddDays(-inactiveDays);

            var activeCustomerIds = await _context.Orders
                .Where(o => o.CreatedAt >= thresholdDate)
                .Select(o => o.UserId)
                .Distinct()
                .ToListAsync();

            return await _context.Users
                .Where(u => u.RoleType == RoleType.Client
                    && u.IsActive
                    && u.CreatedAt < thresholdDate
                    && !activeCustomerIds.Contains(u.Id))
                .ToListAsync();
        }

        public async Task<List<(DateTime Period, int Count)>> GetNewUsersByPeriodAsync(
            DateTime dateFrom,
            DateTime dateTo,
            string periodType = "day")
        {
            var users = await _context.Users
                .Where(u => u.CreatedAt >= dateFrom && u.CreatedAt <= dateTo)
                .Select(u => u.CreatedAt)
                .ToListAsync();

            var grouped = periodType.ToLower() switch
            {
                "day" => users
                    .GroupBy(d => d.Date)
                    .Select(g => (Period: g.Key, Count: g.Count()))
                    .OrderBy(x => x.Period)
                    .ToList(),

                "week" => users
                    .GroupBy(d => GetWeekStart(d))
                    .Select(g => (Period: g.Key, Count: g.Count()))
                    .OrderBy(x => x.Period)
                    .ToList(),

                "month" => users
                    .GroupBy(d => new DateTime(d.Year, d.Month, 1))
                    .Select(g => (Period: g.Key, Count: g.Count()))
                    .OrderBy(x => x.Period)
                    .ToList(),

                _ => users
                    .GroupBy(d => d.Date)
                    .Select(g => (Period: g.Key, Count: g.Count()))
                    .OrderBy(x => x.Period)
                    .ToList()
            };

            return grouped;
        }

        public async Task<List<(string City, string State, string Country, int TotalCustomers)>>
            GetUsersByGeographicDistributionAsync(RoleType? roleType = null)
        {
            var query = _context.Users
                .Where(u => u.IsActive
                    && !string.IsNullOrEmpty(u.AddressCity)
                    && !string.IsNullOrEmpty(u.AddressCountry));

            if (roleType.HasValue)
            {
                query = query.Where(u => u.RoleType == roleType.Value);
            }

            var result = await query
                .GroupBy(u => new
                {
                    City = u.AddressCity ?? "No especificada",
                    State = u.AddressState ?? "No especificado",
                    Country = u.AddressCountry ?? "No especificado"
                })
                .Select(g => new
                {
                    City = g.Key.City,
                    State = g.Key.State,
                    Country = g.Key.Country,
                    TotalCustomers = g.Count()
                })
                .OrderByDescending(x => x.TotalCustomers)
                .ToListAsync();

            return result.Select(x => (
                x.City,
                x.State,
                x.Country,
                x.TotalCustomers
            )).ToList();
        }

        public async Task<decimal> GetRetentionRateAsync(
            DateTime periodStart,
            DateTime periodEnd)
        {
            // Clientes que compraron antes del período
            var existingCustomers = await _context.Orders
                .Where(o => o.CreatedAt < periodStart)
                .Select(o => o.UserId)
                .Distinct()
                .ToListAsync();

            if (!existingCustomers.Any())
            {
                return 0;
            }

            // Clientes existentes que compraron durante el período
            var retainedCustomers = await _context.Orders
                .Where(o => o.CreatedAt >= periodStart
                    && o.CreatedAt <= periodEnd
                    && existingCustomers.Contains(o.UserId))
                .Select(o => o.UserId)
                .Distinct()
                .CountAsync();

            return (decimal)retainedCustomers / existingCustomers.Count * 100;
        }

        public async Task<List<User>> GetFrequentCustomersAsync(
            int minOrders = 5,
            DateTime? dateFrom = null,
            DateTime? dateTo = null)
        {
            var customerOrderCounts = _context.Orders
                .AsQueryable();

            if (dateFrom.HasValue && dateTo.HasValue)
            {
                customerOrderCounts = customerOrderCounts
                    .Where(o => o.CreatedAt >= dateFrom.Value && o.CreatedAt <= dateTo.Value);
            }

            var frequentCustomerIds = await customerOrderCounts
                .GroupBy(o => o.UserId)
                .Where(g => g.Count() >= minOrders)
                .Select(g => g.Key)
                .ToListAsync();

            return await _context.Users
                .Where(u => u.RoleType == RoleType.Client
                    && u.IsActive
                    && frequentCustomerIds.Contains(u.Id))
                .ToListAsync();
        }

        public async Task<(int TotalCustomers, int NewCustomers, int ActiveCustomers, int InactiveCustomers)>
            GetCustomerStatsByPeriodAsync(
                DateTime dateFrom,
                DateTime dateTo)
        {
            var totalCustomers = await _context.Users
                .Where(u => u.RoleType == RoleType.Client && u.IsActive)
                .CountAsync();

            var newCustomers = await _context.Users
                .Where(u => u.RoleType == RoleType.Client
                    && u.IsActive
                    && u.CreatedAt >= dateFrom
                    && u.CreatedAt <= dateTo)
                .CountAsync();

            var activeCustomerIds = await _context.Orders
                .Where(o => o.CreatedAt >= dateFrom && o.CreatedAt <= dateTo)
                .Select(o => o.UserId)
                .Distinct()
                .ToListAsync();

            var activeCustomers = await _context.Users
                .Where(u => u.RoleType == RoleType.Client
                    && u.IsActive
                    && activeCustomerIds.Contains(u.Id))
                .CountAsync();

            var inactiveCustomers = totalCustomers - activeCustomers;

            return (totalCustomers, newCustomers, activeCustomers, inactiveCustomers);
        }

        private DateTime GetWeekStart(DateTime date)
        {
            var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            return date.AddDays(-1 * diff).Date;
        }
    }
}
