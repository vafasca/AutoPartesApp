using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Shared.Pages.Admin
{
    public partial class Dashboard : ComponentBase
    {
        [Inject]
        private NavigationManager? NavigationManager { get; set; }

        // State
        private bool hasNotifications = true;
        private string selectedPeriod = "week";

        // Data
        private AdminProfile adminProfile = new();
        private List<KpiMetric> kpiMetrics = new();
        private List<string> chartDays = new();
        private List<RecentOrder> recentOrders = new();

        protected override void OnInitialized()
        {
            LoadAdminProfile();
            LoadKpiMetrics();
            LoadChartDays();
            LoadRecentOrders();
        }

        private void LoadAdminProfile()
        {
            adminProfile = new AdminProfile
            {
                Name = "Administrador",
                AvatarUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuAirWVAqjNh64G5RMGvb9QzA0A63kJwopve3MT3L0TPNifh7qWDmsYXo84IzKVmUjiWFA5pWJfj1qD55jn4PfbS6N7X01gfkQHeTnkgwsCeVVEiCMzSDLMt546X_2xDN4mbwhFFHB7LIYpS5R21Fy1IUkCAY4hO7AKMG-RU9dzvZw4zO1CN_IOjTUt559wFigic_Q_qj4OCMrgPOCmM7LGe-3ixaom-ITXlOqA-jPMkEL3TJ5Xdfi6QWJymqCXKRm_sAEAf3LrbbFg"
            };
        }

        private void LoadKpiMetrics()
        {
            kpiMetrics = new List<KpiMetric>
            {
                new KpiMetric
                {
                    Title = "Ventas Totales",
                    Value = "$12,450.00",
                    Icon = "trending_up",
                    IconColor = "text-primary",
                    ChangeText = "+12.5% hoy",
                    ChangeColor = "text-[#0bda5b]",
                    ChangeBgColor = "bg-[#0bda5b]/10"
                },
                new KpiMetric
                {
                    Title = "Ticket Prom.",
                    Value = "$85.00",
                    Icon = "payments",
                    IconColor = "text-orange-400",
                    ChangeText = "-2.1% sem.",
                    ChangeColor = "text-[#fa6238]",
                    ChangeBgColor = "bg-[#fa6238]/10"
                },
                new KpiMetric
                {
                    Title = "Tasa Conv.",
                    Value = "3.2%",
                    Icon = "ads_click",
                    IconColor = "text-blue-400",
                    ChangeText = "+0.5% mes",
                    ChangeColor = "text-[#0bda5b]",
                    ChangeBgColor = "bg-[#0bda5b]/10"
                }
            };
        }

        private void LoadChartDays()
        {
            chartDays = new List<string> { "LUN", "MAR", "MIÉ", "JUE", "VIE", "SÁB", "HOY" };
        }

        private void LoadRecentOrders()
        {
            recentOrders = new List<RecentOrder>
            {
                new RecentOrder
                {
                    Id = 1,
                    OrderNumber = "#ORD-2891",
                    CustomerName = "Juan Pérez",
                    TimeAgo = "hace 10m",
                    Total = 245.50m,
                    Status = "PENDIENTE",
                    Icon = "receipt_long"
                },
                new RecentOrder
                {
                    Id = 2,
                    OrderNumber = "#ORD-2890",
                    CustomerName = "María G.",
                    TimeAgo = "hace 1h",
                    Total = 1120.00m,
                    Status = "ENVIADO",
                    Icon = "local_shipping"
                },
                new RecentOrder
                {
                    Id = 3,
                    OrderNumber = "#ORD-2889",
                    CustomerName = "Taller \"El Rayo\"",
                    TimeAgo = "hace 3h",
                    Total = 89.00m,
                    Status = "COMPLETADO",
                    Icon = "task_alt"
                }
            };
        }

        // UI Helpers
        private string GetPeriodButtonClass(string period)
        {
            return selectedPeriod == period
                ? "px-3 py-1 text-xs font-bold rounded-md bg-white dark:bg-[#233648] shadow-sm"
                : "px-3 py-1 text-xs font-bold text-slate-500 dark:text-slate-400";
        }

        private string GetStatusBadgeClass(string status)
        {
            return status switch
            {
                "PENDIENTE" => "text-[10px] px-2 py-0.5 rounded-full font-bold bg-yellow-500/10 text-yellow-500",
                "ENVIADO" => "text-[10px] px-2 py-0.5 rounded-full font-bold bg-primary/10 text-primary",
                "COMPLETADO" => "text-[10px] px-2 py-0.5 rounded-full font-bold bg-[#0bda5b]/10 text-[#0bda5b]",
                _ => "text-[10px] px-2 py-0.5 rounded-full font-bold bg-slate-100 dark:bg-slate-800 text-slate-600"
            };
        }

        // Event Handlers
        private void ToggleNotifications()
        {
            hasNotifications = false;
            StateHasChanged();
        }

        private void ChangePeriod(string period)
        {
            selectedPeriod = period;
            Console.WriteLine($"📊 Cambio de período: {period}");
            // Aquí cargarías datos del período seleccionado
        }

        private void GoToOrders() => NavigationManager?.NavigateTo("/admin/orders");
        private void GoToInventory() => NavigationManager?.NavigateTo("/admin/inventory");
        private void GoToReports() => NavigationManager?.NavigateTo("/admin/reports");
        private void ViewAllOrders() => NavigationManager?.NavigateTo("/admin/orders");

        private void ViewOrderDetail(int orderId)
        {
            Console.WriteLine($"Ver detalle del pedido: {orderId}");
            // NavigationManager?.NavigateTo($"/admin/order-detail/{orderId}");
        }

        // Data Models
        private class AdminProfile
        {
            public string Name { get; set; } = string.Empty;
            public string AvatarUrl { get; set; } = string.Empty;
        }

        private class KpiMetric
        {
            public string Title { get; set; } = string.Empty;
            public string Value { get; set; } = string.Empty;
            public string Icon { get; set; } = string.Empty;
            public string IconColor { get; set; } = string.Empty;
            public string ChangeText { get; set; } = string.Empty;
            public string ChangeColor { get; set; } = string.Empty;
            public string ChangeBgColor { get; set; } = string.Empty;
        }

        private class RecentOrder
        {
            public int Id { get; set; }
            public string OrderNumber { get; set; } = string.Empty;
            public string CustomerName { get; set; } = string.Empty;
            public string TimeAgo { get; set; } = string.Empty;
            public decimal Total { get; set; }
            public string Status { get; set; } = string.Empty;
            public string Icon { get; set; } = string.Empty;
        }
    }
}
