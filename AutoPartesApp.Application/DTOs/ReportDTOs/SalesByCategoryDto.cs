using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.DTOs.ReportDTOs
{
    public class SalesByCategoryDto
    {
        public string CategoryId { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public decimal TotalRevenue { get; set; }
        public int TotalUnits { get; set; }
        public decimal Percentage { get; set; }
        public string Color { get; set; } = "#3b82f6"; // Default blue
    }
}
