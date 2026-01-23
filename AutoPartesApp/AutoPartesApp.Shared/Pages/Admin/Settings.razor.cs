using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Shared.Pages.Admin
{
    public partial class Settings : ComponentBase
    {
        [Inject]
        private NavigationManager? NavigationManager { get; set; }

        // State
        private bool hasNotifications = true;
        private bool hasUnsavedChanges = false;

        // Admin Profile
        private AdminProfile adminProfile = new();

        // Global Parameters
        private string selectedCurrency = "USD";
        private int taxRate = 16;
        private string selectedLanguage = "Español";

        // Integrations
        private bool gpsEnabled = true;
        private bool emailNotificationsEnabled = true;

        // Security
        private bool twoFactorEnabled = false;
        private bool autoBackupEnabled = true;
        private string lastBackupDate = "Hace 2 días";

        // App Info
        private string appVersion = "2.4.1";

        protected override void OnInitialized()
        {
            LoadAdminProfile();
        }

        private void LoadAdminProfile()
        {
            adminProfile = new AdminProfile
            {
                Name = "Administrador Principal",
                Email = "admin@autopartes.com",
                AvatarUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuBslnV9ATU0dHL4iaC2HI4Pmmtlp542k6EjZ9V3ARR7I_Bz8iahOE_IlFgeWLbVUZ7hJffDWqZMOncyBXkiI872Yc97xP_b6v1diJoBuQC17b-leGr8S9vHqJPk20Dzs5h-M85j6AuTpxjQfN8U8ecW7iZ8WkgwyPilbN-2aWEZ3Ywg_ZfHVZsU9AlwErwMdh6l9rJ2eddt7UY9nvojAMvw9ZQZmWXLXyt8BkzWNZA_7EdbCwyNFmbM0iaf52K-5lFCE3JvERosV6k"
            };
        }

        // Event Handlers
        private void ToggleNotifications()
        {
            hasNotifications = false;
            StateHasChanged();
        }

        private void GoBack()
        {
            if (hasUnsavedChanges)
            {
                // Mostrar confirmación antes de salir
                Console.WriteLine("⚠️ Tienes cambios sin guardar");
            }
            NavigationManager?.NavigateTo("/admin/dashboard");
        }

        // Global Parameters Actions
        private void OpenCurrencySelector()
        {
            Console.WriteLine("💱 Abrir selector de moneda");
            // Implementar modal o navegación a selector
            // NavigationManager?.NavigateTo("/admin/settings/currency");
        }

        private void OpenTaxEditor()
        {
            Console.WriteLine("📊 Editar tasa de impuestos");
            // Implementar modal de edición
            // Podría ser un modal con input numérico
        }

        private void OpenLanguageSelector()
        {
            Console.WriteLine("🌐 Abrir selector de idioma");
            // Implementar modal con lista de idiomas disponibles
        }

        // Roles and Permissions Actions
        private void ManageRoles()
        {
            Console.WriteLine("👥 Administrar roles");
            NavigationManager?.NavigateTo("/admin/settings/roles");
        }

        private void ManageSellerPermissions()
        {
            Console.WriteLine("🏪 Administrar permisos de vendedor");
            NavigationManager?.NavigateTo("/admin/settings/seller-permissions");
        }

        // Integrations Toggle Actions
        private void ToggleGPS()
        {
            gpsEnabled = !gpsEnabled;
            hasUnsavedChanges = true;
            Console.WriteLine($"📍 GPS: {(gpsEnabled ? "Activado" : "Desactivado")}");
            StateHasChanged();
        }

        private void ManagePaymentGateways()
        {
            Console.WriteLine("💳 Administrar pasarelas de pago");
            NavigationManager?.NavigateTo("/admin/settings/payment-gateways");
        }

        private void ToggleEmailNotifications()
        {
            emailNotificationsEnabled = !emailNotificationsEnabled;
            hasUnsavedChanges = true;
            Console.WriteLine($"📧 Notificaciones Email: {(emailNotificationsEnabled ? "Activadas" : "Desactivadas")}");
            StateHasChanged();
        }

        // Security Actions
        private void ToggleTwoFactor()
        {
            twoFactorEnabled = !twoFactorEnabled;
            hasUnsavedChanges = true;
            Console.WriteLine($"🔐 2FA: {(twoFactorEnabled ? "Activado" : "Desactivado")}");

            if (twoFactorEnabled)
            {
                // Aquí se mostraría un modal para configurar 2FA
                Console.WriteLine("Mostrar configuración de 2FA");
            }

            StateHasChanged();
        }

        private void ToggleAutoBackup()
        {
            autoBackupEnabled = !autoBackupEnabled;
            hasUnsavedChanges = true;
            Console.WriteLine($"💾 Backup Automático: {(autoBackupEnabled ? "Activado" : "Desactivado")}");
            StateHasChanged();
        }

        private async Task CreateBackup()
        {
            Console.WriteLine("📦 Creando backup manual...");

            // Simular proceso de backup
            // Mostrar indicador de carga
            // En producción, esto llamaría a un servicio de backup

            await Task.Delay(2000); // Simular proceso

            lastBackupDate = "Hace unos momentos";
            Console.WriteLine("✅ Backup creado exitosamente");

            // Mostrar notificación de éxito
            StateHasChanged();
        }

        // Main Actions
        private async Task SaveChanges()
        {
            Console.WriteLine("💾 Guardando cambios...");

            // Simular guardado
            await Task.Delay(1000);

            // En producción:
            // var settings = new GlobalSettings
            // {
            //     Currency = selectedCurrency,
            //     TaxRate = taxRate,
            //     Language = selectedLanguage,
            //     GpsEnabled = gpsEnabled,
            //     EmailNotificationsEnabled = emailNotificationsEnabled,
            //     TwoFactorEnabled = twoFactorEnabled,
            //     AutoBackupEnabled = autoBackupEnabled
            // };
            // await SettingsService.SaveAsync(settings);

            hasUnsavedChanges = false;
            Console.WriteLine("✅ Cambios guardados exitosamente");

            // Mostrar notificación toast
            StateHasChanged();
        }

        private void ConfigureSecurity()
        {
            Console.WriteLine("🔒 Configurar seguridad avanzada");
            NavigationManager?.NavigateTo("/admin/settings/security");
        }

        // UI Helpers
        private string GetToggleClass(bool isEnabled)
        {
            return isEnabled
                ? "flex items-center h-6 w-12 rounded-full bg-primary cursor-pointer transition-colors relative"
                : "flex items-center h-6 w-12 rounded-full bg-slate-300 dark:bg-slate-600 cursor-pointer transition-colors relative";
        }

        private string GetToggleButtonClass(bool isEnabled)
        {
            return isEnabled
                ? "absolute right-0.5 top-0.5 size-5 bg-white rounded-full shadow-sm transition-all"
                : "absolute left-0.5 top-0.5 size-5 bg-white rounded-full shadow-sm transition-all";
        }

        // Data Models
        private class AdminProfile
        {
            public string Name { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string AvatarUrl { get; set; } = string.Empty;
        }
    }
}
