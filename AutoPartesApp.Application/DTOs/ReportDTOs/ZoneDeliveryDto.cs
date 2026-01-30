using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.DTOs.ReportDTOs
{
    public class ZoneDeliveryDto
    {
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public int TotalDeliveries { get; set; }
        public TimeSpan AverageTime { get; set; }
        public decimal CompletionRate { get; set; }
    }
}
