using AutoPartesApp.Domain.Entities;
using AutoPartesApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Shared.Services
{
    public class AuthState
    {
        public User? CurrentUser { get; private set; }
        public bool IsAuthenticated => CurrentUser != null;
        public RoleType? UserRole => CurrentUser?.RoleType;

        public event Action? OnAuthStateChanged;

        public void SetUser(User? user)
        {
            CurrentUser = user;
            if (user != null)
            {
                user.LastLoginAt = DateTime.UtcNow;
            }
            OnAuthStateChanged?.Invoke();
        }

        public void ClearUser()
        {
            CurrentUser = null;
            OnAuthStateChanged?.Invoke();
        }

        public bool HasRole(RoleType role)
        {
            return IsAuthenticated && CurrentUser?.RoleType == role;
        }

        public bool HasAnyRole(params RoleType[] roles)
        {
            return IsAuthenticated && roles.Contains(CurrentUser!.RoleType);
        }
    }
}
