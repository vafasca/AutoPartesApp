using AutoPartesApp.Core.Application.DTOs.UserDTOs;
using AutoPartesApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.Users
{
    public class SearchUsersUseCase
    {
        private readonly IUserRepository _userRepository;

        public SearchUsersUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<UserListItemDto>> ExecuteAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return new List<UserListItemDto>();
            }

            var users = await _userRepository.SearchAsync(query);

            return users.Select(u => new UserListItemDto
            {
                Id = u.Id,
                Email = u.Email,
                FullName = u.FullName,
                Phone = u.Phone,
                Role = u.Role,
                RoleType = u.RoleType,
                AvatarUrl = u.AvatarUrl,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                LastLoginAt = u.LastLoginAt,
                City = u.AddressCity,
                Country = u.AddressCountry,
                TotalOrders = u.Orders?.Count ?? 0,
                TotalDeliveries = 0
            }).ToList();
        }
    }
}
