using AutoPartesApp.Shared.Services;
using AutoPartesApp.Web.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using AutoPartesApp.Core.Application.Auth;
using AutoPartesApp.Domain.Interfaces;
using AutoPartesApp.Shared.Extensions;
using AutoPartesApp.Infrastructure.Identity;
using AutoPartesApp.Shared.Services.Admin;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

//Servicios específicos del dispositivo
builder.Services.AddSingleton<IFormFactor, FormFactor>();

//HttpClient para AuthService apuntando a tu API real
builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7120/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

//Casos de uso y servicios que se inyectan en componentes Shared
builder.Services.AddScoped<LoginUseCase>();
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<AuthState>();
builder.Services.AddScoped<DashboardService>();

// ========== SERVICIOS - ADMIN ==========
builder.Services.AddScoped<InventoryService>();

// ========== HTTP CLIENT ==========
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7001/") // Cambia por la URL real de tu API
});

// … aquí puedes agregar otros servicios de AutoPartesApp.Shared.Services
// builder.Services.AddScoped<PedidosService>();
// builder.Services.AddScoped<CatalogoService>();

//Extensión común para Web (sin repositorios ni DbContext)
builder.Services.AddAutoPartesWebServices();

await builder.Build().RunAsync();

