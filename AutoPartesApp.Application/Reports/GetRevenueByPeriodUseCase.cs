using AutoPartesApp.Core.Application.DTOs.ReportDTOs;
using AutoPartesApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.Reports
{
    public class GetRevenueByPeriodUseCase
    {
        private readonly IOrderRepository _orderRepository;

        public GetRevenueByPeriodUseCase(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<List<PeriodRevenueDto>> ExecuteAsync(ReportFilterDto filter)
        {
            var dateTo = filter.DateTo ?? DateTime.UtcNow;
            var dateFrom = filter.DateFrom ?? dateTo.AddMonths(-6);

            var orders = await _orderRepository.GetOrdersByDateRangeAsync(
                dateFrom,
                dateTo
            );

            var revenueData = new List<PeriodRevenueDto>();

            switch (filter.Period)
            {
                case ReportPeriodType.Day:
                    revenueData = GetRevenueByDay(orders);
                    break;

                case ReportPeriodType.Week:
                    revenueData = GetRevenueByWeek(orders);
                    break;

                case ReportPeriodType.Month:
                    revenueData = GetRevenueByMonth(orders);
                    break;

                case ReportPeriodType.Quarter:
                    revenueData = GetRevenueByQuarter(orders);
                    break;

                case ReportPeriodType.Year:
                    revenueData = GetRevenueByYear(orders);
                    break;

                default:
                    revenueData = GetRevenueByMonth(orders);
                    break;
            }

            return revenueData;
        }

        private List<PeriodRevenueDto> GetRevenueByDay(List<Domain.Entities.Order> orders)
        {
            var dayGroups = orders
                .GroupBy(o => o.CreatedAt.Date)
                .OrderBy(g => g.Key);

            return dayGroups.Select(group => new PeriodRevenueDto
            {
                Period = group.Key.ToString("dd/MM/yyyy"),
                Revenue = group.Sum(o => o.Total.Amount),
                OrderCount = group.Count(),
                PeriodStart = group.Key,
                PeriodEnd = group.Key.AddDays(1).AddTicks(-1)
            }).ToList();
        }

        private List<PeriodRevenueDto> GetRevenueByWeek(List<Domain.Entities.Order> orders)
        {
            var weekGroups = orders
                .GroupBy(o => GetWeekNumber(o.CreatedAt))
                .OrderBy(g => g.Key);

            return weekGroups.Select(group =>
            {
                var firstDate = group.Min(o => o.CreatedAt);
                var lastDate = group.Max(o => o.CreatedAt);

                return new PeriodRevenueDto
                {
                    Period = $"Semana {group.Key} - {firstDate.Year}",
                    Revenue = group.Sum(o => o.Total.Amount),
                    OrderCount = group.Count(),
                    PeriodStart = firstDate.Date,
                    PeriodEnd = lastDate.Date
                };
            }).ToList();
        }

        private List<PeriodRevenueDto> GetRevenueByMonth(List<Domain.Entities.Order> orders)
        {
            var monthGroups = orders
                .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month);

            return monthGroups.Select(group =>
            {
                var monthDate = new DateTime(group.Key.Year, group.Key.Month, 1);
                var lastDay = DateTime.DaysInMonth(group.Key.Year, group.Key.Month);
                var monthEnd = new DateTime(group.Key.Year, group.Key.Month, lastDay, 23, 59, 59);

                return new PeriodRevenueDto
                {
                    Period = monthDate.ToString("MMMM yyyy"),
                    Revenue = group.Sum(o => o.Total.Amount),
                    OrderCount = group.Count(),
                    PeriodStart = monthDate,
                    PeriodEnd = monthEnd
                };
            }).ToList();
        }

        private List<PeriodRevenueDto> GetRevenueByQuarter(List<Domain.Entities.Order> orders)
        {
            var quarterGroups = orders
                .GroupBy(o => new
                {
                    o.CreatedAt.Year,
                    Quarter = (o.CreatedAt.Month - 1) / 3 + 1
                })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Quarter);

            return quarterGroups.Select(group =>
            {
                var quarterStartMonth = (group.Key.Quarter - 1) * 3 + 1;
                var quarterStart = new DateTime(group.Key.Year, quarterStartMonth, 1);
                var quarterEnd = quarterStart.AddMonths(3).AddTicks(-1);

                return new PeriodRevenueDto
                {
                    Period = $"Q{group.Key.Quarter} {group.Key.Year}",
                    Revenue = group.Sum(o => o.Total.Amount),
                    OrderCount = group.Count(),
                    PeriodStart = quarterStart,
                    PeriodEnd = quarterEnd
                };
            }).ToList();
        }

        private List<PeriodRevenueDto> GetRevenueByYear(List<Domain.Entities.Order> orders)
        {
            var yearGroups = orders
                .GroupBy(o => o.CreatedAt.Year)
                .OrderBy(g => g.Key);

            return yearGroups.Select(group =>
            {
                var yearStart = new DateTime(group.Key, 1, 1);
                var yearEnd = new DateTime(group.Key, 12, 31, 23, 59, 59);

                return new PeriodRevenueDto
                {
                    Period = group.Key.ToString(),
                    Revenue = group.Sum(o => o.Total.Amount),
                    OrderCount = group.Count(),
                    PeriodStart = yearStart,
                    PeriodEnd = yearEnd
                };
            }).ToList();
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
