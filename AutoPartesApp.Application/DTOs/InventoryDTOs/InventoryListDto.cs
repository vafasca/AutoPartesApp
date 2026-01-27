using System;
using System.Collections.Generic;

namespace AutoPartesApp.Application.DTOs.InventoryDTOs
{
    public class InventoryListDto
    {
        public IEnumerable<InventoryItemDto> Items { get; set; }
        public int TotalItems { get; set; }
        public int TotalStock { get; set; }
        public int LowStockItemsCount { get; set; }
        public Dictionary<string, int> StockByCategory { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }
}