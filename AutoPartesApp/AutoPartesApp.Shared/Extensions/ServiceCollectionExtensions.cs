using AutoPartesApp.Core.Application.Auth;
using AutoPartesApp.Core.Application.Inventory;
using AutoPartesApp.Domain.Interfaces;
using AutoPartesApp.Infrastructure.Identity;
using AutoPartesApp.Infrastructure.Persistence.Repositories;
using AutoPartesApp.Shared.Services;
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

            // ✅ Repositorios
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            // ✅ Servicios de Infraestructura
            services.AddScoped<IAuthService, AuthService>();

            // ✅ Casos de Uso de Aplicación
            services.AddScoped<LoginUseCase>();
            services.AddScoped<GetLowStockUseCase>();
            services.AddScoped<CreateProductUseCase>();
            services.AddScoped<UpdateProductUseCase>();

            // ✅ Servicios de Presentación
            services.AddScoped<AuthState>();
            services.AddScoped<LoginService>();

            return services;
        }
    }
}
