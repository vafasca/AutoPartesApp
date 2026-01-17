using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using AutoPartesApp.Domain.Entities;
using AutoPartesApp.Domain.Interfaces;

namespace AutoPartesApp.Infrastructure.Identity
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _http;

        public AuthService(HttpClient http)
        {
            _http = http;
        }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            try
            {
                // Obtener usuarios de la API Mock
                var users = await _http.GetFromJsonAsync<List<User>>(
                    "https://69691d6d69178471522ca1bb.mockapi.io/api/v1/Users"
                );

                if (users == null) return null;

                // Buscar usuario por email (en producción validarías el password hasheado)
                var user = users.FirstOrDefault(u =>
                    u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
                    u.IsActive
                );

                // Simulación de validación de password
                // En producción: compararías el hash del password
                // Por ahora, aceptamos cualquier password no vacío
                if (user != null && !string.IsNullOrWhiteSpace(password))
                {
                    return user;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AuthService error: {ex.Message}");
                return null;
            }
        }

    }
}
