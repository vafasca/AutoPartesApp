using AutoPartesApp.Core.Application.Auth;
using AutoPartesApp.Domain.Interfaces;
using AutoPartesApp.Services;
using AutoPartesApp.Shared.Services;
using AutoPartesApp.Shared.Extensions;
using Microsoft.Extensions.Logging;
using AutoPartesApp.Infrastructure.Identity;

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

            builder.Services.AddHttpClient("AutoPartesAPI", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7120/");
                client.Timeout = TimeSpan.FromSeconds(30);
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

            builder.Services.AddAutoPartesWebServices();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}