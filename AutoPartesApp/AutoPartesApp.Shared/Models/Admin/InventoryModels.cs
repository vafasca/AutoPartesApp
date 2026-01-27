using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Shared.Models.Admin
{
    public class InventoryStat
    {
        public string Title { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string TrendText { get; set; } = string.Empty;
        public string TrendColor { get; set; } = string.Empty;
    }

    public class FilterOption
    {
        public string Label { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
