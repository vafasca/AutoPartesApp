using AutoPartesApp.Core.Application.DTOs.ReportDTOs;
using AutoPartesApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.Reports
{
    public class GetFinancialReportUseCase
    {
        private readonly IOrderRepository _orderRepository;

        public GetFinancialReportUseCase(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<FinancialReportDto> ExecuteAsync(ReportFilterDto filter)
        {
            var dateTo = filter.DateTo ?? DateTime.UtcNow;
            var dateFrom = filter.DateFrom ?? dateTo.AddMonths(-6); // Últimos 6 meses

            var period = filter.Period ?? ReportPeriodType.Month;

            var orders = await _orderRepository.GetOrdersByDateRangeAsync(
                dateFrom,
                dateTo
            );

            var totalRevenue = orders.Sum(o => o.Total.Amount);
            var averageTicket = orders.Any()
                ? orders.Average(o => o.Total.Amount)
                : 0;

            // Ingresos por período (mensual)
            var revenueByPeriod = GetRevenueByPeriod(orders, period);

            // Top productos por ingresos
            var topRevenueProducts = GetTopRevenueProducts(orders, 10);

            // Calcular tasa de crecimiento
            var halfPoint = dateFrom.AddDays((dateTo - dateFrom).TotalDays / 2);
            var firstHalfRevenue = orders
                .Where(o => o.CreatedAt < halfPoint)
                .Sum(o => o.Total.Amount);
            var secondHalfRevenue = orders
                .Where(o => o.CreatedAt >= halfPoint)
                .Sum(o => o.Total.Amount);

            var growthRate = firstHalfRevenue > 0
                ? ((secondHalfRevenue - firstHalfRevenue) / firstHalfRevenue) * 100
                : 0;

            // Proyección simple basada en tendencia
            var projectedRevenue = CalculateProjectedRevenue(
                revenueByPeriod,
                growthRate
            );

            // Costos estimados (70% del revenue como ejemplo)
            var totalCosts = totalRevenue * 0.70m;
            var netProfit = totalRevenue - totalCosts;

            return new FinancialReportDto
            {
                TotalRevenue = totalRevenue,
                AverageTicket = averageTicket,
                RevenueByPeriod = revenueByPeriod,
                TopRevenueProducts = topRevenueProducts,
                ProjectedRevenue = projectedRevenue,
                GrowthRate = growthRate,
                TotalCosts = totalCosts,
                NetProfit = netProfit
            };
        }

        private List<PeriodRevenueDto> GetRevenueByPeriod(
            List<Domain.Entities.Order> orders,
            ReportPeriodType periodType)
        {
            var revenueData = new List<PeriodRevenueDto>();

            switch (periodType)
            {
                case ReportPeriodType.Month:
                default:
                    var monthGroups = orders
                        .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
                        .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month);

                    foreach (var group in monthGroups)
                    {
                        var monthDate = new DateTime(group.Key.Year, group.Key.Month, 1);
                        var lastDay = DateTime.DaysInMonth(group.Key.Year, group.Key.Month);
                        var monthEnd = new DateTime(group.Key.Year, group.Key.Month, lastDay);

                        revenueData.Add(new PeriodRevenueDto
                        {
                            Period = monthDate.ToString("MMMM yyyy"),
                            Revenue = group.Sum(o => o.Total.Amount),
                            OrderCount = group.Count(),
                            PeriodStart = monthDate,
                            PeriodEnd = monthEnd
                        });
                    }
                    break;

                case ReportPeriodType.Week:
                    var weekGroups = orders
                        .GroupBy(o => GetWeekNumber(o.CreatedAt))
                        .OrderBy(g => g.Key);

                    foreach (var group in weekGroups)
                    {
                        var firstDate = group.Min(o => o.CreatedAt);
                        var lastDate = group.Max(o => o.CreatedAt);

                        revenueData.Add(new PeriodRevenueDto
                        {
                            Period = $"Semana {group.Key}",
                            Revenue = group.Sum(o => o.Total.Amount),
                            OrderCount = group.Count(),
                            PeriodStart = firstDate.Date,
                            PeriodEnd = lastDate.Date
                        });
                    }
                    break;
            }

            return revenueData;
        }

        private List<TopProductDto> GetTopRevenueProducts(
            List<Domain.Entities.Order> orders,
            int topN)
        {
            var productStats = orders
                .SelectMany(o => o.Items)
                .GroupBy(oi => new
                {
                    oi.Product.Id,
                    oi.Product.Name,
                    CategoryName = oi.Product.Category?.Name ?? "Sin categoría",
                    oi.Product.ImageUrl
                })
                .Select(g => new TopProductDto
                {
                    ProductId = g.Key.Id,
                    Name = g.Key.Name,
                    CategoryName = g.Key.CategoryName,
                    UnitsSold = g.Sum(oi => oi.Quantity),
                    Revenue = g.Sum(oi => oi.Subtotal),
                    ImageUrl = g.Key.ImageUrl,
                    Trend = "stable",
                    TrendPercentage = 0
                })
                .OrderByDescending(p => p.Revenue)
                .Take(topN)
                .ToList();

            return productStats;
        }

        private decimal CalculateProjectedRevenue(
            List<PeriodRevenueDto> revenueByPeriod,
            decimal growthRate)
        {
            if (!revenueByPeriod.Any())
                return 0;

            var lastPeriodRevenue = revenueByPeriod.Last().Revenue;
            var projection = lastPeriodRevenue * (1 + (growthRate / 100));

            return projection;
        }

        private int GetWeekNumber(DateTime date)
        {
            var culture = System.Globalization.CultureInfo.CurrentCulture;
            var calendar = culture.Calendar;
            return calendar.GetWeekOfYear(
                date,
                culture.DateTimeFormat.CalendarWeekRule,
                culture.DateTimeFormat.FirstDayOfWeek
            );
        }
    }
}
