using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.DTOs.ReportDTOs
{
    public class CustomerReportDto
    {
        public int TotalCustomers { get; set; }
        public int NewCustomers { get; set; }
        public List<TopCustomerDto> TopCustomers { get; set; } = new();
        public int InactiveCustomers { get; set; }
        public List<GeographicDistributionDto> GeographicDistribution { get; set; } = new();
        public decimal RetentionRate { get; set; }
        public int ActiveCustomers { get; set; }
    }
}
