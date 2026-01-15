using Microsoft.AspNetCore.Components;
using System;

namespace AutoPartesApp.Shared.Pages.Common
{
    public partial class Login : ComponentBase
    {
        [Inject]
        private NavigationManager? NavigationManager { get; set; }

        private string Email { get; set; } = string.Empty;
        private string Password { get; set; } = string.Empty;
        private bool ShowPassword { get; set; } = false;

        private string GetPasswordInputType()
        {
            return ShowPassword ? "text" : "password";
        }

        private string GetVisibilityIcon()
        {
            return ShowPassword ? "visibility_off" : "visibility";
        }

        private void TogglePasswordVisibility()
        {
            ShowPassword = !ShowPassword;
        }

        private void HandleLogin()
        {
            // Implementar lógica de inicio de sesión
            Console.WriteLine($"Login attempt with email: {Email}");

            // Ejemplo de validación básica
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                Console.WriteLine("Email and password are required");
                return;
            }

            // Aquí irá tu lógica de autenticación
            // Por ejemplo: await AuthService.LoginAsync(Email, Password);

            // Ejemplo de navegación después del login exitoso:
            // NavigationManager?.NavigateTo("/dashboard");
        }

        private void ForgotPassword()
        {
            // Implementar lógica de recuperación de contraseña
            Console.WriteLine("Forgot password clicked");
            // NavigationManager?.NavigateTo("/forgot-password");
        }

        private void CreateAccount()
        {
            // Navegar a página de registro
            Console.WriteLine("Create account clicked");
            // NavigationManager?.NavigateTo("/register");
        }

        private void ExploreAsGuest()
        {
            // Navegar como invitado
            Console.WriteLine("Explore as guest clicked");
            // NavigationManager?.NavigateTo("/home");
        }

        private void GoBack()
        {
            // Navegar hacia atrás
            Console.WriteLine("Go back clicked");
            // NavigationManager?.NavigateTo("/");
        }
    }
}