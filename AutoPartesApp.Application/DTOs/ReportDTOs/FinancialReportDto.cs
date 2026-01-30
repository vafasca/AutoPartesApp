using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.DTOs.ReportDTOs
{
    public class FinancialReportDto
    {
        public decimal TotalRevenue { get; set; }
        public decimal AverageTicket { get; set; }
        public List<PeriodRevenueDto> RevenueByPeriod { get; set; } = new();
        public List<TopProductDto> TopRevenueProducts { get; set; } = new();
        public decimal? ProjectedRevenue { get; set; }
        public decimal GrowthRate { get; set; }
        public decimal TotalCosts { get; set; }
        public decimal NetProfit { get; set; }
        public List<ProjectionDto> Projections { get; set; } = new();
    }
}
