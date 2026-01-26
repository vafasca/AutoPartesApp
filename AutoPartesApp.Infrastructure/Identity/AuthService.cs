using AutoPartesApp.Domain.Entities;
using AutoPartesApp.Domain.Enums;
using AutoPartesApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace AutoPartesApp.Infrastructure.Identity
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _http;

        public AuthService(HttpClient http)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
            Console.WriteLine($"✅ AuthService BaseAddress: {_http.BaseAddress}");
        }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            try
            {
                var loginRequest = new
                {
                    Email = email,
                    Password = password
                };

                var url = "api/Auth/login";
                Console.WriteLine($"🔵 Llamando a: {_http.BaseAddress}{url}");

                var response = await _http.PostAsJsonAsync(url, loginRequest);
                Console.WriteLine($"📡 Status Code: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Error Response: {errorContent}");
                    return null;
                }

                var loginResponse = await response.Content.ReadFromJsonAsync<LoginApiResponse>();

                if (loginResponse?.Success == true && loginResponse.User != null)
                {
                    Console.WriteLine($"✅ Login exitoso: {loginResponse.User.FullName}");

                    return new User
                    {
                        Id = loginResponse.User.Id,
                        Email = loginResponse.User.Email,
                        FullName = loginResponse.User.FullName,
                        RoleType = loginResponse.User.RoleType,
                        Phone = loginResponse.User.Phone,
                        AvatarUrl = loginResponse.User.AvatarUrl,
                        IsActive = loginResponse.User.IsActive,
                        LastLoginAt = DateTime.UtcNow
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ AuthService Exception: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return null;
            }
        }

        private class LoginApiResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
            public UserDto? User { get; set; }
        }

        private class UserDto
        {
            public string Id { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string FullName { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty;
            public RoleType RoleType { get; set; }
            public string Phone { get; set; } = string.Empty;
            public string? AvatarUrl { get; set; }
            public bool IsActive { get; set; }
        }


    }
}
