using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Shared.Pages.Delivery
{
    public partial class Dashboard : ComponentBase
    {
        [Inject]
        private NavigationManager? NavigationManager { get; set; }

        // State
        private string driverName = "Roberto";
        private bool hasNotifications = true;

        // Data
        private List<DailyStat> dailyStats = new();
        private DeliveryOrder? nextDelivery;
        private List<DeliveryOrder> pendingDeliveries = new();

        protected override void OnInitialized()
        {
            LoadDailyStats();
            LoadDeliveries();
        }

        private void LoadDailyStats()
        {
            dailyStats = new List<DailyStat>
            {
                new DailyStat { Label = "Total", Value = "15", ColorClass = "text-slate-500 dark:text-slate-400" },
                new DailyStat { Label = "Pendientes", Value = "4", ColorClass = "text-orange-500" },
                new DailyStat { Label = "Completados", Value = "11", ColorClass = "text-emerald-500" }
            };
        }

        private void LoadDeliveries()
        {
            // Siguiente entrega prioritaria
            nextDelivery = new DeliveryOrder
            {
                Id = 1,
                OrderNumber = "#ORD-9921",
                CustomerName = "Juan Pérez",
                Address = "Calle Falsa 123, Col. Centro",
                Status = "Asignado",
                MapImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuCShtNbjVWD_EJ1xANrIw-x99rVk0-k6DiXVocZecMdMoEricw1EEUIVoHf_SJFNrMTjiFny_FYNFWIF5tTT9QWy-TxQ6cUApY78WNOjJ38D6Xd98lcwGhvaHkuBzqIUR2H8PxQlSo2iMWrH0gRM2rO3ioCDugvDLpN0nTb9e1DnPjmMt9H1QI1xuqpKhC9IUOl5vdzYybsMH3d973IndHHoG3zKtSNaRrKfnT95A1BZktNiFj46y46CAbmJ3ViCNDlf6_9Xm3c6GU"
            };

            // Entregas pendientes
            pendingDeliveries = new List<DeliveryOrder>
            {
                new DeliveryOrder
                {
                    Id = 2,
                    OrderNumber = "#ORD-9922",
                    CustomerName = "María García",
                    Address = "Av. Insurgentes Sur 450, Int 4",
                    Status = "En Camino",
                    ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuC8WfhAjD9k5PrciEZ9zxal0NImi4rh3FgKoovN8k5iA7BSpRBtIpjBsw10B63TontrVF92d6sgjNvqDxGQ8FNCDOI_l-BzmDDoZI_eO-aB4A4kLjbfDeXUmuqhaGYab_yCvxMVb6cJAqycvPzSBAMn3HCkn5mjXXWpx9TV-yv7vago7f54F2-5_oNXsFHpCJIttnixgQO4tB0sZ40GXx-JrnIxqMlVyCcM3rGt3TPG6CcmNxaCdCAP6w-AqGT1kjEYTXiAFT636wI"
                },
                new DeliveryOrder
                {
                    Id = 3,
                    OrderNumber = "#ORD-9925",
                    CustomerName = "Ricardo Luna",
                    Address = "Callejón del Beso 45, Guanajuato",
                    Status = "Asignado",
                    ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuAQTe5A8Cr5ZMLgBvYBisAQndICe-WGBwZuXtiAy49pJknhQCVHJdfg_GstgqDGZEt6Xd6rpFr1ziInuffFOKg96Wl5TddRgXZSdcnAiKC4LBAKz4I2XNXvYL3TivlY9AxKPAkZgTB1WwSs3Fn986wkdRiA8p7vUPWbNoRk_7JwHEKMCKNjoKXfAnd0jdnTL12Y2fohKMLcG2cGhCpCMgy6pQ_cv9dLXZ3Vu3GGYbKjf4Klj6zgvSBNivc3LP0TIA1Yet2UeajsqBQ"
                },
                new DeliveryOrder
                {
                    Id = 4,
                    OrderNumber = "#ORD-9920",
                    CustomerName = "Ana Martínez",
                    Address = "Calle Morelos 89, Centro",
                    Status = "Asignado",
                    ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuC8WfhAjD9k5PrciEZ9zxal0NImi4rh3FgKoovN8k5iA7BSpRBtIpjBsw10B63TontrVF92d6sgjNvqDxGQ8FNCDOI_l-BzmDDoZI_eO-aB4A4kLjbfDeXUmuqhaGYab_yCvxMVb6cJAqycvPzSBAMn3HCkn5mjXXWpx9TV-yv7vago7f54F2-5_oNXsFHpCJIttnixgQO4tB0sZ40GXx-JrnIxqMlVyCcM3rGt3TPG6CcmNxaCdCAP6w-AqGT1kjEYTXiAFT636wI"
                }
            };
        }

        // UI Helpers
        private string GetDeliveryActionIcon(string status)
        {
            return status switch
            {
                "En Camino" => "check_circle",
                "Asignado" => "play_circle",
                _ => "info"
            };
        }

        private string GetDeliveryActionText(string status)
        {
            return status switch
            {
                "En Camino" => "Marcar Entregado",
                "Asignado" => "Iniciar Entrega",
                _ => "Ver Detalles"
            };
        }

        // Event Handlers
        private void ToggleNotifications()
        {
            hasNotifications = false;
            StateHasChanged();
        }

        private void ViewAllOrders()
        {
            Console.WriteLine("📋 Ver todos los pedidos");
            // NavigationManager?.NavigateTo("/delivery/all-orders");
        }

        private void StartDelivery(int deliveryId)
        {
            Console.WriteLine($"🚗 Iniciar entrega: {deliveryId}");
            NavigationManager?.NavigateTo($"/delivery/map?orderId={deliveryId}");
        }

        private void ViewDeliveryDetails(int deliveryId)
        {
            Console.WriteLine($"👁️ Ver detalles: {deliveryId}");
            // NavigationManager?.NavigateTo($"/delivery/order/{deliveryId}");
        }

        private void MarkAsDelivered(int deliveryId)
        {
            var delivery = pendingDeliveries.FirstOrDefault(d => d.Id == deliveryId);
            if (delivery != null)
            {
                Console.WriteLine($"✅ Marcar como entregado: {delivery.OrderNumber}");
                // Implementar confirmación y actualización
            }
        }

        // Data Models
        private class DailyStat
        {
            public string Label { get; set; } = string.Empty;
            public string Value { get; set; } = string.Empty;
            public string ColorClass { get; set; } = string.Empty;
        }

        private class DeliveryOrder
        {
            public int Id { get; set; }
            public string OrderNumber { get; set; } = string.Empty;
            public string CustomerName { get; set; } = string.Empty;
            public string Address { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public string ImageUrl { get; set; } = string.Empty;
            public string MapImageUrl { get; set; } = string.Empty;
        }
    }
}
