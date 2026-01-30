using AutoPartesApp.Core.Application.DTOs.ReportDTOs;
using AutoPartesApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.Reports
{
    public class GetTopSellingProductsUseCase
    {
        private readonly IOrderRepository _orderRepository;

        public GetTopSellingProductsUseCase(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<List<TopProductDto>> ExecuteAsync(ReportFilterDto filter, int topN = 10)
        {
            var dateTo = filter.DateTo ?? DateTime.UtcNow;
            var dateFrom = filter.DateFrom ?? dateTo.AddMonths(-1);

            // Período actual
            var orders = await _orderRepository.GetOrdersByDateRangeAsync(
                dateFrom,
                dateTo,
                null,
                filter.UserId
            );

            // Período anterior para calcular trending
            var periodDays = (dateTo - dateFrom).Days;
            var previousDateTo = dateFrom.AddDays(-1);
            var previousDateFrom = previousDateTo.AddDays(-periodDays);

            var previousOrders = await _orderRepository.GetOrdersByDateRangeAsync(
                previousDateFrom,
                previousDateTo,
                null,
                filter.UserId
            );

            // Estadísticas período actual
            var currentStats = orders
                .SelectMany(o => o.Items)
                .GroupBy(oi => oi.Product.Id)
                .Select(g => new
                {
                    ProductId = g.Key,
                    UnitsSold = g.Sum(oi => oi.Quantity),
                    Revenue = g.Sum(oi => oi.Subtotal)
                })
                .ToDictionary(x => x.ProductId);

            // Estadísticas período anterior
            var previousStats = previousOrders
                .SelectMany(o => o.Items)
                .GroupBy(oi => oi.Product.Id)
                .Select(g => new
                {
                    ProductId = g.Key,
                    UnitsSold = g.Sum(oi => oi.Quantity)
                })
                .ToDictionary(x => x.ProductId, x => x.UnitsSold);

            // Construir resultado con trending
            var topProducts = orders
                .SelectMany(o => o.Items)
                .GroupBy(oi => new
                {
                    oi.Product.Id,
                    oi.Product.Name,
                    CategoryName = oi.Product.Category?.Name ?? "Sin categoría",
                    oi.Product.ImageUrl
                })
                .Select(g =>
                {
                    var productId = g.Key.Id;
                    var currentUnits = currentStats.ContainsKey(productId)
                        ? currentStats[productId].UnitsSold
                        : 0;
                    var previousUnits = previousStats.ContainsKey(productId)
                        ? previousStats[productId]
                        : 0;

                    var trendPercentage = previousUnits > 0
                        ? ((decimal)(currentUnits - previousUnits) / previousUnits) * 100
                        : 0;

                    return new TopProductDto
                    {
                        ProductId = g.Key.Id,
                        Name = g.Key.Name,
                        CategoryName = g.Key.CategoryName,
                        UnitsSold = g.Sum(oi => oi.Quantity),
                        Revenue = g.Sum(oi => oi.Subtotal),
                        ImageUrl = g.Key.ImageUrl,
                        Trend = trendPercentage > 0 ? "up" : trendPercentage < 0 ? "down" : "stable",
                        TrendPercentage = Math.Abs(trendPercentage)
                    };
                })
                .OrderByDescending(p => p.Revenue)
                .Take(topN)
                .ToList();

            return topProducts;
        }
    }
}
