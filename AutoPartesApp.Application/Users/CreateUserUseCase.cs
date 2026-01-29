using AutoPartesApp.Core.Application.DTOs.UserDTOs;
using AutoPartesApp.Domain.Entities;
using AutoPartesApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.Users
{
    public class CreateUserUseCase
    {
        private readonly IUserRepository _userRepository;

        public CreateUserUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDetailDto?> ExecuteAsync(CreateUserDto dto)
        {
            // Validar que el email no exista
            var emailExists = await _userRepository.EmailExistsAsync(dto.Email);
            if (emailExists)
            {
                throw new InvalidOperationException("El email ya está registrado en el sistema");
            }

            // Crear entidad User
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = HashPassword(dto.Password), // ⚠️ En producción usar BCrypt
                Phone = dto.Phone,
                RoleType = dto.RoleType,
                AvatarUrl = dto.AvatarUrl,
                IsActive = true,
                AddressStreet = dto.AddressStreet,
                AddressCity = dto.AddressCity,
                AddressState = dto.AddressState,
                AddressCountry = dto.AddressCountry,
                AddressZipCode = dto.AddressZipCode,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Guardar en BD
            var createdUser = await _userRepository.CreateAsync(user);

            // Mapear a DTO
            return new UserDetailDto
            {
                Id = createdUser.Id,
                Email = createdUser.Email,
                FullName = createdUser.FullName,
                Phone = createdUser.Phone,
                Role = createdUser.Role,
                RoleType = createdUser.RoleType,
                AvatarUrl = createdUser.AvatarUrl,
                IsActive = createdUser.IsActive,
                AddressStreet = createdUser.AddressStreet,
                AddressCity = createdUser.AddressCity,
                AddressState = createdUser.AddressState,
                AddressCountry = createdUser.AddressCountry,
                AddressZipCode = createdUser.AddressZipCode,
                CreatedAt = createdUser.CreatedAt,
                UpdatedAt = createdUser.UpdatedAt,
                LastLoginAt = createdUser.LastLoginAt
            };
        }

        // ⚠️ TEMPORAL - En producción implementar con BCrypt.Net-Next
        private string HashPassword(string password)
        {
            // TODO: Implementar BCrypt
            return password; // SOLO PARA DESARROLLO
        }
    }
}
