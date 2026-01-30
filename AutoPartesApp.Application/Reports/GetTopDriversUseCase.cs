using AutoPartesApp.Core.Application.DTOs.ReportDTOs;
using AutoPartesApp.Domain.Enums;
using AutoPartesApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.Reports
{
    public class GetTopDriversUseCase
    {
        private readonly IDeliveryRepository _deliveryRepository;

        public GetTopDriversUseCase(IDeliveryRepository deliveryRepository)
        {
            _deliveryRepository = deliveryRepository;
        }

        public async Task<List<TopDriverDto>> ExecuteAsync(
            ReportFilterDto filter,
            int topN = 10)
        {
            var dateTo = filter.DateTo ?? DateTime.UtcNow;
            var dateFrom = filter.DateFrom ?? dateTo.AddMonths(-1);

            var deliveries = await _deliveryRepository.GetDeliveriesByDateRangeAsync(
                dateFrom,
                dateTo
            );

            var driverStats = deliveries
                .Where(d => !string.IsNullOrEmpty(d.DriverId))
                .GroupBy(d => new
                {
                    d.DriverId,
                    DriverName = d.Driver?.FullName ?? "N/A",
                    d.Driver?.AvatarUrl
                })
                .Select(g =>
                {
                    var driverDeliveries = g.ToList();
                    var completed = driverDeliveries.Count(d => d.Status == DeliveryStatus.Delivered);
                    var total = driverDeliveries.Count;

                    // Calcular tiempo promedio de entrega
                    var completedWithTime = driverDeliveries
                        .Where(d => d.Status == DeliveryStatus.Delivered
                            && d.AssignedAt.HasValue
                            && d.DeliveredAt.HasValue)
                        .ToList();

                    var avgTime = completedWithTime.Any()
    ? TimeSpan.FromHours(completedWithTime.Average(d =>
        (d.DeliveredAt!.Value - d.AssignedAt!.Value).TotalHours))
    : TimeSpan.Zero;

                    var efficiencyRate = total > 0
                        ? ((decimal)completed / total) * 100
                        : 0;

                    return new TopDriverDto
                    {
                        DriverId = g.Key.DriverId!,
                        FullName = g.Key.DriverName,
                        TotalDeliveries = total,
                        CompletedDeliveries = completed,
                        AverageTime = avgTime,
                        EfficiencyRate = efficiencyRate,
                        AvatarUrl = g.Key.AvatarUrl
                    };
                })
                .OrderByDescending(d => d.EfficiencyRate)
                .ThenByDescending(d => d.CompletedDeliveries)
                .ThenBy(d => d.AverageTime)
                .Take(topN)
                .ToList();

            return driverStats;
        }
    }
}
