using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.DTOs.ReportDTOs
{
    public class OrderReportDto
    {
        public int TotalOrders { get; set; }
        public List<OrderStatusDto> OrdersByStatus { get; set; } = new();
        public TimeSpan AverageProcessingTime { get; set; }

        public decimal CancellationRate { get; set; }
        public int PendingOrders { get; set; }
        public int CompletedOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public List<PendingOrderDto> PendingOrdersList { get; set; } = new();

    }
}
