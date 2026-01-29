using AutoPartesApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AutoPartesApp.Core.Application.DTOs.UserDTOs
{
    public class ChangeRoleDto
    {
        [Required(ErrorMessage = "El nuevo rol es obligatorio")]
        public RoleType NewRole { get; set; }

        public string? Reason { get; set; }
    }
}
