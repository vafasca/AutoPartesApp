using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.DTOs.ReportDTOs
{
    public class TopProductDto
    {
        public string ProductId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int UnitsSold { get; set; }
        public decimal Revenue { get; set; }
        public string Trend { get; set; } = "stable"; // "up", "down", "stable"
        public decimal TrendPercentage { get; set; }
        public string? ImageUrl { get; set; }
        public decimal Cost { get; set; }
    }
}
