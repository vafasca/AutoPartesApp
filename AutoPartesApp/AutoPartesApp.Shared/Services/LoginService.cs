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

        public LoginService(LoginUseCase loginUseCase)
        {
            _loginUseCase = loginUseCase;
        }

        public async Task<LoginViewModel> LoginAsync(string email, string password)
        {
            var user = await _loginUseCase.Execute(email, password);

            return new LoginViewModel
            {
                Email = email,
                IsAuthenticated = user != null,
                Role = user?.Role ?? string.Empty
            };
        }

    }
}
