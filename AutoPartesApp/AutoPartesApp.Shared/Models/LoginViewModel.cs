using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Shared.Models
{
    public class LoginViewModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool IsAuthenticated { get; set; }
        public string Role { get; set; } = string.Empty;

    }
}
