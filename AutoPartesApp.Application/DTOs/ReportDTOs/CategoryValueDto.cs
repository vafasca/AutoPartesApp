using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.DTOs.ReportDTOs
{
    public class CategoryValueDto
    {
        public string CategoryName { get; set; } = string.Empty;
        public decimal TotalValue { get; set; }
        public int TotalUnits { get; set; }
        public decimal Percentage { get; set; }
    }
}
