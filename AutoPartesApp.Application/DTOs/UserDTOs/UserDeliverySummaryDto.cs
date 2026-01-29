using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.DTOs.UserDTOs
{
    public class UserDeliverySummaryDto
    {
        public int TotalDeliveries { get; set; }
        public int CompletedDeliveries { get; set; }
        public int InProgressDeliveries { get; set; }
        public int CancelledDeliveries { get; set; }

        public DateTime? LastDeliveryDate { get; set; }
        public DateTime? FirstDeliveryDate { get; set; }

        // Tasa de éxito
        public decimal SuccessRate => TotalDeliveries > 0
            ? (decimal)CompletedDeliveries / TotalDeliveries * 100
            : 0;

        // Últimas 5 entregas
        public List<RecentDeliveryDto> RecentDeliveries { get; set; } = new();
    }

    public class RecentDeliveryDto
    {
        public string DeliveryId { get; set; } = string.Empty;
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime? AssignedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
