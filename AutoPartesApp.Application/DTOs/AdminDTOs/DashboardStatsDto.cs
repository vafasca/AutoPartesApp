using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.DTOs.AdminDTOs
{
    public class DashboardStatsDto
    {
        // DTO para las estadísticas principales del dashboard
        public decimal TotalSales { get; set; }
        public decimal SalesChangePercentage { get; set; }
        public decimal AverageTicket { get; set; }
        public decimal TicketChangePercentage { get; set; }
        public decimal ConversionRate { get; set; }
        public decimal ConversionChangePercentage { get; set; }
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int LowStockProducts { get; set; }
    }
}
