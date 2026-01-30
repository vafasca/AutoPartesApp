using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.DTOs.ReportDTOs
{
    public class SalesReportDto
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public ReportPeriodDto Period { get; set; } = null!;
        public List<ChartDataDto> ChartData { get; set; } = new();
        public List<TopProductDto> TopProducts { get; set; } = new();
        public List<SalesByCategoryDto> SalesByCategory { get; set; } = new();
        public decimal GrowthPercentage { get; set; }
        public decimal PreviousPeriodRevenue { get; set; }
    }
}
