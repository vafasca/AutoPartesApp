using AutoPartesApp.Core.Application.DTOs.ReportDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Shared.Models.Admin
{
    /// <summary>
    /// Modelo para tarjetas de estadísticas en UI
    /// </summary>
    public class StatCardModel
    {
        public string Title { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string IconColor { get; set; } = string.Empty;
        public string TrendText { get; set; } = string.Empty;
        public string TrendColor { get; set; } = string.Empty;
        public string TrendIcon { get; set; } = string.Empty;
        public bool IsPositiveTrend { get; set; } = true;
    }

    /// <summary>
    /// Modelo para datos de gráficos en UI
    /// </summary>
    public class ChartDataModel
    {
        public string Label { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public string Color { get; set; } = string.Empty;
        public DateTime? Date { get; set; }
        public int Percentage { get; set; }
    }

    /// <summary>
    /// Modelo para opciones de filtros
    /// </summary>
    public class FilterOptionModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }

    /// <summary>
    /// Modelo para opciones de período
    /// </summary>
    public class PeriodOptionModel
    {
        public ReportPeriodType PeriodType { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public bool IsSelected { get; set; }

        public static List<PeriodOptionModel> GetDefaultOptions()
        {
            return new List<PeriodOptionModel>
            {
                new() { PeriodType = ReportPeriodType.Day, DisplayName = "Diario", ShortName = "Día", IsSelected = false },
                new() { PeriodType = ReportPeriodType.Week, DisplayName = "Semanal", ShortName = "Semana", IsSelected = false },
                new() { PeriodType = ReportPeriodType.Month, DisplayName = "Mensual", ShortName = "Mes", IsSelected = true },
                new() { PeriodType = ReportPeriodType.Year, DisplayName = "Anual", ShortName = "Año", IsSelected = false },
                new() { PeriodType = ReportPeriodType.Custom, DisplayName = "Personalizado", ShortName = "Custom", IsSelected = false }
            };
        }
    }

    /// <summary>
    /// Modelo para tabs de reportes
    /// </summary>
    public class ReportTabModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int? BadgeCount { get; set; }

        public static List<ReportTabModel> GetDefaultTabs()
        {
            return new List<ReportTabModel>
            {
                new() { Id = "sales", Name = "Ventas", Icon = "trending_up", IsActive = true },
                new() { Id = "inventory", Name = "Inventario", Icon = "inventory_2", IsActive = false },
                new() { Id = "customers", Name = "Clientes", Icon = "group", IsActive = false },
                new() { Id = "deliveries", Name = "Entregas", Icon = "local_shipping", IsActive = false },
                new() { Id = "orders", Name = "Órdenes", Icon = "receipt_long", IsActive = false },
                new() { Id = "financial", Name = "Financiero", Icon = "payments", IsActive = false }
            };
        }
    }
}
