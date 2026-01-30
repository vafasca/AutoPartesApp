using AutoPartesApp.Core.Application.DTOs.ReportDTOs;
using AutoPartesApp.Domain.Enums;
using AutoPartesApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoPartesApp.Core.Application.Reports
{
    public class GetOrderReportUseCase
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrderReportUseCase(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<OrderReportDto> ExecuteAsync(ReportFilterDto filter)
        {
            // ✅ NORMALIZACIÓN DE FECHAS A UTC
            var dateTo = NormalizeToUtc(filter.DateTo ?? DateTime.UtcNow);
            var dateFrom = NormalizeToUtc(filter.DateFrom ?? dateTo.AddMonths(-1));

            // Validación de rango
            if (dateFrom > dateTo)
            {
                throw new ArgumentException("La fecha de inicio no puede ser mayor a la fecha de fin");
            }

            var orders = await _orderRepository.GetOrdersByDateRangeAsync(
                dateFrom,
                dateTo,
                null,
                filter.UserId
            );

            var totalOrders = orders.Count;
            var pendingOrders = orders.Count(o =>
                o.Status == OrderStatus.Pending || o.Status == OrderStatus.Confirmed);
            var completedOrders = orders.Count(o => o.Status == OrderStatus.Delivered);
            var cancelledOrders = orders.Count(o => o.Status == OrderStatus.Cancelled);

            // Distribución por estado
            var ordersByStatus = GetOrdersByStatus(orders);

            // Tiempo promedio de procesamiento (de Pending a Delivered)
            var deliveredOrders = orders
                .Where(o => o.Status == OrderStatus.Delivered)
                .ToList();

            var averageProcessingTime = deliveredOrders.Any()
                ? TimeSpan.FromHours(deliveredOrders.Average(o => (o.UpdatedAt - o.CreatedAt).TotalHours))
                : TimeSpan.Zero;

            // Tasa de cancelación
            var cancellationRate = totalOrders > 0
                ? ((decimal)cancelledOrders / totalOrders) * 100
                : 0;

            // Valor promedio de orden
            var averageOrderValue = totalOrders > 0
                ? orders.Average(o => o.Total.Amount)
                : 0;

            return new OrderReportDto
            {
                TotalOrders = totalOrders,
                OrdersByStatus = ordersByStatus,
                AverageProcessingTime = averageProcessingTime,
                CancellationRate = cancellationRate,
                PendingOrders = pendingOrders,
                CompletedOrders = completedOrders,
                AverageOrderValue = averageOrderValue
            };
        }

        /// <summary>
        /// Normaliza DateTime a UTC sin importar su Kind original
        /// </summary>
        private DateTime NormalizeToUtc(DateTime date)
        {
            return date.Kind switch
            {
                DateTimeKind.Utc => date,
                DateTimeKind.Local => date.ToUniversalTime(),
                DateTimeKind.Unspecified => DateTime.SpecifyKind(date, DateTimeKind.Utc),
                _ => DateTime.SpecifyKind(date, DateTimeKind.Utc)
            };
        }

        private List<OrderStatusDto> GetOrdersByStatus(List<Domain.Entities.Order> orders)
        {
            var statusColors = new Dictionary<OrderStatus, string>
            {
                { OrderStatus.Pending, "#f59e0b" },
                { OrderStatus.Confirmed, "#3b82f6" },
                { OrderStatus.Processing, "#8b5cf6" },
                { OrderStatus.Shipped, "#06b6d4" },
                { OrderStatus.OnRoute, "#14b8a6" },
                { OrderStatus.Delivered, "#10b981" },
                { OrderStatus.Cancelled, "#ef4444" }
            };

            var total = orders.Count;
            var statusGroups = orders
                .GroupBy(o => o.Status)
                .Select(g => new OrderStatusDto
                {
                    Status = g.Key,
                    Count = g.Count(),
                    Percentage = total > 0 ? ((decimal)g.Count() / total) * 100 : 0,
                    Color = statusColors.ContainsKey(g.Key)
                        ? statusColors[g.Key]
                        : "#6b7280"
                })
                .OrderByDescending(s => s.Count)
                .ToList();

            return statusGroups;
        }
    }
}
