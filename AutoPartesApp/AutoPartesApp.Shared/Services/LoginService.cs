using AutoPartesApp.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;
using AutoPartesApp.Core.Application.Auth;

namespace AutoPartesApp.Shared.Services
{
    public class LoginService
    {
        private readonly LoginUseCase _loginUseCase;
        private readonly AuthState _authState;

        public LoginService(LoginUseCase loginUseCase, AuthState authState)
        {
            _loginUseCase = loginUseCase;
            _authState = authState;
        }

        public async Task<LoginViewModel> LoginAsync(string email, string password)
        {
            try
            {
                // Validaciones básicas
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    return new LoginViewModel
                    {
                        Email = email,
                        IsAuthenticated = false,
                        ErrorMessage = "Email y contraseña son requeridos"
                    };
                }

                // Ejecutar caso de uso de login
                var user = await _loginUseCase.Execute(email, password);

                if (user != null)
                {
                    // Actualizar estado de autenticación
                    _authState.SetUser(user);

                    return new LoginViewModel
                    {
                        Email = user.Email,
                        IsAuthenticated = true,
                        Role = user.Role,
                        FullName = user.FullName,
                        UserId = user.Id
                    };
                }
                else
                {
                    return new LoginViewModel
                    {
                        Email = email,
                        IsAuthenticated = false,
                        ErrorMessage = "Email o contraseña incorrectos"
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LoginService error: {ex.Message}");
                return new LoginViewModel
                {
                    Email = email,
                    IsAuthenticated = false,
                    ErrorMessage = "Error de conexión. Intenta nuevamente."
                };
            }
        }

        public void Logout()
        {
            _authState.ClearUser();
        }

        public bool IsAuthenticated()
        {
            return _authState.IsAuthenticated;
        }

        public string? GetCurrentUserRole()
        {
            return _authState.CurrentUser?.Role;
        }

    }
}
