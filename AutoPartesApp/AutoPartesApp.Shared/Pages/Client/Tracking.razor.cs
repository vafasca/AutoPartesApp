using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Shared.Pages.Client
{
    public partial class Tracking : ComponentBase
    {
        [Parameter]
        public string? OrderId { get; set; }

        [Inject]
        private NavigationManager? NavigationManager { get; set; }

        // State
        private bool hasNewNotifications = true;
        private int cartItemsCount = 3;
        private bool isLoading = false;

        // Current Order Data
        private OrderTrackingInfo currentOrder = new();
        private List<TrackingStep> trackingSteps = new();

        protected override async Task OnInitializedAsync()
        {
            await LoadTrackingData();
        }

        protected override async Task OnParametersSetAsync()
        {
            if (!string.IsNullOrEmpty(OrderId))
            {
                await LoadTrackingData(OrderId);
            }
        }

        private async Task LoadTrackingData(string? orderId = null)
        {
            isLoading = true;
            StateHasChanged();

            // Simular carga de datos
            await Task.Delay(500);

            // Datos del pedido actual
            currentOrder = new OrderTrackingInfo
            {
                OrderNumber = orderId ?? "ORD-76150",
                Status = "En Camino",
                EstimatedArrival = "15:20 - 15:40",
                DriverName = "Carlos Martínez",
                DriverPhone = "+52 555 123 4567",
                VehicleInfo = "Honda CRV Blanca • ABC-1234",
                MapImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuAbr-BgbvZbjjPwOCIiF48KjdFkK04LkowZ6AkTYy770LltyHY6VBb4qU0-5PsRNkOlSIEaySc2YJoKd53gWgODzj-bCxn6WxIaRAlUgMpNsXNrM-FEe_q1O2q06cV5e8XYebKJFBj48iHYLHgQwbRQYnTOSM0E3p9UhzX3aoO-kM-DUqlplhBeRXl7pdH3097WhdElSRT4z-HVLz8ElqnXKqaqNT6O_-XPbUpVHHfgVI21D_ND3Tc_TeuUqON42OM2DhS5le_bMGw",
                CurrentLocation = "A 2.3 km de tu ubicación",
                LastUpdate = DateTime.Now
            };

            // Timeline de tracking
            trackingSteps = new List<TrackingStep>
            {
                new TrackingStep
                {
                    Id = 1,
                    Title = "Pedido Recibido",
                    Time = "Hoy, 14:05",
                    Description = "Tu pedido ha sido confirmado",
                    Icon = "check_circle",
                    Status = TrackingStepStatus.Completed,
                    IsLast = false
                },
                new TrackingStep
                {
                    Id = 2,
                    Title = "Preparado",
                    Time = "Hoy, 14:45",
                    Description = "Empaquetado y listo para envío",
                    Icon = "check_circle",
                    Status = TrackingStepStatus.Completed,
                    IsLast = false
                },
                new TrackingStep
                {
                    Id = 3,
                    Title = "En Camino",
                    Time = "En curso...",
                    Description = $"El repartidor está en camino. {currentOrder.CurrentLocation}",
                    Icon = "local_shipping",
                    Status = TrackingStepStatus.Active,
                    IsLast = false
                },
                new TrackingStep
                {
                    Id = 4,
                    Title = "Entregado",
                    Time = "Pendiente",
                    Description = null,
                    Icon = "schedule",
                    Status = TrackingStepStatus.Pending,
                    IsLast = true
                }
            };

            isLoading = false;
            StateHasChanged();
        }

        private async Task RefreshLocation()
        {
            Console.WriteLine("🔄 Actualizando ubicación...");

            // Simular actualización
            await Task.Delay(500);

            currentOrder.LastUpdate = DateTime.Now;
            currentOrder.CurrentLocation = "A 1.8 km de tu ubicación";

            // Actualizar el paso activo
            var activeStep = trackingSteps.FirstOrDefault(s => s.Status == TrackingStepStatus.Active);
            if (activeStep != null)
            {
                activeStep.Description = $"El repartidor está en camino. {currentOrder.CurrentLocation}";
            }

            StateHasChanged();

            Console.WriteLine("✅ Ubicación actualizada");
        }

        // Helper Methods para clases CSS dinámicas
        private string GetStepCircleClass(TrackingStep step)
        {
            return step.Status switch
            {
                TrackingStepStatus.Completed => "bg-primary/20 dark:bg-primary/30",
                TrackingStepStatus.Active => "bg-primary shadow-[0_0_15px_rgba(19,127,236,0.4)] animate-pulse-slow",
                TrackingStepStatus.Pending => "bg-slate-100 dark:bg-slate-800 border-2 border-slate-200 dark:border-slate-700",
                _ => "bg-slate-100 dark:bg-slate-800"
            };
        }

        private string GetStepIconSize(TrackingStep step)
        {
            return step.Status == TrackingStepStatus.Active ? "text-xl" : "text-[20px]";
        }

        private string GetStepLineClass(TrackingStep step)
        {
            return step.Status == TrackingStepStatus.Completed ? "bg-primary" : "bg-slate-200 dark:bg-slate-700";
        }

        private string GetStepTitleClass(TrackingStep step)
        {
            return step.Status switch
            {
                TrackingStepStatus.Completed => "text-slate-900 dark:text-white",
                TrackingStepStatus.Active => "text-primary",
                TrackingStepStatus.Pending => "text-slate-400 dark:text-slate-500",
                _ => "text-slate-900 dark:text-white"
            };
        }

        private string GetStepTimeClass(TrackingStep step)
        {
            return step.Status switch
            {
                TrackingStepStatus.Completed => "text-slate-500 dark:text-[#92adc9]",
                TrackingStepStatus.Active => "text-primary/80 font-medium italic",
                TrackingStepStatus.Pending => "text-slate-400 dark:text-[#92adc9]/40",
                _ => "text-slate-500 dark:text-[#92adc9]"
            };
        }

        // Action Handlers
        private void ContactDriver()
        {
            Console.WriteLine($"💬 Contactar a {currentOrder.DriverName}");
            // Aquí se abriría un chat o modal de mensajería
            // NavigationManager?.NavigateTo($"/client/chat/{currentOrder.DriverName}");
        }

        private void CallDriver()
        {
            Console.WriteLine($"📞 Llamar a {currentOrder.DriverPhone}");
            // En una app real, esto abriría el dialer del teléfono
            // En web, podría ser: window.location.href = `tel:${currentOrder.DriverPhone}`
        }

        private void ShareLocation()
        {
            Console.WriteLine("📍 Compartir ubicación del pedido");
            // Implementar funcionalidad de compartir
            // navigator.share() API en web
        }

        private void ToggleNotifications()
        {
            hasNewNotifications = false;
            StateHasChanged();
        }

        private void GoToCart()
        {
            NavigationManager?.NavigateTo("/client/cart");
        }

        private void GoBack()
        {
            NavigationManager?.NavigateTo("/client/orders");
        }

        // Data Models
        private class OrderTrackingInfo
        {
            public string OrderNumber { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public string EstimatedArrival { get; set; } = string.Empty;
            public string DriverName { get; set; } = string.Empty;
            public string DriverPhone { get; set; } = string.Empty;
            public string VehicleInfo { get; set; } = string.Empty;
            public string MapImageUrl { get; set; } = string.Empty;
            public string CurrentLocation { get; set; } = string.Empty;
            public DateTime LastUpdate { get; set; }
        }

        private class TrackingStep
        {
            public int Id { get; set; }
            public string Title { get; set; } = string.Empty;
            public string Time { get; set; } = string.Empty;
            public string? Description { get; set; }
            public string Icon { get; set; } = string.Empty;
            public TrackingStepStatus Status { get; set; }
            public bool IsLast { get; set; }
        }

        private string GetStepIconColor(TrackingStep step)
        {
            return step.Status switch
            {
                TrackingStepStatus.Completed => "text-primary",
                TrackingStepStatus.Active => "text-white",
                TrackingStepStatus.Pending => "text-slate-400 dark:text-slate-500",
                _ => "text-slate-500 dark:text-[#92adc9]"
            };
        }

        private enum TrackingStepStatus
        {
            Completed,
            Active,
            Pending
        }
    }
}
