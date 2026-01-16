using AutoPartesApp.Core.Application.Auth;
using AutoPartesApp.Domain.Interfaces;
using AutoPartesApp.Services;
using AutoPartesApp.Shared.Services;
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

            // Add device-specific services used by the AutoPartesApp.Shared project
            builder.Services.AddSingleton<IFormFactor, FormFactor>();

            builder.Services.AddMauiBlazorWebView();

            //added
            builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
            {
                client.BaseAddress = new Uri("https://69691d6d69178471522ca1bb.mockapi.io/api/v1/");
            });

            builder.Services.AddScoped<LoginUseCase>();
            builder.Services.AddScoped<LoginService>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
