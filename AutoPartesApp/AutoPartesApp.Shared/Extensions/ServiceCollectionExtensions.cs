using AutoPartesApp.Core.Application.Auth;
using AutoPartesApp.Domain.Interfaces;
using AutoPartesApp.Shared.Services;
using AutoPartesApp.Infrastructure.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Shared.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAutoPartesServices(this IServiceCollection services)
        {
            // HttpClient para API
            services.AddHttpClient<IAuthService, AuthService>(client =>
            {
                client.BaseAddress = new Uri("https://69691d6d69178471522ca1bb.mockapi.io/api/v1/");
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            // Servicios de Infraestructura
            services.AddScoped<IAuthService, AuthService>();

            // Casos de Uso de Aplicación
            services.AddScoped<LoginUseCase>();

            // Servicios de Presentación
            services.AddScoped<AuthState>();
            services.AddScoped<LoginService>();

            return services;
        }
    }
}
