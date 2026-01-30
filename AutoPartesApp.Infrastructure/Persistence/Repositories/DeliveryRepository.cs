using AutoPartesApp.Domain.Entities;
using AutoPartesApp.Domain.Enums;
using AutoPartesApp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Infrastructure.Persistence.Repositories
{
    public class DeliveryRepository : IDeliveryRepository
    {
        private readonly AutoPartesDbContext _context;

        public DeliveryRepository(AutoPartesDbContext context)
        {
            _context = context;
        }

        public async Task<Delivery?> GetByIdAsync(string id)
        {
            return await _context.Deliveries
                .Include(d => d.Order)
                    .ThenInclude(o => o.User)
                .Include(d => d.Driver)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<List<Delivery>> GetAllAsync()
        {
            return await _context.Deliveries
                .Include(d => d.Order)
                .Include(d => d.Driver)
                .OrderByDescending(d => d.AssignedAt)
                .ToListAsync();
        }

        public async Task<List<Delivery>> GetByDriverIdAsync(string driverId)
        {
            return await _context.Deliveries
                .Include(d => d.Order)
                    .ThenInclude(o => o.User)
                .Where(d => d.DriverId == driverId)
                .OrderByDescending(d => d.AssignedAt)
                .ToListAsync();
        }

        public async Task<Delivery?> GetByOrderIdAsync(string orderId)
        {
            return await _context.Deliveries
                .Include(d => d.Order)
                .Include(d => d.Driver)
                .FirstOrDefaultAsync(d => d.OrderId == orderId);
        }

        public async Task<Delivery> CreateAsync(Delivery delivery)
        {
            _context.Deliveries.Add(delivery);
            await _context.SaveChangesAsync();
            return delivery;
        }

        public async Task<Delivery> UpdateAsync(Delivery delivery)
        {
            _context.Deliveries.Update(delivery);
            await _context.SaveChangesAsync();
            return delivery;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var delivery = await _context.Deliveries.FindAsync(id);
            if (delivery == null) return false;

            _context.Deliveries.Remove(delivery);
            await _context.SaveChangesAsync();
            return true;
        }

        // ========== MÉTODOS PARA REPORTES DE ENTREGAS ==========

        public async Task<List<Delivery>> GetDeliveriesByDateRangeAsync(
            DateTime dateFrom,
            DateTime dateTo,
            DeliveryStatus? status = null)
        {
            var query = _context.Deliveries
                .Include(d => d.Order)
                    .ThenInclude(o => o.User)
                .Include(d => d.Driver)
                .Where(d => d.AssignedAt.HasValue
                    && d.AssignedAt.Value >= dateFrom
                    && d.AssignedAt.Value <= dateTo);

            if (status.HasValue)
            {
                query = query.Where(d => d.Status == status.Value);
            }

            return await query
                .OrderByDescending(d => d.AssignedAt)
                .ToListAsync();
        }

        public async Task<List<(string DriverId, string DriverName, int TotalDeliveries, int CompletedDeliveries, TimeSpan AverageTime, decimal EfficiencyRate)>>
            GetDeliveryStatsByDriverAsync(
                DateTime dateFrom,
                DateTime dateTo)
        {
            var deliveries = await _context.Deliveries
                .Include(d => d.Driver)
                .Where(d => d.AssignedAt.HasValue
                    && d.AssignedAt.Value >= dateFrom
                    && d.AssignedAt.Value <= dateTo
                    && d.DriverId != null)
                .ToListAsync();

            var result = deliveries
                .GroupBy(d => new
                {
                    DriverId = d.DriverId!,
                    DriverName = d.Driver.FullName
                })
                .Select(g =>
                {
                    var totalDeliveries = g.Count();
                    var completedDeliveries = g.Count(d => d.Status == DeliveryStatus.Delivered);
                    var deliveriesWithTime = g.Where(d => d.AssignedAt.HasValue && d.DeliveredAt.HasValue).ToList();

                    var averageTime = deliveriesWithTime.Any()
                        ? TimeSpan.FromSeconds(deliveriesWithTime.Average(d =>
                            (d.DeliveredAt!.Value - d.AssignedAt!.Value).TotalSeconds))
                        : TimeSpan.Zero;

                    var efficiencyRate = totalDeliveries > 0
                        ? (decimal)completedDeliveries / totalDeliveries * 100
                        : 0;

                    return (
                        DriverId: g.Key.DriverId,
                        DriverName: g.Key.DriverName,
                        TotalDeliveries: totalDeliveries,
                        CompletedDeliveries: completedDeliveries,
                        AverageTime: averageTime,
                        EfficiencyRate: efficiencyRate
                    );
                })
                .OrderByDescending(x => x.EfficiencyRate)
                .ToList();

            return result;
        }

        public async Task<TimeSpan> GetAverageDeliveryTimeAsync(
            DateTime dateFrom,
            DateTime dateTo,
            string? driverId = null)
        {
            var query = _context.Deliveries
                .Where(d => d.AssignedAt.HasValue
                    && d.DeliveredAt.HasValue
                    && d.AssignedAt.Value >= dateFrom
                    && d.AssignedAt.Value <= dateTo
                    && d.Status == DeliveryStatus.Delivered);

            if (!string.IsNullOrEmpty(driverId))
            {
                query = query.Where(d => d.DriverId == driverId);
            }

            var deliveries = await query.ToListAsync();

            if (!deliveries.Any())
            {
                return TimeSpan.Zero;
            }

            var averageSeconds = deliveries.Average(d =>
                (d.DeliveredAt!.Value - d.AssignedAt!.Value).TotalSeconds);

            return TimeSpan.FromSeconds(averageSeconds);
        }

        public async Task<decimal> GetDeliveryEfficiencyAsync(
            DateTime dateFrom,
            DateTime dateTo)
        {
            var totalDeliveries = await _context.Deliveries
                .Where(d => d.AssignedAt.HasValue
                    && d.AssignedAt.Value >= dateFrom
                    && d.AssignedAt.Value <= dateTo)
                .CountAsync();

            if (totalDeliveries == 0)
            {
                return 0;
            }

            var completedDeliveries = await _context.Deliveries
                .Where(d => d.AssignedAt.HasValue
                    && d.AssignedAt.Value >= dateFrom
                    && d.AssignedAt.Value <= dateTo
                    && d.Status == DeliveryStatus.Delivered)
                .CountAsync();

            return (decimal)completedDeliveries / totalDeliveries * 100;
        }

        public async Task<List<(string City, string State, int TotalDeliveries, TimeSpan AverageTime)>>
            GetDeliveriesByZoneAsync(
                DateTime dateFrom,
                DateTime dateTo)
        {
            var deliveries = await _context.Deliveries
                .Include(d => d.Order)
                .Where(d => d.AssignedAt.HasValue
                    && d.AssignedAt.Value >= dateFrom
                    && d.AssignedAt.Value <= dateTo)
                .ToListAsync();

            var result = deliveries
                .GroupBy(d => new
                {
                    City = d.Order.DeliveryAddress.City ?? "No especificada",
                    State = d.Order.DeliveryAddress.State ?? "No especificado"
                })
                .Select(g =>
                {
                    var deliveriesWithTime = g.Where(d => d.AssignedAt.HasValue && d.DeliveredAt.HasValue).ToList();

                    var averageTime = deliveriesWithTime.Any()
                        ? TimeSpan.FromSeconds(deliveriesWithTime.Average(d =>
                            (d.DeliveredAt!.Value - d.AssignedAt!.Value).TotalSeconds))
                        : TimeSpan.Zero;

                    return (
                        City: g.Key.City,
                        State: g.Key.State,
                        TotalDeliveries: g.Count(),
                        AverageTime: averageTime
                    );
                })
                .OrderByDescending(x => x.TotalDeliveries)
                .ToList();

            return result;
        }

        public async Task<List<(DeliveryStatus Status, int Count)>> GetDeliveriesByStatusAsync(
            DateTime? dateFrom = null,
            DateTime? dateTo = null)
        {
            var query = _context.Deliveries.AsQueryable();

            if (dateFrom.HasValue && dateTo.HasValue)
            {
                query = query.Where(d => d.AssignedAt.HasValue
                    && d.AssignedAt.Value >= dateFrom.Value
                    && d.AssignedAt.Value <= dateTo.Value);
            }

            var result = await query
                .GroupBy(d => d.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            return result.Select(x => (x.Status, x.Count)).ToList();
        }

        public async Task<List<Delivery>> GetPendingDeliveriesAsync()
        {
            return await _context.Deliveries
                .Include(d => d.Order)
                    .ThenInclude(o => o.User)
                .Include(d => d.Driver)
                .Where(d => d.Status == DeliveryStatus.Pending || d.Status == DeliveryStatus.OnRoute)
                .OrderBy(d => d.AssignedAt)
                .ToListAsync();
        }

        public async Task<List<Delivery>> GetDelayedDeliveriesAsync()
        {
            var now = DateTime.UtcNow;

            return await _context.Deliveries
                .Include(d => d.Order)
                    .ThenInclude(o => o.User)
                .Include(d => d.Driver)
                .Where(d => d.Status == DeliveryStatus.OnRoute
                    && d.EstimatedArrival.HasValue
                    && d.EstimatedArrival.Value < now)
                .OrderBy(d => d.EstimatedArrival)
                .ToListAsync();
        }

        public async Task<List<(string DriverId, string DriverName, int CompletedDeliveries, decimal EfficiencyRate, TimeSpan AverageTime)>>
            GetTopDriversAsync(
                DateTime dateFrom,
                DateTime dateTo,
                int topN = 10)
        {
            var deliveries = await _context.Deliveries
                .Include(d => d.Driver)
                .Where(d => d.AssignedAt.HasValue
                    && d.AssignedAt.Value >= dateFrom
                    && d.AssignedAt.Value <= dateTo
                    && d.DriverId != null)
                .ToListAsync();

            var result = deliveries
                .GroupBy(d => new
                {
                    DriverId = d.DriverId!,
                    DriverName = d.Driver.FullName
                })
                .Select(g =>
                {
                    var totalDeliveries = g.Count();
                    var completedDeliveries = g.Count(d => d.Status == DeliveryStatus.Delivered);
                    var deliveriesWithTime = g.Where(d => d.AssignedAt.HasValue && d.DeliveredAt.HasValue).ToList();

                    var averageTime = deliveriesWithTime.Any()
                        ? TimeSpan.FromSeconds(deliveriesWithTime.Average(d =>
                            (d.DeliveredAt!.Value - d.AssignedAt!.Value).TotalSeconds))
                        : TimeSpan.Zero;

                    var efficiencyRate = totalDeliveries > 0
                        ? (decimal)completedDeliveries / totalDeliveries * 100
                        : 0;

                    return (
                        DriverId: g.Key.DriverId,
                        DriverName: g.Key.DriverName,
                        CompletedDeliveries: completedDeliveries,
                        EfficiencyRate: efficiencyRate,
                        AverageTime: averageTime
                    );
                })
                .OrderByDescending(x => x.EfficiencyRate)
                .ThenByDescending(x => x.CompletedDeliveries)
                .Take(topN)
                .ToList();

            return result;
        }

        public async Task<(int TotalDeliveries, int Completed, int Pending, int InProgress, int Cancelled, decimal SuccessRate)>
            GetDeliveryStatsAsync(
                DateTime dateFrom,
                DateTime dateTo)
        {
            var deliveries = await _context.Deliveries
                .Where(d => d.AssignedAt.HasValue
                    && d.AssignedAt.Value >= dateFrom
                    && d.AssignedAt.Value <= dateTo)
                .ToListAsync();

            var totalDeliveries = deliveries.Count;
            var completed = deliveries.Count(d => d.Status == DeliveryStatus.Delivered);
            var pending = deliveries.Count(d => d.Status == DeliveryStatus.Pending);
            var inProgress = deliveries.Count(d => d.Status == DeliveryStatus.OnRoute);
            var cancelled = deliveries.Count(d => d.Status == DeliveryStatus.Failed);

            var successRate = totalDeliveries > 0
                ? (decimal)completed / totalDeliveries * 100
                : 0;

            return (totalDeliveries, completed, pending, inProgress, cancelled, successRate);
        }

        public async Task<List<Delivery>> GetDeliveriesWithOrdersAsync(
            DateTime dateFrom,
            DateTime dateTo,
            string? driverId = null)
        {
            var query = _context.Deliveries
                .Include(d => d.Order)
                    .ThenInclude(o => o.Items)
                        .ThenInclude(oi => oi.Product)
                .Include(d => d.Order)
                    .ThenInclude(o => o.User)
                .Include(d => d.Driver)
                .Where(d => d.AssignedAt.HasValue
                    && d.AssignedAt.Value >= dateFrom
                    && d.AssignedAt.Value <= dateTo);

            if (!string.IsNullOrEmpty(driverId))
            {
                query = query.Where(d => d.DriverId == driverId);
            }

            return await query
                .OrderByDescending(d => d.AssignedAt)
                .ToListAsync();
        }
    }
}
