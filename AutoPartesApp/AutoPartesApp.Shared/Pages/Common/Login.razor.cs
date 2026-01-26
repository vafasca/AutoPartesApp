using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;
using AutoPartesApp.Shared.Models;
using AutoPartesApp.Shared.Services;

namespace AutoPartesApp.Shared.Pages.Common
{
    public partial class Login : ComponentBase
    {
        [Inject]
        private NavigationManager? NavigationManager { get; set; }

        [Inject]
        private LoginService? LoginService { get; set; }

        private LoginViewModel viewModel = new();
        private bool ShowPassword { get; set; } = false;
        private bool isLoading = false;
        private string errorMessage = string.Empty;

        private string GetPasswordInputType() => ShowPassword ? "text" : "password";
        private string GetVisibilityIcon() => ShowPassword ? "visibility_off" : "visibility";
        private void TogglePasswordVisibility() => ShowPassword = !ShowPassword;

        private async Task HandleLogin()
        {
            // Limpiar errores previos
            errorMessage = string.Empty;
            isLoading = true;

            try
            {
                // Validación en el cliente
                if (string.IsNullOrWhiteSpace(viewModel.Email))
                {
                    errorMessage = "El email es requerido";
                    return;
                }

                if (string.IsNullOrWhiteSpace(viewModel.Password))
                {
                    errorMessage = "La contraseña es requerida";
                    return;
                }

                if (!IsValidEmail(viewModel.Email))
                {
                    errorMessage = "El formato del email no es válido";
                    return;
                }

                // Intentar login
                var result = await LoginService!.LoginAsync(viewModel.Email, viewModel.Password);

                if (result.IsAuthenticated)
                {
                    Console.WriteLine($"✅ Login exitoso - Usuario: {result.FullName}, Rol: {result.Role}");

                    // Redirigir según el rol
                    RedirectByRole(result.Role);
                }
                else
                {
                    errorMessage = result.ErrorMessage ?? "Credenciales inválidas";
                    Console.WriteLine($"❌ Login fallido: {errorMessage}");
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Error de conexión. Por favor intenta nuevamente.";
                Console.WriteLine($"❌ Error en HandleLogin: {ex.Message}");
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }

        private void RedirectByRole(string role)
        {
            var route = role.ToLower() switch
            {
                "admin" or "administrador" => "/admin/dashboard",
                "delivery" or "repartidor" => "/delivery/dashboard",
                "client" or "cliente" => "/client/dashboard",
                _ => "/login"
            };

            Console.WriteLine($"Redirigiendo a: {route}");
            NavigationManager?.NavigateTo(route, forceLoad: true);
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void ForgotPassword()
        {
            Console.WriteLine("Recuperar contraseña");
            NavigationManager?.NavigateTo("/forgot-password");
        }

        private void CreateAccount()
        {
            Console.WriteLine("Crear cuenta nueva");
            NavigationManager?.NavigateTo("/register");
        }

        private void ExploreAsGuest()
        {
            Console.WriteLine("Explorar como invitado");
            NavigationManager?.NavigateTo("/catalog");
        }

        private void GoBack()
        {
            Console.WriteLine("Volver atrás");
            NavigationManager?.NavigateTo("/");
        }
    }
}