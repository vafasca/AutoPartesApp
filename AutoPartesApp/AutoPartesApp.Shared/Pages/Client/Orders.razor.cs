using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Shared.Pages.Client
{
    public partial class Orders : ComponentBase
    {
        [Inject]
        private NavigationManager? NavigationManager { get; set; }

        // State
        private string searchQuery = string.Empty;
        private string selectedFilter = "Todos";
        private bool isLoading = false;
        private bool hasNewNotifications = true;

        // Data
        private List<string> filters = new() { "Todos", "Pendientes", "En Camino", "Completados" };
        private List<OrderItem> allOrders = new();
        private List<OrderItem> filteredOrders = new();
        private int totalOrders = 0;

        protected override async Task OnInitializedAsync()
        {
            await LoadOrders();
        }

        private async Task LoadOrders()
        {
            isLoading = true;
            StateHasChanged();

            // Simular carga de datos
            await Task.Delay(500);

            allOrders = new List<OrderItem>
            {
                new OrderItem
                {
                    Id = 1,
                    OrderNumber = "ORD-77291",
                    Status = "Pendiente",
                    Date = "12 Oct 2023",
                    TimeAgo = "Hace 2 horas",
                    ItemsCount = 3,
                    Total = 245.00m,
                    ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuCpz8AO-Ul5-YrOvF9hRkADcphu0kwil3Of6V230a7QBxkyqrk4-WvbPBz1Vs_lgDa4orS9bJVqsYKJNCQ_c5UjYIap1GyJHVIJvyX2wsBm9YPMEX1lyV61csjqIjocEuebODFYUEKwUJJYsAhYeoSuWe35Ihsc_y47V5qz-WpoWPRmLSF3YmTSwJv7L2xCynqM49xqTV31Eyv5otit8a1DbklYZtJzH7OkcbPHg0eMtuE00XnrzX7qucWc4NrlAeCw-rekUW-g7uc"
                },
                new OrderItem
                {
                    Id = 2,
                    OrderNumber = "ORD-76150",
                    Status = "En Camino",
                    Date = "05 Oct 2023",
                    TimeAgo = "Ayer, 14:30",
                    ItemsCount = 1,
                    Total = 1120.50m,
                    ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuB6uZF50tsprRl4k5YWsf49JRa5sdX2rKWTwPD5A5Zf8XM3h-AQ5vQ9Xcjmwh3jGx0JJnXGSi8MujD-l0sB256gKOXrRIWcqY1lUX-VLalWmR_fuXLAWy4dqnN_tslyEPtWyzOGQKCS_dYgnoiP8lr3pFElzQX7THVvt7bC4rnl3PGsyVbLFEjNK2Yo8UQ3pdsjV1peap3f4fv6MF1-uHuErmnnrbrpV968f2lfWxdDPL6rWxPIgA-TnU2qVK6q4DeD8QZ9F331gYI"
                },
                new OrderItem
                {
                    Id = 3,
                    OrderNumber = "ORD-75902",
                    Status = "Entregado",
                    Date = "28 Sep 2023",
                    TimeAgo = "28 Sep 2023",
                    ItemsCount = 5,
                    Total = 89.99m,
                    ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuAS0wwKKyblieQqaAiJ0U3C9ZoXelcR_n_rw9Q6wltFXj0TbCc9x8ptW3o4wi0PM3hbiECnS9cwTqGv8yrO2A9G7w-UT5N4KRAn54nHtKOVbT3W7Oxrf2dt5LChUhRebLHoDLW_Cka9GninL6nmVBugw8sNzpGtOurtQQavDn2fa0ZIWvPXUb0STuUB-ASgh2cqz2qYFp0XrzZqrXuGkgOzQf9KYFGseQYHVu4WrjSCRlan5YXNAIU0xUO7d2lYif6puI53LFEeGvs"
                },
                new OrderItem
                {
                    Id = 4,
                    OrderNumber = "ORD-75801",
                    Status = "Pendiente",
                    Date = "25 Sep 2023",
                    TimeAgo = "Hace 1 día",
                    ItemsCount = 2,
                    Total = 156.75m,
                    ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuCfeqZOOmLto5AuFqiHyCkVj5jITAfAiump3dSS40ePery4jUahRV63eS3CzHfXe6YXoqhN7gxm6L--BbiG4AoBfwu_wBTXxrZQisNeVGAaP7O4chDg2CEPqII0aK3iwC3qFtH2KXUi3s6CUyO4k-CMK7_pTfxKl3pTRbUBSZDsquT1nu5uxIMu89yQjIq_Fx4dT51mXxxQtJA3uHvyMUk_8Mt2LpLZNKBXIP-HTBXj1LFaOznpYS9ndnazVkR3m_uRMyg2z98OWMc"
                },
                new OrderItem
                {
                    Id = 5,
                    OrderNumber = "ORD-75690",
                    Status = "En Camino",
                    Date = "20 Sep 2023",
                    TimeAgo = "Hace 3 días",
                    ItemsCount = 4,
                    Total = 432.20m,
                    ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuCTgu4qU0-MqiiKPlssEzEySBEAw2_9LTlxPCKJFbTlL-gC5BFCRc8Dg4PCQNcImkiwOlYElVbyiXYt_-KGG1tgMGg3kyyDjRegOWitVQpk-z8P7TW53Qu70XNUrgkasW2q-m9URcwpRufC0dMK07B37iMkCubsfMwqxqrer8V-HwaFp0PGriYlAAnjUGITCm-k-x65OZFEXEjAwjF80iDN4VEEWw02D8qUSzo15LO61UK5BboFCXfkqR6yCJfTbyA6NXYax3hSS_Y"
                },
                new OrderItem
                {
                    Id = 6,
                    OrderNumber = "ORD-75543",
                    Status = "Entregado",
                    Date = "15 Sep 2023",
                    TimeAgo = "15 Sep 2023",
                    ItemsCount = 1,
                    Total = 67.50m,
                    ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuB6uZF50tsprRl4k5YWsf49JRa5sdX2rKWTwPD5A5Zf8XM3h-AQ5vQ9Xcjmwh3jGx0JJnXGSi8MujD-l0sB256gKOXrRIWcqY1lUX-VLalWmR_fuXLAWy4dqnN_tslyEPtWyzOGQKCS_dYgnoiP8lr3pFElzQX7THVvt7bC4rnl3PGsyVbLFEjNK2Yo8UQ3pdsjV1peap3f4fv6MF1-uHuErmnnrbrpV968f2lfWxdDPL6rWxPIgA-TnU2qVK6q4DeD8QZ9F331gYI"
                },
                new OrderItem
                {
                    Id = 7,
                    OrderNumber = "ORD-75412",
                    Status = "Entregado",
                    Date = "10 Sep 2023",
                    TimeAgo = "10 Sep 2023",
                    ItemsCount = 3,
                    Total = 198.30m,
                    ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuAS0wwKKyblieQqaAiJ0U3C9ZoXelcR_n_rw9Q6wltFXj0TbCc9x8ptW3o4wi0PM3hbiECnS9cwTqGv8yrO2A9G7w-UT5N4KRAn54nHtKOVbT3W7Oxrf2dt5LChUhRebLHoDLW_Cka9GninL6nmVBugw8sNzpGtOurtQQavDn2fa0ZIWvPXUb0STuUB-ASgh2cqz2qYFp0XrzZqrXuGkgOzQf9KYFGseQYHVu4WrjSCRlan5YXNAIU0xUO7d2lYif6puI53LFEeGvs"
                },
                new OrderItem
                {
                    Id = 8,
                    OrderNumber = "ORD-75301",
                    Status = "Pendiente",
                    Date = "08 Sep 2023",
                    TimeAgo = "Hace 5 días",
                    ItemsCount = 2,
                    Total = 312.00m,
                    ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuCpz8AO-Ul5-YrOvF9hRkADcphu0kwil3Of6V230a7QBxkyqrk4-WvbPBz1Vs_lgDa4orS9bJVqsYKJNCQ_c5UjYIap1GyJHVIJvyX2wsBm9YPMEX1lyV61csjqIjocEuebODFYUEKwUJJYsAhYeoSuWe35Ihsc_y47V5qz-WpoWPRmLSF3YmTSwJv7L2xCynqM49xqTV31Eyv5otit8a1DbklYZtJzH7OkcbPHg0eMtuE00XnrzX7qucWc4NrlAeCw-rekUW-g7uc"
                }
            };

            totalOrders = allOrders.Count;
            filteredOrders = new List<OrderItem>(allOrders);

            isLoading = false;
            StateHasChanged();
        }

        private void FilterOrders(string filter)
        {
            selectedFilter = filter;
            ApplyFilters();
        }

        private void HandleSearch()
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var result = allOrders.AsEnumerable();

            // Filtrar por estado
            if (selectedFilter != "Todos")
            {
                result = result.Where(o => o.Status.Equals(selectedFilter, StringComparison.OrdinalIgnoreCase) ||
                                           (selectedFilter == "Completados" && o.Status == "Entregado"));
            }

            // Filtrar por búsqueda
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                result = result.Where(o =>
                    o.OrderNumber.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                    o.Status.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
                );
            }

            filteredOrders = result.ToList();
            StateHasChanged();
        }

        // UI Helper Methods
        private string GetStatusBadgeClass(string status)
        {
            return status switch
            {
                "Pendiente" => "bg-amber-100 text-amber-700 dark:bg-amber-900/30 dark:text-amber-400",
                "En Camino" => "bg-blue-100 text-blue-700 dark:bg-blue-900/30 dark:text-blue-400",
                "Entregado" => "bg-emerald-100 text-emerald-700 dark:bg-emerald-900/30 dark:text-emerald-400",
                "Cancelado" => "bg-red-100 text-red-700 dark:bg-red-900/30 dark:text-red-400",
                _ => "bg-slate-100 text-slate-700 dark:bg-slate-900/30 dark:text-slate-400"
            };
        }

        private string GetActionButtonClass(string status)
        {
            return status switch
            {
                "Pendiente" => "bg-primary/10 dark:bg-primary/20 text-primary",
                "En Camino" => "bg-primary/10 dark:bg-primary/20 text-primary",
                "Entregado" => "bg-slate-100 dark:bg-slate-800 text-slate-700 dark:text-white",
                _ => "bg-slate-100 dark:bg-slate-800 text-slate-700 dark:text-white"
            };
        }

        private string GetActionButtonText(string status)
        {
            return status switch
            {
                "Pendiente" => "Ver detalles",
                "En Camino" => "Rastrear",
                "Entregado" => "Reordenar",
                _ => "Ver detalles"
            };
        }

        private string GetActionButtonIcon(string status)
        {
            return status switch
            {
                "Pendiente" => "chevron_right",
                "En Camino" => "local_shipping",
                "Entregado" => "replay",
                _ => "chevron_right"
            };
        }

        // Action Handlers
        private void ViewOrderDetails(int orderId)
        {
            Console.WriteLine($"Ver detalles del pedido: {orderId}");
            // NavigationManager?.NavigateTo($"/client/order-details/{orderId}");
        }

        private void HandleOrderAction(OrderItem order)
        {
            switch (order.Status)
            {
                case "Pendiente":
                    ViewOrderDetails(order.Id);
                    break;
                case "En Camino":
                    TrackOrder(order.Id);
                    break;
                case "Entregado":
                    ReorderItems(order.Id);
                    break;
                default:
                    ViewOrderDetails(order.Id);
                    break;
            }
        }

        private void TrackOrder(int orderId)
        {
            Console.WriteLine($"Rastrear pedido: {orderId}");
            NavigationManager?.NavigateTo("/client/tracking");
        }

        private void ReorderItems(int orderId)
        {
            Console.WriteLine($"Reordenar pedido: {orderId}");
            // Implementar lógica de reorden
        }

        private void ToggleNotifications()
        {
            Console.WriteLine("Toggle notifications");
            hasNewNotifications = false;
            StateHasChanged();
        }

        private void GoToCart()
        {
            Console.WriteLine("Ir al carrito");
            // NavigationManager?.NavigateTo("/client/cart");
        }

        private void GoToCatalog()
        {
            NavigationManager?.NavigateTo("/client/catalog");
        }

        private void GoBack()
        {
            NavigationManager?.NavigateTo("/client/dashboard");
        }

        // Data Model
        private class OrderItem
        {
            public int Id { get; set; }
            public string OrderNumber { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public string Date { get; set; } = string.Empty;
            public string TimeAgo { get; set; } = string.Empty;
            public int ItemsCount { get; set; }
            public decimal Total { get; set; }
            public string ImageUrl { get; set; } = string.Empty;
        }
    }
}
