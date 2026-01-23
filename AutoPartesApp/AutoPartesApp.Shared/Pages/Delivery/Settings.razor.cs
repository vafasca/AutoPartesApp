using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Shared.Pages.Delivery
{
    public partial class Settings : ComponentBase
    {
        [Inject]
        private NavigationManager? NavigationManager { get; set; }

        // State
        private bool darkModeEnabled = true;
        private bool notificationsEnabled = true;
        private bool soundEnabled = true;
        private string selectedLanguage = "Español";

        // Data
        private DeliveryProfileData deliveryProfile = new();
        private List<DriverStat> driverStats = new();

        protected override void OnInitialized()
        {
            LoadDeliveryProfile();
            LoadDriverStats();
        }

        private void LoadDeliveryProfile()
        {
            deliveryProfile = new DeliveryProfileData
            {
                Name = "Juan Pérez",
                Phone = "+52 555-0123",
                Email = "juan.perez@delivery.com",
                Vehicle = "Moto Honda - ABC-123",
                AvatarUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuDaMIoNhxfqOeHQrzjKHoM99IFQbl_mMdcfvs2vSVT1Xo-M7eZz0RSTgNwK5ImH-_G6pSZm-4odF0z3bUfQfY1lxmcQ3jDlnpt_-t74c1epupiop-PCy_9N3d_9jo2u6TC48cPryDulSSvcbynN_SyOEX93qIqldxuNBZZDGAS76sTsT2oYazCWQRAFcX2dn8pIQRXD1Kb_GeeXjMwrqjc6t8MkMxDQvNOsWRT4DHwhbOp5djT3gMjPThzrWuE7G2gle6plvpE51Yo"
            };
        }

        private void LoadDriverStats()
        {
            driverStats = new List<DriverStat>
            {
                new DriverStat
                {
                    Label = "Entregas Hoy",
                    Value = "12",
                    Change = "+3 vs ayer",
                    ColorClass = "text-green-600 dark:text-green-400"
                },
                new DriverStat
                {
                    Label = "Calificación",
                    Value = "4.9",
                    Change = "⭐ Excelente",
                    ColorClass = "text-amber-600 dark:text-amber-400"
                },
                new DriverStat
                {
                    Label = "Esta Semana",
                    Value = "68",
                    Change = "+12%",
                    ColorClass = "text-blue-600 dark:text-blue-400"
                },
                new DriverStat
                {
                    Label = "Total Mes",
                    Value = "248",
                    Change = "Meta: 250",
                    ColorClass = "text-purple-600 dark:text-purple-400"
                }
            };
        }

        // UI Helpers - Toggle Classes
        private string GetToggleClass(bool isEnabled)
        {
            return isEnabled
                ? "relative inline-flex h-6 w-11 items-center rounded-full bg-primary transition-colors"
                : "relative inline-flex h-6 w-11 items-center rounded-full bg-slate-300 dark:bg-slate-700 transition-colors";
        }

        private string GetToggleKnobClass(bool isEnabled)
        {
            return isEnabled
                ? "translate-x-6 inline-block h-4 w-4 transform rounded-full bg-white transition-transform"
                : "translate-x-1 inline-block h-4 w-4 transform rounded-full bg-white transition-transform";
        }

        // Event Handlers - Edición de Datos
        private void EditName()
        {
            Console.WriteLine("✏️ Editando nombre");
            // Implementar modal o navegación a pantalla de edición
        }

        private void EditPhone()
        {
            Console.WriteLine("✏️ Editando teléfono");
        }

        private void EditEmail()
        {
            Console.WriteLine("✏️ Editando email");
        }

        private void EditVehicle()
        {
            Console.WriteLine("🏍️ Editando información del vehículo");
        }

        // Event Handlers - Preferencias
        private void ChangeLanguage()
        {
            selectedLanguage = selectedLanguage == "Español" ? "English" : "Español";
            Console.WriteLine($"🌐 Idioma cambiado a: {selectedLanguage}");
            StateHasChanged();
        }

        private void ToggleDarkMode()
        {
            darkModeEnabled = !darkModeEnabled;
            Console.WriteLine($"🌓 Modo oscuro: {(darkModeEnabled ? "Activado" : "Desactivado")}");
            StateHasChanged();
        }

        private void ToggleNotifications()
        {
            notificationsEnabled = !notificationsEnabled;
            Console.WriteLine($"🔔 Notificaciones: {(notificationsEnabled ? "Activadas" : "Desactivadas")}");
            StateHasChanged();
        }

        private void ToggleSound()
        {
            soundEnabled = !soundEnabled;
            Console.WriteLine($"🔊 Sonidos: {(soundEnabled ? "Activados" : "Desactivados")}");
            StateHasChanged();
        }

        // Event Handlers - Acciones
        private void ShowHelp()
        {
            Console.WriteLine("❓ Mostrando ayuda");
        }

        private async void UpdateProfile()
        {
            Console.WriteLine("💾 Guardando cambios...");
            await Task.Delay(1000);
            Console.WriteLine("✅ Cambios guardados exitosamente");
        }

        private void Logout()
        {
            Console.WriteLine("👋 Cerrando sesión...");
            NavigationManager?.NavigateTo("/login");
        }

        // Data Models
        private class DeliveryProfileData
        {
            public string Name { get; set; } = string.Empty;
            public string Phone { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Vehicle { get; set; } = string.Empty;
            public string AvatarUrl { get; set; } = string.Empty;
        }

        private class DriverStat
        {
            public string Label { get; set; } = string.Empty;
            public string Value { get; set; } = string.Empty;
            public string Change { get; set; } = string.Empty;
            public string ColorClass { get; set; } = string.Empty;
        }
    }
}
