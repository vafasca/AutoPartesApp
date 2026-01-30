using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.DTOs.ReportDTOs
{
    public class DeliveryReportDto
    {
        public int TotalDeliveries { get; set; }
        public int CompletedDeliveries { get; set; }
        public int PendingDeliveries { get; set; }
        public TimeSpan AverageDeliveryTime { get; set; }
        public decimal EfficiencyRate { get; set; }
        public List<TopDriverDto> TopDrivers { get; set; } = new();
        public List<ZoneDeliveryDto> DeliveriesByZone { get; set; } = new();
        public int InTransitDeliveries { get; set; }
    }
}
