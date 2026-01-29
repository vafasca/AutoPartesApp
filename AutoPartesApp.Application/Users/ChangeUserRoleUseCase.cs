using AutoPartesApp.Core.Application.DTOs.UserDTOs;
using AutoPartesApp.Domain.Enums;
using AutoPartesApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.Users
{
    public class ChangeUserRoleUseCase
    {
        private readonly IUserRepository _userRepository;

        public ChangeUserRoleUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> ExecuteAsync(string userId, ChangeRoleDto dto)
        {
            // Verificar que el usuario exista
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            // Validación de negocio: no permitir cambiar rol de Admin
            if (user.RoleType == RoleType.Admin)
            {
                throw new InvalidOperationException("No se puede cambiar el rol de un administrador");
            }

            // Validación: no permitir cambiar a Admin
            if (dto.NewRole == RoleType.Admin)
            {
                throw new InvalidOperationException("No se puede asignar el rol de administrador desde esta función");
            }

            // Cambiar rol
            return await _userRepository.ChangeUserRoleAsync(userId, dto.NewRole);
        }
    }
}
