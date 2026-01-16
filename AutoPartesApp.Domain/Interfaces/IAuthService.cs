using AutoPartesApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Domain.Interfaces
{
    public interface IAuthService
    {
        Task<User?> AuthenticateAsync(string email, string password);

    }
}
