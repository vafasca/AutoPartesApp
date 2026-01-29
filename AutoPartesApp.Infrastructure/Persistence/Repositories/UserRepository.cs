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

        // ========================================
        // MÉTODOS EXISTENTES
        // ========================================

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

        // ========================================
        // 🆕 NUEVOS MÉTODOS IMPLEMENTADOS
        // ========================================

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
    }
}
