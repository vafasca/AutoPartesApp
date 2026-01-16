using System;
using System.Collections.Generic;
using System.Text;
using AutoPartesApp.Domain.Entities;
using AutoPartesApp.Domain.Interfaces;

namespace AutoPartesApp.Core.Application.Auth
{
    public class LoginUseCase
    {
        private readonly IAuthService _authService;

        public LoginUseCase(IAuthService authService)
        {
            _authService = authService;
        }

        public Task<User?> Execute(string email, string password)
        {
            return _authService.AuthenticateAsync(email, password);
        }

    }
}
