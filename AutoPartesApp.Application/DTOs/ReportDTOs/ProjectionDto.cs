using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.DTOs.ReportDTOs
{
    public class ProjectionDto
    {
        public string Period { get; set; } = string.Empty;
        public decimal ProjectedRevenue { get; set; }

    }
}
