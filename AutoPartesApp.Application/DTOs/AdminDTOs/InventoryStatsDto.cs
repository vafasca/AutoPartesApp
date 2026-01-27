using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.DTOs.AdminDTOs
{
    public class InventoryStatsDto
    {
        // Contadores
        public int TotalProducts { get; set; }
        public int ActiveProductsCount { get; set; }
        public int InactiveProductsCount { get; set; }

        // Stock
        public int LowStockCount { get; set; }
        public int OutOfStockCount { get; set; }
        public int AvailableStockCount { get; set; }

        // Valores monetarios
        public decimal TotalValue { get; set; }
        public string Currency { get; set; } = "USD";

        // Tendencias (opcional - para futuras mejoras)
        public decimal ValueTrendPercentage { get; set; } // +5.2%, -2.3%
        public string ValueTrendDirection { get; set; } = "neutral"; // "up", "down", "neutral"

        // Propiedades calculadas
        public string FormattedTotalValue => $"{TotalValue:C} {Currency}";
        public string LowStockAlertText => $"{LowStockCount} items";
        public string OutOfStockAlertText => $"{OutOfStockCount} agotados";
    }
}
