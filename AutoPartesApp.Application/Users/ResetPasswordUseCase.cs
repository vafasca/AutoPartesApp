using AutoPartesApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.Users
{
    public class ResetPasswordUseCase
    {
        private readonly IUserRepository _userRepository;

        public ResetPasswordUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Restablece la contraseña del usuario a una temporal
        /// </summary>
        /// <returns>Contraseña temporal generada</returns>
        public async Task<string?> ExecuteAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            // Generar contraseña temporal
            var temporaryPassword = GenerateTemporaryPassword();

            // Actualizar password (⚠️ en producción usar BCrypt)
            user.PasswordHash = HashPassword(temporaryPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            return temporaryPassword;
        }

        private string GenerateTemporaryPassword()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 10)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // ⚠️ TEMPORAL - En producción implementar con BCrypt.Net-Next
        private string HashPassword(string password)
        {
            // TODO: Implementar BCrypt
            return password; // SOLO PARA DESARROLLO
        }
    }
}
