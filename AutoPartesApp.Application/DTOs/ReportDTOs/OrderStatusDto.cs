using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.DTOs.ReportDTOs
{
    public class OrderStatusDto
    {
        public AutoPartesApp.Domain.Enums.OrderStatus Status { get; set; }
        public int Count { get; set; }
        public decimal Percentage { get; set; }
        public string Color { get; set; } = "#3b82f6"; // Default blue
    }
}
