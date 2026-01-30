using AutoPartesApp.Core.Application.DTOs.ReportDTOs;
using AutoPartesApp.Domain.Enums;
using AutoPartesApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.Reports
{
    public class GetDeliveryReportUseCase
    {
        private readonly IDeliveryRepository _deliveryRepository;

        public GetDeliveryReportUseCase(IDeliveryRepository deliveryRepository)
        {
            _deliveryRepository = deliveryRepository;
        }

        public async Task<DeliveryReportDto> ExecuteAsync(ReportFilterDto filter)
        {
            var dateTo = filter.DateTo ?? DateTime.UtcNow;
            var dateFrom = filter.DateFrom ?? dateTo.AddMonths(-1);

            // Obtener entregas del período
            var deliveries = await _deliveryRepository.GetDeliveriesByDateRangeAsync(
                dateFrom,
                dateTo
            );

            var totalDeliveries = deliveries.Count;
            var completedDeliveries = deliveries
                .Count(d => d.Status == DeliveryStatus.Delivered);
            var pendingDeliveries = deliveries
                .Count(d => d.Status == DeliveryStatus.Pending || d.Status == DeliveryStatus.Assigned);
            var inTransitDeliveries = deliveries
                .Count(d => d.Status == DeliveryStatus.OnRoute || d.Status == DeliveryStatus.PickedUp);

            // Calcular tiempo promedio de entrega (en horas)
            var completedWithTime = deliveries
                .Where(d => d.Status == DeliveryStatus.Delivered
                    && d.AssignedAt.HasValue
                    && d.DeliveredAt.HasValue)
                .ToList();

            var averageDeliveryTime = completedWithTime.Any()
    ? TimeSpan.FromHours(completedWithTime.Average(d =>
        (d.DeliveredAt!.Value - d.AssignedAt!.Value).TotalHours))
    : TimeSpan.Zero;


            // Tasa de eficiencia
            var efficiencyRate = totalDeliveries > 0
                ? ((decimal)completedDeliveries / totalDeliveries) * 100
                : 0;

            // Top repartidores
            var topDrivers = await GetTopDrivers(deliveries, 5);

            // Entregas por zona
            var deliveriesByZone = GetDeliveriesByZone(deliveries);

            return new DeliveryReportDto
            {
                TotalDeliveries = totalDeliveries,
                CompletedDeliveries = completedDeliveries,
                PendingDeliveries = pendingDeliveries,
                AverageDeliveryTime = averageDeliveryTime,
                EfficiencyRate = efficiencyRate,
                TopDrivers = topDrivers,
                DeliveriesByZone = deliveriesByZone,
                InTransitDeliveries = inTransitDeliveries
            };
        }

        private async Task<List<TopDriverDto>> GetTopDrivers(
            List<Domain.Entities.Delivery> deliveries,
            int topN)
        {
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

                    var completedWithTime = driverDeliveries
                        .Where(d => d.Status == DeliveryStatus.Delivered
                            && d.AssignedAt.HasValue
                            && d.DeliveredAt.HasValue)
                        .ToList();

                    var avgTime = completedWithTime.Any()
    ? TimeSpan.FromHours(completedWithTime.Average(d =>
        (d.DeliveredAt!.Value - d.AssignedAt!.Value).TotalHours))
    : TimeSpan.Zero;

                    return new TopDriverDto
                    {
                        DriverId = g.Key.DriverId!,
                        FullName = g.Key.DriverName,
                        TotalDeliveries = total,
                        CompletedDeliveries = completed,
                        AverageTime = avgTime,
                        EfficiencyRate = total > 0 ? ((decimal)completed / total) * 100 : 0,
                        AvatarUrl = g.Key.AvatarUrl
                    };
                })
                .OrderByDescending(d => d.EfficiencyRate)
                .ThenByDescending(d => d.CompletedDeliveries)
                .Take(topN)
                .ToList();

            return driverStats;
        }

        private List<ZoneDeliveryDto> GetDeliveriesByZone(
            List<Domain.Entities.Delivery> deliveries)
        {
            var zoneStats = deliveries
                .Where(d => d.Order?.DeliveryAddress != null)
                .GroupBy(d => new
                {
                    City = d.Order.DeliveryAddress.City,
                    State = d.Order.DeliveryAddress.State
                })
                .Select(g =>
                {
                    var zoneDeliveries = g.ToList();
                    var completed = zoneDeliveries.Count(d => d.Status == DeliveryStatus.Delivered);

                    var completedWithTime = zoneDeliveries
                        .Where(d => d.Status == DeliveryStatus.Delivered
                            && d.AssignedAt.HasValue
                            && d.DeliveredAt.HasValue)
                        .ToList();

                    var avgTime = completedWithTime.Any()
    ? TimeSpan.FromHours(completedWithTime.Average(d =>
        (d.DeliveredAt!.Value - d.AssignedAt!.Value).TotalHours))
    : TimeSpan.Zero;

                    return new ZoneDeliveryDto
                    {
                        City = g.Key.City,
                        State = g.Key.State,
                        TotalDeliveries = zoneDeliveries.Count,
                        AverageTime = avgTime,
                        CompletionRate = zoneDeliveries.Count > 0
                            ? ((decimal)completed / zoneDeliveries.Count) * 100
                            : 0
                    };
                })
                .OrderByDescending(z => z.TotalDeliveries)
                .Take(10)
                .ToList();

            return zoneStats;
        }
    }
}
