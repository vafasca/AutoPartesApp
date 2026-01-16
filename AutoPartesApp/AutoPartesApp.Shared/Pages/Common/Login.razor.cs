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
        private bool showError = false;

        private string GetPasswordInputType() => ShowPassword ? "text" : "password";
        private string GetVisibilityIcon() => ShowPassword ? "visibility_off" : "visibility";
        private void TogglePasswordVisibility() => ShowPassword = !ShowPassword;

        private async Task HandleLogin()
        {
            Console.WriteLine($"Login attempt with email: {viewModel.Email}");

            if (string.IsNullOrWhiteSpace(viewModel.Email) || string.IsNullOrWhiteSpace(viewModel.Password))
            {
                Console.WriteLine("Email and password are required");
                showError = true;
                return;
            }

            try
            {
                viewModel = await LoginService!.LoginAsync(viewModel.Email, viewModel.Password);

                if (viewModel.IsAuthenticated)
                {
                    Console.WriteLine($"Login successful. Role: {viewModel.Role}");

                    switch (viewModel.Role)
                    {
                        case "Admin":
                            NavigationManager?.NavigateTo("/Counter");
                            break;
                        case "Delivery":
                            NavigationManager?.NavigateTo("/Home");
                            break;
                        default:
                            NavigationManager?.NavigateTo("Weather");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid credentials or user not found");
                    showError = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
                showError = true;
            }
        }

        private void ForgotPassword()
        {
            Console.WriteLine("Forgot password clicked");
            NavigationManager?.NavigateTo("/forgot-password");
        }

        private void CreateAccount()
        {
            Console.WriteLine("Create account clicked");
            NavigationManager?.NavigateTo("/register");
        }

        private void ExploreAsGuest()
        {
            Console.WriteLine("Explore as guest clicked");
            NavigationManager?.NavigateTo("/home");
        }

        private void GoBack()
        {
            Console.WriteLine("Go back clicked");
            NavigationManager?.NavigateTo("/");
        }
    }
}