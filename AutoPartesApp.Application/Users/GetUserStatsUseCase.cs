using AutoPartesApp.Core.Application.DTOs.UserDTOs;
using AutoPartesApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.Users
{
    public class GetUserStatsUseCase
    {
        private readonly IUserRepository _userRepository;

        public GetUserStatsUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserStatsDto> ExecuteAsync()
        {
            var stats = await _userRepository.GetUserStatsAsync();

            return new UserStatsDto
            {
                TotalUsers = stats.Total,
                ActiveUsers = stats.Active,
                InactiveUsers = stats.Inactive,
                TotalClients = stats.Clients,
                TotalDeliveries = stats.Deliveries,
                TotalAdmins = stats.Admins
            };
        }
    }
}
