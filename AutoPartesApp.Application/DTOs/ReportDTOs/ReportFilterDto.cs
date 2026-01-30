using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.DTOs.ReportDTOs
{
    public class ReportFilterDto
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string? CategoryId { get; set; }
        public string? UserId { get; set; }
        public string? Status { get; set; }
        public ReportPeriodType? Period { get; set; }
    }
    public enum ReportPeriodType
    {
        Day,
        Week,
        Month,
        Quarter,
        Year,
        Custom
    }

    public class ReportPeriodDto
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public ReportPeriodType PeriodType { get; set; }
        public string DisplayText { get; set; } = string.Empty;
    }
}
