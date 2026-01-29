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
        /// <summary>
        /// Registra servicios comunes de AutoPartes para proyectos Web/MAUI.
        /// IMPORTANTE: Este método NO debe registrar HttpClient ni AuthService,
        /// ya que esos se configuran específicamente en cada proyecto.
        /// </summary>
        public static IServiceCollection AddAutoPartesWebServices(this IServiceCollection services)
        {
            //HttpClient y AuthService se configuran en Program.cs de cada proyecto
            // porque tienen configuraciones específicas (puerto, SSL, etc.)

            // Solo registrar servicios que no dependan de HttpClient
            // o que no tengan configuraciones específicas de plataforma

            // Ejemplo: Si tuvieras servicios de lógica pura, registrarlos aquí
            // services.AddScoped<CalculadoraService>();
            // services.AddScoped<ValidadorService>();

            return services;
        }


    }
}
