using AutoPartesApp.Core.Application.Auth;
using AutoPartesApp.Domain.Interfaces;
using AutoPartesApp.Services;
using AutoPartesApp.Shared.Services;
using AutoPartesApp.Shared.Extensions;
using Microsoft.Extensions.Logging;
using AutoPartesApp.Infrastructure.Identity;
using AutoPartesApp.Shared.Services.Admin;

namespace AutoPartesApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddSingleton<IFormFactor, FormFactor>();
            builder.Services.AddMauiBlazorWebView();

            // ========== CONFIGURACIÓN DE API BASE URL ==========
            string apiBaseUrl = GetApiBaseUrl();

            builder.Services.AddHttpClient("AutoPartesAPI", client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
#if DEBUG
                // Solo en desarrollo: aceptar certificados auto-firmados
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };
                return handler;
#else
                return new HttpClientHandler();
#endif
            });

            builder.Services.AddScoped<IAuthService, AuthService>(sp =>
            {
                var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient("AutoPartesAPI");
                return new AuthService(httpClient);
            });

            // Casos de uso y servicios
            builder.Services.AddScoped<LoginUseCase>();
            builder.Services.AddScoped<LoginService>();
            builder.Services.AddScoped<AuthState>();
            builder.Services.AddScoped<DashboardService>();
            builder.Services.AddAutoPartesWebServices();

            builder.Services.AddScoped<UserManagementService>(sp =>
            {
                var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient("AutoPartesAPI");
                return new UserManagementService(httpClient);
            });

            // ========== SERVICIOS - ADMIN ==========
            builder.Services.AddScoped<InventoryService>(sp =>
            {
                var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient("AutoPartesAPI");
                var logger = sp.GetRequiredService<ILogger<InventoryService>>();
                return new InventoryService(httpClient, logger);
            });

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        /// <summary>
        /// Obtiene la URL base de la API según la plataforma
        /// </summary>
        private static string GetApiBaseUrl()
        {
#if DEBUG
#if ANDROID
            // Android Emulator: 10.0.2.2 apunta al localhost del host
            // Si usas dispositivo físico, cambia a tu IP local (ej: "https://192.168.1.100:7120/")
            return "https://10.0.2.2:7120/";
#elif IOS
            // iOS Simulator puede usar localhost
            return "https://localhost:7120/";
#else
            // Windows, Mac, etc. usan localhost normalmente
            return "https://localhost:7120/";
#endif
#else
            // En producción, usar la URL real de tu API
            return "https://api.autopartes.com/";
#endif
        }
    }
}