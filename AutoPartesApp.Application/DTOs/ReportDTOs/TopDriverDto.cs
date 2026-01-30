using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.DTOs.ReportDTOs
{
    public class TopDriverDto
    {
        public string DriverId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public int TotalDeliveries { get; set; }
        public int CompletedDeliveries { get; set; }
        public TimeSpan AverageTime { get; set; }
        public decimal EfficiencyRate { get; set; }
        public string? AvatarUrl { get; set; }
    }
}
