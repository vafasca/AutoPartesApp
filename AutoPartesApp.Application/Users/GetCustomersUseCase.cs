using AutoPartesApp.Core.Application.DTOs.UserDTOs;
using AutoPartesApp.Domain.Enums;
using AutoPartesApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.Users
{
    public class GetCustomersUseCase
    {
        private readonly IUserRepository _userRepository;

        public GetCustomersUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Obtiene solo usuarios con rol Cliente con sus estadísticas
        /// </summary>
        public async Task<List<UserListItemDto>> ExecuteAsync()
        {
            var customers = await _userRepository.GetByRoleAsync(RoleType.Client);

            return customers.Select(c => new UserListItemDto
            {
                Id = c.Id,
                Email = c.Email,
                FullName = c.FullName,
                Phone = c.Phone,
                Role = c.Role,
                RoleType = c.RoleType,
                AvatarUrl = c.AvatarUrl,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                LastLoginAt = c.LastLoginAt,
                City = c.AddressCity,
                Country = c.AddressCountry,
                TotalOrders = c.Orders?.Count ?? 0,
                TotalDeliveries = 0
            }).ToList();
        }
    }
}
