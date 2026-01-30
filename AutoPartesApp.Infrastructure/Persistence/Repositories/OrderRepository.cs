using AutoPartesApp.Domain.Entities;
using AutoPartesApp.Domain.Enums;
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

        public async Task<List<Order>> GetOrdersByDateRangeAsync(
            DateTime dateFrom,
            DateTime dateTo,
            OrderStatus? status = null,
            string? userId = null)
        {
            var query = _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Category)
                .Include(o => o.User)
                .Include(o => o.Delivery)
                .Where(o => o.CreatedAt >= dateFrom && o.CreatedAt <= dateTo);

            if (status.HasValue)
            {
                query = query.Where(o => o.Status == status.Value);
            }

            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(o => o.UserId == userId);
            }

            return await query
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<(int TotalOrders, decimal TotalRevenue, decimal AverageOrderValue)> GetOrderStatsByPeriodAsync(
            DateTime dateFrom,
            DateTime dateTo)
        {
            var orders = await _context.Orders
                .Where(o => o.CreatedAt >= dateFrom && o.CreatedAt <= dateTo)
                .ToListAsync();

            if (!orders.Any())
            {
                return (0, 0, 0);
            }

            var totalOrders = orders.Count;
            var totalRevenue = orders.Sum(o => o.Total.Amount);
            var averageOrderValue = totalRevenue / totalOrders;

            return (totalOrders, totalRevenue, averageOrderValue);
        }

        public async Task<List<(string ProductId, string ProductName, string CategoryName, int UnitsSold, decimal Revenue, string ImageUrl)>>
            GetTopSellingProductsAsync(
                DateTime dateFrom,
                DateTime dateTo,
                int topN = 10)
        {
            var result = await _context.Orders
                .Where(o => o.CreatedAt >= dateFrom && o.CreatedAt <= dateTo)
                .SelectMany(o => o.Items)
                .GroupBy(oi => new
                {
                    oi.Product.Id,
                    oi.Product.Name,
                    CategoryName = oi.Product.Category.Name,
                    oi.Product.ImageUrl
                })
                .Select(g => new
                {
                    ProductId = g.Key.Id,
                    ProductName = g.Key.Name,
                    CategoryName = g.Key.CategoryName,
                    UnitsSold = g.Sum(oi => oi.Quantity),
                    Revenue = g.Sum(oi => oi.Quantity * oi.UnitPrice), // <-- Corregido aquí
                    ImageUrl = g.Key.ImageUrl ?? ""
                })
                .OrderByDescending(x => x.UnitsSold)
                .Take(topN)
                .ToListAsync();

            return result.Select(x => (
                x.ProductId,
                x.ProductName,
                x.CategoryName,
                x.UnitsSold,
                x.Revenue,
                x.ImageUrl
            )).ToList();
        }

        public async Task<List<(string CategoryId, string CategoryName, decimal TotalRevenue, int TotalUnits)>>
            GetSalesByCategoryAsync(
                DateTime dateFrom,
                DateTime dateTo)
        {
            var result = await _context.Orders
                .Where(o => o.CreatedAt >= dateFrom && o.CreatedAt <= dateTo)
                .SelectMany(o => o.Items)
                .GroupBy(oi => new
                {
                    CategoryId = oi.Product.CategoryId,
                    CategoryName = oi.Product.Category.Name
                })
                .Select(g => new
                {
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.CategoryName,
                    TotalRevenue = g.Sum(oi => oi.Quantity * oi.UnitPrice), // <-- Corregido aquí
                    TotalUnits = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(x => x.TotalRevenue)
                .ToListAsync();

            return result.Select(x => (
                x.CategoryId,
                x.CategoryName,
                x.TotalRevenue,
                x.TotalUnits
            )).ToList();
        }

        public async Task<List<(DateTime Period, decimal Revenue, int OrderCount)>>
            GetRevenueByPeriodAsync(
                DateTime dateFrom,
                DateTime dateTo,
                string periodType = "day")
        {
            var orders = await _context.Orders
                .Where(o => o.CreatedAt >= dateFrom && o.CreatedAt <= dateTo)
                .Select(o => new { o.CreatedAt, o.Total.Amount })
                .ToListAsync();

            var grouped = periodType.ToLower() switch
            {
                "day" => orders
                    .GroupBy(o => o.CreatedAt.Date)
                    .Select(g => (
                        Period: g.Key,
                        Revenue: g.Sum(x => x.Amount),
                        OrderCount: g.Count()
                    ))
                    .OrderBy(x => x.Period)
                    .ToList(),

                "week" => orders
                    .GroupBy(o => GetWeekStart(o.CreatedAt))
                    .Select(g => (
                        Period: g.Key,
                        Revenue: g.Sum(x => x.Amount),
                        OrderCount: g.Count()
                    ))
                    .OrderBy(x => x.Period)
                    .ToList(),

                "month" => orders
                    .GroupBy(o => new DateTime(o.CreatedAt.Year, o.CreatedAt.Month, 1))
                    .Select(g => (
                        Period: g.Key,
                        Revenue: g.Sum(x => x.Amount),
                        OrderCount: g.Count()
                    ))
                    .OrderBy(x => x.Period)
                    .ToList(),

                "year" => orders
                    .GroupBy(o => new DateTime(o.CreatedAt.Year, 1, 1))
                    .Select(g => (
                        Period: g.Key,
                        Revenue: g.Sum(x => x.Amount),
                        OrderCount: g.Count()
                    ))
                    .OrderBy(x => x.Period)
                    .ToList(),

                _ => orders
                    .GroupBy(o => o.CreatedAt.Date)
                    .Select(g => (
                        Period: g.Key,
                        Revenue: g.Sum(x => x.Amount),
                        OrderCount: g.Count()
                    ))
                    .OrderBy(x => x.Period)
                    .ToList()
            };

            return grouped;
        }

        public async Task<List<(OrderStatus Status, int Count)>> GetOrdersByStatusAsync(
            DateTime? dateFrom = null,
            DateTime? dateTo = null)
        {
            var query = _context.Orders.AsQueryable();

            if (dateFrom.HasValue && dateTo.HasValue)
            {
                query = query.Where(o => o.CreatedAt >= dateFrom.Value && o.CreatedAt <= dateTo.Value);
            }

            var result = await query
                .GroupBy(o => o.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            return result.Select(x => (x.Status, x.Count)).ToList();
        }

        public async Task<decimal> GetAverageOrderValueAsync(
            DateTime dateFrom,
            DateTime dateTo)
        {
            var orders = await _context.Orders
                .Where(o => o.CreatedAt >= dateFrom && o.CreatedAt <= dateTo)
                .ToListAsync();

            if (!orders.Any())
            {
                return 0;
            }

            return orders.Average(o => o.Total.Amount);
        }

        public async Task<decimal> GetCancellationRateAsync(
            DateTime dateFrom,
            DateTime dateTo)
        {
            var totalOrders = await _context.Orders
                .Where(o => o.CreatedAt >= dateFrom && o.CreatedAt <= dateTo)
                .CountAsync();

            if (totalOrders == 0)
            {
                return 0;
            }

            var cancelledOrders = await _context.Orders
                .Where(o => o.CreatedAt >= dateFrom && o.CreatedAt <= dateTo && o.Status == OrderStatus.Cancelled)
                .CountAsync();

            return (decimal)cancelledOrders / totalOrders * 100;
        }

        public async Task<TimeSpan> GetAverageProcessingTimeAsync(
            DateTime dateFrom,
            DateTime dateTo)
        {
            var orders = await _context.Orders
                .Where(o => o.CreatedAt >= dateFrom && o.CreatedAt <= dateTo
                    && o.Status == OrderStatus.Delivered)
                .Select(o => new { o.CreatedAt, o.UpdatedAt })
                .ToListAsync();

            if (!orders.Any())
            {
                return TimeSpan.Zero;
            }

            var totalSeconds = orders.Average(o => (o.UpdatedAt - o.CreatedAt).TotalSeconds);
            return TimeSpan.FromSeconds(totalSeconds);
        }

        public async Task<List<Order>> GetPendingOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.User)
                .Include(o => o.Delivery)
                .Where(o => o.Status == OrderStatus.Pending || o.Status == OrderStatus.Processing)
                .OrderBy(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<(decimal CurrentRevenue, decimal PreviousRevenue, decimal GrowthPercentage)>
            GetPeriodComparisonAsync(
                DateTime currentFrom,
                DateTime currentTo,
                DateTime previousFrom,
                DateTime previousTo)
        {
            var currentOrders = await _context.Orders
                .Where(o => o.CreatedAt >= currentFrom && o.CreatedAt <= currentTo)
                .ToListAsync();

            var previousOrders = await _context.Orders
                .Where(o => o.CreatedAt >= previousFrom && o.CreatedAt <= previousTo)
                .ToListAsync();

            var currentRevenue = currentOrders.Sum(o => o.Total.Amount);
            var previousRevenue = previousOrders.Sum(o => o.Total.Amount);

            var growthPercentage = previousRevenue == 0 ? 0 :
                ((currentRevenue - previousRevenue) / previousRevenue) * 100;

            return (currentRevenue, previousRevenue, growthPercentage);
        }

        // ========== MÉTODOS AUXILIARES ==========

        private DateTime GetWeekStart(DateTime date)
        {
            var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            return date.AddDays(-1 * diff).Date;
        }
    }
}
