using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.DTOs.ReportDTOs
{
    public class InventoryReportDto
    {
        public int TotalProducts { get; set; }
        public decimal TotalValue { get; set; }
        public int OutOfStock { get; set; }
        public int LowStock { get; set; }
        public List<CategoryValueDto> ValueByCategory { get; set; } = new();
        public decimal AverageProductValue { get; set; }
        public int AvailableProducts { get; set; }
        public List<LowRotationProductDto> LowRotationProducts { get; set; } = new();
    }
}
