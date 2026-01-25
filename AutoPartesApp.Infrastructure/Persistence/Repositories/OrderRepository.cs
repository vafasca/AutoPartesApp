using AutoPartesApp.Domain.Entities;
using AutoPartesApp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Infrastructure.Persistence.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AutoPartesDbContext _context;

        public OrderRepository(AutoPartesDbContext context)
        {
            _context = context;
        }

        public async Task<Order?> GetByIdAsync(string id)
        {
            return await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.User)
                .Include(o => o.Delivery)
                .FirstOrDefaultAsync(o => o.Id == id);

        }

        public async Task<List<Order>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.Items)
                .Include(o => o.User)
                .Include(o => o.Delivery)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Order>> GetByUserIdAsync(string userId)
        {
            return await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Delivery)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

        }

        public async Task<Order> CreateAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order> UpdateAsync(Order order)
        {
            order.UpdatedAt = DateTime.UtcNow;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return false;

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
