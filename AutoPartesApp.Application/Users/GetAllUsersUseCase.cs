using AutoPartesApp.Core.Application.DTOs.Common;
using AutoPartesApp.Core.Application.DTOs.UserDTOs;
using AutoPartesApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.Users
{
    public class GetAllUsersUseCase
    {
        private readonly IUserRepository _userRepository;

        public GetAllUsersUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<PagedResultDto<UserListItemDto>> ExecuteAsync(UserFilterDto filters)
        {
            // Obtener usuarios paginados con filtros
            var (users, totalCount) = await _userRepository.GetPagedAsync(
                searchQuery: filters.SearchQuery,
                roleType: filters.RoleType,
                isActive: filters.IsActive,
                createdFrom: filters.CreatedFrom,
                createdTo: filters.CreatedTo,
                pageNumber: filters.PageNumber,
                pageSize: filters.PageSize
            );

            // Mapear a DTO
            var userDtos = users.Select(u => new UserListItemDto
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
                TotalDeliveries = 0 // Se calculará en el próximo método específico
            }).ToList();

            return new PagedResultDto<UserListItemDto>
            {
                Items = userDtos,
                TotalCount = totalCount,
                PageNumber = filters.PageNumber,
                PageSize = filters.PageSize
                // TotalPages se calcula automáticamente
            };
        }
    }
}
