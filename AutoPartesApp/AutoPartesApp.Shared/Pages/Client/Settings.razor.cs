using AutoPartesApp.Shared.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Shared.Pages.Client
{
    public partial class Settings : ComponentBase
    {
        [Inject]
        private NavigationManager? NavigationManager { get; set; }

        [Inject]
        private AuthState? AuthState { get; set; }

        [Inject]
        private LoginService? LoginService { get; set; }

        // User Profile Data
        private UserProfileData userProfile = new();

        // Vehicles
        private List<VehicleData> vehicles = new();

        // Settings State
        private bool biometricEnabled = false;
        private bool darkModeEnabled = true;
        private string notificationMode = "PUSH";
        private string selectedLanguage = "Español";
        private string appVersion = "2.4.1";

        protected override void OnInitialized()
        {
            LoadUserProfile();
            LoadVehicles();
            LoadSettings();
        }

        private void LoadUserProfile()
        {
            // Obtener datos del usuario autenticado
            if (AuthState?.CurrentUser != null)
            {
                userProfile = new UserProfileData
                {
                    FullName = AuthState.CurrentUser.FullName,
                    Email = AuthState.CurrentUser.Email,
                    Phone = "+52 555 987 6543", // En producción vendría de la BD
                    Address = "Av. Insurgentes Sur 123, CDMX",
                    AvatarUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuACGke-rMUeps7tvyyICeutizS6lEB3BiO1vU0Ey2bPrzC3BSapZA7UizvP6eVKjlI5SU2uOYYS6W2WyiJdAdBloJVoLfCUhRvdm-6Zfsp4ouYUlOpw9cg-_pMFnFP8NaHuLH3Q58HJH-8U60RgYUM487tZsNUPqXLnlRMoaiRsXn2JrAxrYajV3mIMWeiMdhamdiq8TDYxv8hucOxaCzsJvpTQDak3pAYfjC_S-T4p3Hw__7cLxcFBkMuV2sqkzhuGWVvsT_YrAdI",
                    TotalOrders = 28,
                    SavedVehicles = 1,
                    FavoriteProducts = 12
                };
            }
            else
            {
                // Datos por defecto si no hay usuario
                userProfile = new UserProfileData
                {
                    FullName = "Carlos Rodríguez",
                    Email = "carlos.rod@autopartes.com",
                    Phone = "+52 555 987 6543",
                    Address = "Av. Insurgentes Sur 123, CDMX",
                    AvatarUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuACGke-rMUeps7tvyyICeutizS6lEB3BiO1vU0Ey2bPrzC3BSapZA7UizvP6eVKjlI5SU2uOYYS6W2WyiJdAdBloJVoLfCUhRvdm-6Zfsp4ouYUlOpw9cg-_pMFnFP8NaHuLH3Q58HJH-8U60RgYUM487tZsNUPqXLnlRMoaiRsXn2JrAxrYajV3mIMWeiMdhamdiq8TDYxv8hucOxaCzsJvpTQDak3pAYfjC_S-T4p3Hw__7cLxcFBkMuV2sqkzhuGWVvsT_YrAdI",
                    TotalOrders = 28,
                    SavedVehicles = 1,
                    FavoriteProducts = 12
                };
            }
        }

        private void LoadVehicles()
        {
            vehicles = new List<VehicleData>
            {
                new VehicleData
                {
                    Id = 1,
                    Name = "Toyota Corolla 2022",
                    Type = "2.0L Sedán",
                    IsPrimary = true
                }
            };
        }

        private void LoadSettings()
        {
            // En producción, cargar desde preferencias guardadas
            biometricEnabled = false;
            darkModeEnabled = true; // Detectar del sistema o preferencia guardada
            notificationMode = "PUSH";
            selectedLanguage = "Español";
        }

        // Profile Actions
        private void ChangeAvatar()
        {
            Console.WriteLine("Cambiar avatar");
            // Implementar selector de imagen
        }

        private void EditProfile()
        {
            Console.WriteLine("Editar perfil completo");
            // NavigationManager?.NavigateTo("/client/edit-profile");
        }

        private void EditName()
        {
            Console.WriteLine($"Editar nombre: {userProfile.FullName}");
            // Mostrar modal o navegar a edición
        }

        private void EditPhone()
        {
            Console.WriteLine($"Editar teléfono: {userProfile.Phone}");
            // Mostrar modal o navegar a edición
        }

        private void EditAddress()
        {
            Console.WriteLine($"Editar dirección: {userProfile.Address}");
            // Mostrar modal o navegar a edición
        }

        // Vehicle Actions
        private void AddVehicle()
        {
            Console.WriteLine("Agregar nuevo vehículo");
            // NavigationManager?.NavigateTo("/client/add-vehicle");
        }

        private void EditVehicle(int vehicleId)
        {
            Console.WriteLine($"Editar vehículo ID: {vehicleId}");
            // NavigationManager?.NavigateTo($"/client/edit-vehicle/{vehicleId}");
        }

        // Security Actions
        private void ChangePassword()
        {
            Console.WriteLine("Cambiar contraseña");
            // NavigationManager?.NavigateTo("/client/change-password");
        }

        private void ToggleBiometric()
        {
            biometricEnabled = !biometricEnabled;
            Console.WriteLine($"Acceso biométrico: {(biometricEnabled ? "Activado" : "Desactivado")}");

            // En producción:
            // await BiometricService.SetEnabled(biometricEnabled);
            StateHasChanged();
        }

        // Preferences Actions
        private void ToggleDarkMode()
        {
            darkModeEnabled = !darkModeEnabled;
            Console.WriteLine($"Modo oscuro: {(darkModeEnabled ? "Activado" : "Desactivado")}");

            // Implementar toggle de dark mode
            // En producción:
            // await ThemeService.SetDarkMode(darkModeEnabled);
            // if (darkModeEnabled)
            //     document.documentElement.classList.add('dark');
            // else
            //     document.documentElement.classList.remove('dark');

            StateHasChanged();
        }

        private void ManageNotifications()
        {
            Console.WriteLine("Gestionar notificaciones");
            // NavigationManager?.NavigateTo("/client/notifications");
        }

        private void ChangeLanguage()
        {
            Console.WriteLine("Cambiar idioma");
            // Mostrar selector de idioma
            // En producción:
            // await LocalizationService.ShowLanguageSelector();
        }

        // Support Actions
        private void ContactSupport()
        {
            Console.WriteLine("Contactar soporte");
            // NavigationManager?.NavigateTo("/client/support");
        }

        private void ViewFAQ()
        {
            Console.WriteLine("Ver preguntas frecuentes");
            // NavigationManager?.NavigateTo("/client/faq");
        }

        // Save Settings
        private async Task SaveSettings()
        {
            Console.WriteLine("Guardando configuración...");

            // Simular guardado
            await Task.Delay(500);

            // En producción:
            // var settings = new UserSettings
            // {
            //     BiometricEnabled = biometricEnabled,
            //     DarkModeEnabled = darkModeEnabled,
            //     NotificationMode = notificationMode,
            //     Language = selectedLanguage
            // };
            // await SettingsService.SaveAsync(settings);

            Console.WriteLine("✅ Configuración guardada");

            // Mostrar toast o mensaje de confirmación
        }

        // Logout
        private async Task HandleLogout()
        {
            Console.WriteLine("🚪 Cerrando sesión...");

            // Confirmar antes de cerrar sesión
            // En producción, mostrar un modal de confirmación

            try
            {
                // Limpiar estado de autenticación
                LoginService?.Logout();

                // Navegar al login
                NavigationManager?.NavigateTo("/login", forceLoad: true);

                Console.WriteLine("✅ Sesión cerrada exitosamente");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al cerrar sesión: {ex.Message}");
            }
        }

        // Navigation
        private void GoBack()
        {
            NavigationManager?.NavigateTo("/client/dashboard");
        }

        // Data Models
        private class UserProfileData
        {
            public string FullName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Phone { get; set; } = string.Empty;
            public string Address { get; set; } = string.Empty;
            public string AvatarUrl { get; set; } = string.Empty;
            public int TotalOrders { get; set; }
            public int SavedVehicles { get; set; }
            public int FavoriteProducts { get; set; }
        }

        private class VehicleData
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty;
            public bool IsPrimary { get; set; }
        }
    }
}
