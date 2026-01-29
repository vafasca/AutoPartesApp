using AutoPartesApp.Core.Application.DTOs.UserDTOs;
using AutoPartesApp.Domain.Enums;
using AutoPartesApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.Users
{
    public class GetDeliveriesUsersUseCase
    {
        private readonly IUserRepository _userRepository;

        public GetDeliveriesUsersUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Obtiene solo usuarios con rol Repartidor con sus estadísticas
        /// </summary>
        public async Task<List<UserListItemDto>> ExecuteAsync()
        {
            var deliveries = await _userRepository.GetByRoleAsync(RoleType.Delivery);

            return deliveries.Select(d => new UserListItemDto
            {
                Id = d.Id,
                Email = d.Email,
                FullName = d.FullName,
                Phone = d.Phone,
                Role = d.Role,
                RoleType = d.RoleType,
                AvatarUrl = d.AvatarUrl,
                IsActive = d.IsActive,
                CreatedAt = d.CreatedAt,
                LastLoginAt = d.LastLoginAt,
                City = d.AddressCity,
                Country = d.AddressCountry,
                TotalOrders = 0,
                TotalDeliveries = d.Orders?.Count(o => o.Delivery != null) ?? 0
            }).ToList();
        }
    }
}
