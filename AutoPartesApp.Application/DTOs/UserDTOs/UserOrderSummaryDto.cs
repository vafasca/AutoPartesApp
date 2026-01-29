using AutoPartesApp.Core.Application.DTOs.AdminDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.DTOs.UserDTOs
{
    public class UserOrderSummaryDto
    {
        public int TotalOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int PendingOrders { get; set; }
        public int CancelledOrders { get; set; }

        public decimal TotalSpent { get; set; }
        public string Currency { get; set; } = "USD";

        public DateTime? LastOrderDate { get; set; }
        public DateTime? FirstOrderDate { get; set; }

        // Últimas 5 órdenes
        public List<RecentOrderDto> RecentOrders { get; set; } = new();
    }
    public class RecentOrderDto
    {
        public string OrderId { get; set; } = string.Empty;
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public string Currency { get; set; } = "USD";
    }
}
