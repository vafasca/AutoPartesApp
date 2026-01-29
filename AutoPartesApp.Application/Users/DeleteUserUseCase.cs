using AutoPartesApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.Users
{
    public class DeleteUserUseCase
    {
        private readonly IUserRepository _userRepository;

        public DeleteUserUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Eliminación lógica: marca IsActive = false
        /// </summary>
        public async Task<bool> ExecuteAsync(string userId)
        {
            return await _userRepository.DeleteAsync(userId);
        }
    }
}
