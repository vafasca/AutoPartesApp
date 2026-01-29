using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AutoPartesApp.Core.Application.DTOs.UserDTOs
{
    public class UpdateUserDto
    {
        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 200 caracteres")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [Phone(ErrorMessage = "El formato del teléfono no es válido")]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        // Avatar (opcional)
        [Url(ErrorMessage = "La URL del avatar no es válida")]
        [StringLength(500)]
        public string? AvatarUrl { get; set; }

        // Dirección (opcional)
        [StringLength(200)]
        public string? AddressStreet { get; set; }

        [StringLength(100)]
        public string? AddressCity { get; set; }

        [StringLength(100)]
        public string? AddressState { get; set; }

        [StringLength(100)]
        public string? AddressCountry { get; set; }

        [StringLength(20)]
        public string? AddressZipCode { get; set; }
    }
}
