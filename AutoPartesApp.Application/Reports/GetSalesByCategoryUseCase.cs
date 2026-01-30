using AutoPartesApp.Core.Application.DTOs.ReportDTOs;
using AutoPartesApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.Reports
{
    public class GetSalesByCategoryUseCase
    {
        private readonly IOrderRepository _orderRepository;

        public GetSalesByCategoryUseCase(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<List<SalesByCategoryDto>> ExecuteAsync(ReportFilterDto filter)
        {
            var dateTo = filter.DateTo ?? DateTime.UtcNow;
            var dateFrom = filter.DateFrom ?? dateTo.AddMonths(-1);

            var orders = await _orderRepository.GetOrdersByDateRangeAsync(
                dateFrom,
                dateTo,
                null,
                filter.UserId
            );

            // Agrupar por categoría
            var categoryStats = orders
                .SelectMany(o => o.Items)
                .GroupBy(oi => new
                {
                    CategoryId = oi.Product.CategoryId,
                    CategoryName = oi.Product.Category?.Name ?? "Sin categoría"
                })
                .Select(g => new
                {
                    g.Key.CategoryId,
                    g.Key.CategoryName,
                    TotalRevenue = g.Sum(oi => oi.Subtotal),
                    TotalUnits = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(c => c.TotalRevenue)
                .ToList();

            var totalRevenue = categoryStats.Sum(c => c.TotalRevenue);

            // Colores predefinidos
            var colors = new[] { "#3b82f6", "#8b5cf6", "#ec4899", "#f59e0b", "#10b981", "#6366f1" };

            var result = categoryStats.Select((stat, index) => new SalesByCategoryDto
            {
                CategoryId = stat.CategoryId,
                CategoryName = stat.CategoryName,
                TotalRevenue = stat.TotalRevenue,
                TotalUnits = stat.TotalUnits,
                Percentage = totalRevenue > 0 ? (stat.TotalRevenue / totalRevenue) * 100 : 0,
                Color = colors[index % colors.Length]
            }).ToList();

            return result;
        }
    }
}
