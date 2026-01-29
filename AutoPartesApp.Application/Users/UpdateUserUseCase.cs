using AutoPartesApp.Core.Application.DTOs.UserDTOs;
using AutoPartesApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.Users
{
    public class UpdateUserUseCase
    {
        private readonly IUserRepository _userRepository;

        public UpdateUserUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDetailDto?> ExecuteAsync(string userId, UpdateUserDto dto)
        {
            // Verificar que el usuario exista
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            // Validar que el email no esté en uso por otro usuario
            var emailExists = await _userRepository.EmailExistsAsync(dto.Email, userId);
            if (emailExists)
            {
                throw new InvalidOperationException("El email ya está registrado por otro usuario");
            }

            // Actualizar campos
            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.Phone = dto.Phone;
            user.AvatarUrl = dto.AvatarUrl;
            user.AddressStreet = dto.AddressStreet;
            user.AddressCity = dto.AddressCity;
            user.AddressState = dto.AddressState;
            user.AddressCountry = dto.AddressCountry;
            user.AddressZipCode = dto.AddressZipCode;
            user.UpdatedAt = DateTime.UtcNow;

            // Guardar cambios
            var updatedUser = await _userRepository.UpdateAsync(user);

            // Mapear a DTO
            return new UserDetailDto
            {
                Id = updatedUser.Id,
                Email = updatedUser.Email,
                FullName = updatedUser.FullName,
                Phone = updatedUser.Phone,
                Role = updatedUser.Role,
                RoleType = updatedUser.RoleType,
                AvatarUrl = updatedUser.AvatarUrl,
                IsActive = updatedUser.IsActive,
                AddressStreet = updatedUser.AddressStreet,
                AddressCity = updatedUser.AddressCity,
                AddressState = updatedUser.AddressState,
                AddressCountry = updatedUser.AddressCountry,
                AddressZipCode = updatedUser.AddressZipCode,
                CreatedAt = updatedUser.CreatedAt,
                UpdatedAt = updatedUser.UpdatedAt,
                LastLoginAt = updatedUser.LastLoginAt
            };
        }
    }
}
