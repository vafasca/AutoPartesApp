using AutoPartesApp.Core.Application.DTOs.ReportDTOs;
using AutoPartesApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.Reports
{
    public class GetSalesReportUseCase
    {
        private readonly IOrderRepository _orderRepository;

        public GetSalesReportUseCase(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<SalesReportDto> ExecuteAsync(ReportFilterDto filter)
        {
            // 🆕 VALIDAR Y CONVERTIR FECHAS A UTC
            var dateTo = filter.DateTo ?? DateTime.UtcNow;
            var dateFrom = filter.DateFrom ?? dateTo.AddMonths(-1);

            // Asegurar que las fechas sean UTC
            if (dateTo.Kind != DateTimeKind.Utc)
            {
                dateTo = DateTime.SpecifyKind(dateTo, DateTimeKind.Utc);
            }
            if (dateFrom.Kind != DateTimeKind.Utc)
            {
                dateFrom = DateTime.SpecifyKind(dateFrom, DateTimeKind.Utc);
            }

            var period = filter.Period ?? ReportPeriodType.Month;

            // Obtener órdenes del período actual
            var orders = await _orderRepository.GetOrdersByDateRangeAsync(
                dateFrom,
                dateTo,
                null,
                filter.UserId
            );

            // Calcular período anterior para comparación (growth)
            var periodDays = (dateTo - dateFrom).Days;
            var previousDateTo = dateFrom.AddDays(-1);
            var previousDateFrom = previousDateTo.AddDays(-periodDays);

            var previousOrders = await _orderRepository.GetOrdersByDateRangeAsync(
                previousDateFrom,
                previousDateTo,
                null,
                filter.UserId
            );

            // Calcular métricas principales
            var totalRevenue = orders.Sum(o => o.Total.Amount);
            var totalOrders = orders.Count;
            var averageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;

            var previousRevenue = previousOrders.Sum(o => o.Total.Amount);
            var growthPercentage = previousRevenue > 0
                ? ((totalRevenue - previousRevenue) / previousRevenue) * 100
                : 0;

            // Generar datos para gráfico según período
            var chartData = GenerateChartData(orders, dateFrom, dateTo, period);

            // Obtener top productos
            var topProducts = await GetTopProducts(orders, 5);

            // 🆕 OBTENER VENTAS POR CATEGORÍA
            var salesByCategory = GetSalesByCategory(orders);

            return new SalesReportDto
            {
                TotalRevenue = totalRevenue,
                TotalOrders = totalOrders,
                AverageOrderValue = averageOrderValue,
                Period = new ReportPeriodDto
                {
                    DateFrom = dateFrom,
                    DateTo = dateTo,
                    PeriodType = period,
                    DisplayText = GetPeriodDisplayText(dateFrom, dateTo, period)
                },
                ChartData = chartData,
                TopProducts = topProducts,
                SalesByCategory = salesByCategory, // 🆕 AGREGAR
                GrowthPercentage = growthPercentage,
                PreviousPeriodRevenue = previousRevenue
            };
        }

        // 🆕 MÉTODO PARA CALCULAR VENTAS POR CATEGORÍA
        private List<SalesByCategoryDto> GetSalesByCategory(List<Domain.Entities.Order> orders)
        {
            var totalRevenue = orders.Sum(o => o.Total.Amount);

            var categoryStats = orders
                .SelectMany(o => o.Items)
                .GroupBy(oi => new
                {
                    CategoryId = oi.Product.CategoryId,
                    CategoryName = oi.Product.Category?.Name ?? "Sin categoría"
                })
                .Select(g => new SalesByCategoryDto
                {
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.CategoryName,
                    TotalRevenue = g.Sum(oi => oi.Subtotal),
                    TotalUnits = g.Sum(oi => oi.Quantity),
                    Percentage = 0, // Se calcula después
                    Color = GetCategoryColor(g.Key.CategoryName)
                })
                .OrderByDescending(c => c.TotalRevenue)
                .ToList();

            // Calcular porcentajes
            foreach (var category in categoryStats)
            {
                category.Percentage = totalRevenue > 0
                    ? (int)Math.Round((category.TotalRevenue / totalRevenue) * 100)
                    : 0;
            }

            return categoryStats;
        }

        private string GetCategoryColor(string categoryName)
        {
            // Asignar colores según la categoría
            var colorMap = new Dictionary<string, string>
            {
                { "Motor", "bg-primary" },
                { "Frenos", "bg-red-500" },
                { "Suspensión", "bg-orange-500" },
                { "Eléctrico", "bg-yellow-500" },
                { "Transmisión", "bg-green-500" },
                { "Carrocería", "bg-blue-500" },
                { "Luces", "bg-indigo-500" },
                { "Filtros", "bg-purple-500" },
                { "Aceites", "bg-pink-500" }
            };

            return colorMap.TryGetValue(categoryName, out var color)
                ? color
                : "bg-slate-500";
        }

        private List<ChartDataDto> GenerateChartData(
            List<Domain.Entities.Order> orders,
            DateTime dateFrom,
            DateTime dateTo,
            ReportPeriodType periodType)
        {
            var chartData = new List<ChartDataDto>();

            switch (periodType)
            {
                case ReportPeriodType.Day:
                    // Agrupar por día
                    var dayGroups = orders
                        .GroupBy(o => o.CreatedAt.Date)
                        .OrderBy(g => g.Key);

                    foreach (var group in dayGroups)
                    {
                        chartData.Add(new ChartDataDto
                        {
                            Label = group.Key.ToString("dd/MM"),
                            Value = group.Sum(o => o.Total.Amount),
                            Date = group.Key
                        });
                    }
                    break;

                case ReportPeriodType.Week:
                    // Agrupar por semana
                    var weekGroups = orders
                        .GroupBy(o => GetWeekNumber(o.CreatedAt))
                        .OrderBy(g => g.Key);

                    foreach (var group in weekGroups)
                    {
                        var firstDate = group.Min(o => o.CreatedAt);
                        chartData.Add(new ChartDataDto
                        {
                            Label = $"Sem {group.Key}",
                            Value = group.Sum(o => o.Total.Amount),
                            Date = firstDate
                        });
                    }
                    break;

                case ReportPeriodType.Month:
                default:
                    // Agrupar por mes
                    var monthGroups = orders
                        .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
                        .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month);

                    foreach (var group in monthGroups)
                    {
                        var monthDate = new DateTime(group.Key.Year, group.Key.Month, 1);
                        chartData.Add(new ChartDataDto
                        {
                            Label = monthDate.ToString("MMM yyyy"),
                            Value = group.Sum(o => o.Total.Amount),
                            Date = monthDate
                        });
                    }
                    break;
            }

            return chartData;
        }

        private async Task<List<TopProductDto>> GetTopProducts(List<Domain.Entities.Order> orders, int topN)
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

            return await Task.FromResult(productStats);
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

        private string GetPeriodDisplayText(
            DateTime dateFrom,
            DateTime dateTo,
            ReportPeriodType periodType)
        {
            return periodType switch
            {
                ReportPeriodType.Day => $"{dateFrom:dd/MM/yyyy} - {dateTo:dd/MM/yyyy}",
                ReportPeriodType.Week => $"Semana del {dateFrom:dd/MM} al {dateTo:dd/MM}",
                ReportPeriodType.Month => $"{dateFrom:MMMM yyyy}",
                ReportPeriodType.Year => $"{dateFrom.Year}",
                _ => $"{dateFrom:dd/MM/yyyy} - {dateTo:dd/MM/yyyy}"
            };
        }
    }
}
