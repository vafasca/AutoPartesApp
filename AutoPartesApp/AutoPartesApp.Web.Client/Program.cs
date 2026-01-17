using AutoPartesApp.Shared.Services;
using AutoPartesApp.Web.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using AutoPartesApp.Core.Application.Auth;
using AutoPartesApp.Domain.Interfaces;
using AutoPartesApp.Shared.Extensions;
using AutoPartesApp.Infrastructure.Identity;




var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add device-specific services used by the AutoPartesApp.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();

// HttpClient para AuthService
builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
{
    client.BaseAddress = new Uri("https://69691d6d69178471522ca1bb.mockapi.io/api/v1/");
});

// Casos de uso y servicios que se inyectan en componentes Shared
builder.Services.AddScoped<LoginUseCase>();
builder.Services.AddScoped<LoginService>();

// Aquí agregas TODOS los servicios que tus componentes Shared usan
//builder.Services.AddScoped<PedidosService>();
//builder.Services.AddScoped<CatalogoService>();
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<AuthState>();

// … cualquier otro servicio que esté en AutoPartesApp.Shared.Services

// Extensiones comunes
builder.Services.AddAutoPartesServices();


await builder.Build().RunAsync();
