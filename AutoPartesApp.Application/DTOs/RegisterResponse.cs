using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.DTOs
{
    public class RegisterResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? UserId { get; set; }
    }
}
