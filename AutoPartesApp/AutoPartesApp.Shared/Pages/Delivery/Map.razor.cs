using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Shared.Pages.Delivery
{
    public partial class Map : ComponentBase
    {
        [Inject]
        private NavigationManager? NavigationManager { get; set; }

        // Current delivery data
        private DeliveryMapData currentDelivery = new();

        protected override void OnInitialized()
        {
            LoadDeliveryData();
        }

        private void LoadDeliveryData()
        {
            // Simular datos de entrega actual
            currentDelivery = new DeliveryMapData
            {
                OrderId = "3928",
                CustomerName = "Juan Pérez",
                CustomerAvatar = "https://lh3.googleusercontent.com/aida-public/AB6AXuAoF5CLUv7qoVVBpRhwHa-qumnAAsBrbPzdAcWoB2-oFFSDexr0GvM1P2j-xcVrlCaE2ZlwiwyITLeRp90gpBI4eVEZBL92BflxVjA9TIMWCzKq5YoVcr8Z01S2vVc-Xy8Ee4V0cspf27SwwGbglUSAYURMQV_aOTcq93KJN5l8QcwtiZX2iacuMrfA8sLU_0_T1HyFnsN8zH_xq7x-xzqu9A5qPq1PlOvc-ChO-_14HAIlFa5dumXGgK-0lRtfSuzv-u1W508_wKY",
                EstimatedTime = "12 min",
                Distance = "2.4 km",
                NextTurn = "Av. Insurgentes Sur, 150m",
                MapImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuCShtNbjVWD_EJ1xANrIw-x99rVk0-k6DiXVocZecMdMoEricw1EEUIVoHf_SJFNrMTjiFny_FYNFWIF5tTT9QWy-TxQ6cUApY78WNOjJ38D6Xd98lcwGhvaHkuBzqIUR2H8PxQlSo2iMWrH0gRM2rO3ioCDugvDLpN0nTb9e1DnPjmMt9H1QI1xuqpKhC9IUOl5vdzYybsMH3d973IndHHoG3zKtSNaRrKfnT95A1BZktNiFj46y46CAbmJ3ViCNDlf6_9Xm3c6GU"
            };
        }

        // Navigation
        private void GoBack()
        {
            NavigationManager?.NavigateTo("/delivery/dashboard");
        }

        // Map Controls
        private void ZoomIn()
        {
            Console.WriteLine("🔍 Zoom In");
            // Implementar lógica de zoom
        }

        private void ZoomOut()
        {
            Console.WriteLine("🔍 Zoom Out");
            // Implementar lógica de zoom
        }

        private void CenterLocation()
        {
            Console.WriteLine("📍 Centrar ubicación actual");
            // Implementar centrado de mapa
        }

        private void SearchLocation()
        {
            Console.WriteLine("🔍 Buscar ubicación");
            // Abrir buscador de direcciones
        }

        // Actions
        private async Task ShareLocation()
        {
            Console.WriteLine("📤 Compartir ubicación GPS");

            // Simular compartir
            await Task.Delay(500);

            // En producción:
            // await LocationService.ShareCurrentLocation();
            Console.WriteLine("✅ Ubicación compartida con el cliente");
        }

        private async Task UpdateRoute()
        {
            Console.WriteLine("🔄 Actualizando ruta...");

            // Simular actualización
            await Task.Delay(800);

            // En producción:
            // await NavigationService.RecalculateRoute();

            currentDelivery.NextTurn = "Av. Reforma, 200m";
            currentDelivery.EstimatedTime = "11 min";

            StateHasChanged();
            Console.WriteLine("✅ Ruta actualizada");
        }

        private void ToggleBattery()
        {
            Console.WriteLine("🔋 Información de batería");
            // Mostrar estado de batería del dispositivo
        }

        // Data Model
        private class DeliveryMapData
        {
            public string OrderId { get; set; } = string.Empty;
            public string CustomerName { get; set; } = string.Empty;
            public string CustomerAvatar { get; set; } = string.Empty;
            public string EstimatedTime { get; set; } = string.Empty;
            public string Distance { get; set; } = string.Empty;
            public string NextTurn { get; set; } = string.Empty;
            public string MapImageUrl { get; set; } = string.Empty;
        }
    }
}
