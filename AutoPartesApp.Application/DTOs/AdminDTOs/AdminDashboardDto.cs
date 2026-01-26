using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.DTOs.AdminDTOs
{
    public class AdminDashboardDto
    {
        public DashboardStatsDto Stats { get; set; } = new();
        public List<RecentOrderDto> RecentOrders { get; set; } = new();
        public SalesChartDataDto SalesChart { get; set; } = new();
    }
}
