using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.DTOs.UserDTOs
{
    public class UserStatsDto
    {
        // Totales generales
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }

        // Por rol
        public int TotalClients { get; set; }
        public int TotalDeliveries { get; set; }
        public int TotalAdmins { get; set; }

        // Porcentajes
        public decimal ActivePercentage => TotalUsers > 0
            ? (decimal)ActiveUsers / TotalUsers * 100
            : 0;

        public decimal ClientsPercentage => TotalUsers > 0
            ? (decimal)TotalClients / TotalUsers * 100
            : 0;

        public decimal DeliveriesPercentage => TotalUsers > 0
            ? (decimal)TotalDeliveries / TotalUsers * 100
            : 0;
    }
}
