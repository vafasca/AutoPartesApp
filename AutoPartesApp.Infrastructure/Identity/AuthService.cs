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
            //MockAPI tiene un endpoint /users
            var users = await _http.GetFromJsonAsync<List<User>>("https://69691d6d69178471522ca1bb.mockapi.io/api/v1/Users");

            return users?.FirstOrDefault(u => u.Email == email /* y password simulado */);
        }

    }
}
