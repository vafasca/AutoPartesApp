using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Shared.Pages.Delivery
{
    public partial class History : ComponentBase
    {
        [Inject]
        private NavigationManager? NavigationManager { get; set; }

        // Stats
        private DeliveryStats stats = new();

        // History data
        private List<DeliveryHistoryItem> deliveryHistory = new();
        private bool hasMore = true;

        protected override void OnInitialized()
        {
            LoadStats();
            LoadDeliveryHistory();
        }

        private void LoadStats()
        {
            stats = new DeliveryStats
            {
                AverageTime = "22 min",
                TimeChange = -2,
                TotalDeliveries = 145,
                DeliveriesChange = 5
            };
        }

        private void LoadDeliveryHistory()
        {
            deliveryHistory = new List<DeliveryHistoryItem>
            {
                new DeliveryHistoryItem
                {
                    Id = 1,
                    OrderNumber = "8293",
                    Status = "Entregado",
                    Date = "12 Oct, 2023",
                    Duration = "18 min",
                    Amount = 12.50m
                },
                new DeliveryHistoryItem
                {
                    Id = 2,
                    OrderNumber = "8290",
                    Status = "Cancelado",
                    Date = "11 Oct, 2023",
                    Duration = "35 min",
                    Amount = 0.00m
                },
                new DeliveryHistoryItem
                {
                    Id = 3,
                    OrderNumber = "8285",
                    Status = "Entregado",
                    Date = "11 Oct, 2023",
                    Duration = "22 min",
                    Amount = 15.20m
                },
                new DeliveryHistoryItem
                {
                    Id = 4,
                    OrderNumber = "8271",
                    Status = "Entregado",
                    Date = "10 Oct, 2023",
                    Duration = "15 min",
                    Amount = 9.00m
                }
            };
        }

        // Navigation
        private void GoBack()
        {
            NavigationManager?.NavigateTo("/delivery/dashboard");
        }

        private void ViewDeliveryDetails(int deliveryId)
        {
            Console.WriteLine($"📦 Ver detalles de entrega #{deliveryId}");
            // NavigationManager?.NavigateTo($"/delivery/details/{deliveryId}");
        }

        private async Task LoadMore()
        {
            Console.WriteLine("📥 Cargando más entregas...");

            // Simular carga
            await Task.Delay(500);

            // En producción:
            // var moreDeliveries = await DeliveryService.GetHistory(page: 2);
            // deliveryHistory.AddRange(moreDeliveries);

            hasMore = false; // No hay más por ahora
            StateHasChanged();
        }

        // Helper Methods
        private string GetStatusIconClass(string status)
        {
            return status switch
            {
                "Entregado" => "text-primary bg-primary/10 dark:bg-primary/20",
                "Cancelado" => "text-slate-400 bg-slate-100 dark:bg-slate-800",
                _ => "text-slate-400 bg-slate-100 dark:bg-slate-800"
            };
        }

        private string GetStatusIcon(string status)
        {
            return status switch
            {
                "Entregado" => "local_shipping",
                "Cancelado" => "block",
                _ => "local_shipping"
            };
        }

        private string GetStatusTextClass(string status)
        {
            return status switch
            {
                "Entregado" => "text-slate-500 dark:text-slate-400",
                "Cancelado" => "text-slate-500 dark:text-slate-400",
                _ => "text-slate-500 dark:text-slate-400"
            };
        }

        private string GetStatusDotClass(string status)
        {
            return status switch
            {
                "Entregado" => "bg-green-500 shadow-[0_0_8px_rgba(11,218,91,0.5)]",
                "Cancelado" => "bg-red-500 shadow-[0_0_8px_rgba(250,98,56,0.5)]",
                _ => "bg-slate-400"
            };
        }

        private string GetAmountTextClass(string status)
        {
            return status switch
            {
                "Entregado" => "text-slate-900 dark:text-white",
                "Cancelado" => "text-slate-400 dark:text-slate-500",
                _ => "text-slate-900 dark:text-white"
            };
        }

        // Data Models
        private class DeliveryStats
        {
            public string AverageTime { get; set; } = string.Empty;
            public int TimeChange { get; set; }
            public int TotalDeliveries { get; set; }
            public int DeliveriesChange { get; set; }
        }

        private class DeliveryHistoryItem
        {
            public int Id { get; set; }
            public string OrderNumber { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public string Date { get; set; } = string.Empty;
            public string Duration { get; set; } = string.Empty;
            public decimal Amount { get; set; }
        }
    }
}
