using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.DTOs.ReportDTOs
{
    public class PeriodRevenueDto
    {
        public string Period { get; set; } = string.Empty; // "Enero 2024", "Semana 15", etc.
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
    }
}
