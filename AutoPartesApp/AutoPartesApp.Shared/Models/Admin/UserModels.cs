using AutoPartesApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Shared.Models.Admin
{
    /// <summary>
    /// Opción de filtro para dropdowns
    /// </summary>
    public class FilterOption<T>
    {
        public string Label { get; set; } = string.Empty;
        public T Value { get; set; } = default!;
        public string? Icon { get; set; }
    }

    /// <summary>
    /// Estadística individual para tarjetas
    /// </summary>
    public class UserStatCard
    {
        public string Label { get; set; } = string.Empty;
        public int Value { get; set; }
        public string Icon { get; set; } = string.Empty;
        public string Color { get; set; } = "blue"; // blue, green, yellow, red
        public decimal? Percentage { get; set; }
        public string? Trend { get; set; } // "up", "down", "stable"
    }

    /// <summary>
    /// Opciones predefinidas de filtros
    /// </summary>
    public static class UserFilterOptions
    {
        public static List<FilterOption<RoleType?>> RoleFilters => new()
        {
            new FilterOption<RoleType?>
            {
                Label = "Todos los roles",
                Value = null,
                Icon = "group"
            },
            new FilterOption<RoleType?>
            {
                Label = "Clientes",
                Value = RoleType.Client,
                Icon = "person"
            },
            new FilterOption<RoleType?>
            {
                Label = "Repartidores",
                Value = RoleType.Delivery,
                Icon = "local_shipping"
            },
            new FilterOption<RoleType?>
            {
                Label = "Administradores",
                Value = RoleType.Admin,
                Icon = "admin_panel_settings"
            }
        };

        public static List<FilterOption<bool?>> StatusFilters => new()
        {
            new FilterOption<bool?>
            {
                Label = "Todos los estados",
                Value = null
            },
            new FilterOption<bool?>
            {
                Label = "Activos",
                Value = true
            },
            new FilterOption<bool?>
            {
                Label = "Inactivos",
                Value = false
            }
        };

        public static List<FilterOption<int>> PageSizeOptions => new()
        {
            new FilterOption<int> { Label = "10 por página", Value = 10 },
            new FilterOption<int> { Label = "20 por página", Value = 20 },
            new FilterOption<int> { Label = "50 por página", Value = 50 },
            new FilterOption<int> { Label = "100 por página", Value = 100 }
        };
    }

    /// <summary>
    /// Modelo para ordenamiento de tabla
    /// </summary>
    public class SortOption
    {
        public string Field { get; set; } = string.Empty;
        public SortDirection Direction { get; set; } = SortDirection.Ascending;
    }

    public enum SortDirection
    {
        Ascending,
        Descending
    }

    /// <summary>
    /// Acción en lote sobre usuarios
    /// </summary>
    public class BulkAction
    {
        public string Id { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string ConfirmMessage { get; set; } = string.Empty;
        public bool RequiresConfirmation { get; set; } = true;
    }

    /// <summary>
    /// Acciones en lote predefinidas
    /// </summary>
    public static class UserBulkActions
    {
        public static List<BulkAction> AvailableActions => new()
        {
            new BulkAction
            {
                Id = "activate",
                Label = "Activar seleccionados",
                Icon = "check_circle",
                ConfirmMessage = "¿Activar los usuarios seleccionados?",
                RequiresConfirmation = true
            },
            new BulkAction
            {
                Id = "deactivate",
                Label = "Desactivar seleccionados",
                Icon = "block",
                ConfirmMessage = "¿Desactivar los usuarios seleccionados?",
                RequiresConfirmation = true
            },
            new BulkAction
            {
                Id = "delete",
                Label = "Eliminar seleccionados",
                Icon = "delete",
                ConfirmMessage = "¿Eliminar permanentemente los usuarios seleccionados? Esta acción no se puede deshacer.",
                RequiresConfirmation = true
            },
            new BulkAction
            {
                Id = "export",
                Label = "Exportar seleccionados",
                Icon = "download",
                ConfirmMessage = "",
                RequiresConfirmation = false
            }
        };
    }

    /// <summary>
    /// Datos para gráficos y visualizaciones
    /// </summary>
    public class UserChartData
    {
        public string Label { get; set; } = string.Empty;
        public int Value { get; set; }
        public string Color { get; set; } = string.Empty;
    }

    /// <summary>
    /// Actividad reciente del usuario
    /// </summary>
    public class UserActivity
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Icon { get; set; } = string.Empty;
        public string? IpAddress { get; set; }
    }

    /// <summary>
    /// Notificación para el usuario
    /// </summary>
    public class UserNotification
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; } = NotificationType.Info;
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }

    public enum NotificationType
    {
        Info,
        Success,
        Warning,
        Error
    }
}
