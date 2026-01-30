using AutoPartesApp.Shared.Services;
using AutoPartesApp.Web.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using AutoPartesApp.Core.Application.Auth;
using AutoPartesApp.Domain.Interfaces;
using AutoPartesApp.Shared.Extensions;
using AutoPartesApp.Infrastructure.Identity;
using AutoPartesApp.Shared.Services.Admin;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Servicios específicos del dispositivo
builder.Services.AddSingleton<IFormFactor, FormFactor>();

// ✅ HttpClient principal con BaseAddress CORRECTA
builder.Services.AddHttpClient("AutoPartesAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7120/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// ✅ AuthService con HttpClient configurado
builder.Services.AddScoped<IAuthService, AuthService>(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient("AutoPartesAPI");
    return new AuthService(httpClient);
});

// ✅ DashboardService con HttpClient configurado
builder.Services.AddScoped<DashboardService>(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    return new DashboardService(httpClientFactory);
});

// ✅ InventoryService con HttpClient configurado
builder.Services.AddScoped<InventoryService>(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient("AutoPartesAPI");
    var logger = sp.GetRequiredService<ILogger<InventoryService>>();
    return new InventoryService(httpClient, logger);
});

// ✅ UserManagementService con HttpClient configurado
builder.Services.AddScoped<UserManagementService>(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient("AutoPartesAPI");
    return new UserManagementService(httpClient);
});

// Casos de uso y servicios
builder.Services.AddScoped<LoginUseCase>();
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<AuthState>();
// ========== REPORTES Y EXPORTACIÓN ==========
builder.Services.AddScoped<ReportService>(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient("AutoPartesAPI");
    return new ReportService(httpClient);
});

builder.Services.AddScoped<ExportService>();


// Extensión común para Web
builder.Services.AddAutoPartesWebServices();

await builder.Build().RunAsync();