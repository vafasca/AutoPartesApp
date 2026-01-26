using AutoPartesApp.Core.Application.Auth;
using AutoPartesApp.Core.Application.Inventory;
using AutoPartesApp.Domain.Interfaces;
using AutoPartesApp.Infrastructure.Identity;
using AutoPartesApp.Infrastructure.Persistence;
using AutoPartesApp.Infrastructure.Persistence.Repositories;
using AutoPartesApp.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Shared.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAutoPartesWebServices(this IServiceCollection services)
        {
            // ✅ CORRECCIÓN: Configurar HttpClient con nombre
            services.AddHttpClient("AutoPartesAPI", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7120/");
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            // ✅ Registrar AuthService manualmente con IHttpClientFactory
            services.AddScoped<IAuthService, AuthService>(sp =>
            {
                var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient("AutoPartesAPI");
                return new AuthService(httpClient);
            });

            services.AddScoped<LoginUseCase>();
            services.AddScoped<LoginService>();
            services.AddScoped<AuthState>();

            return services;
        }


    }
}
