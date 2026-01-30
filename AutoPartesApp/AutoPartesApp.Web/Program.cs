using AutoPartesApp.Core.Application.Auth;
using AutoPartesApp.Domain.Interfaces;
using AutoPartesApp.Infrastructure.Identity;
using AutoPartesApp.Shared.Extensions;
using AutoPartesApp.Shared.Services;
using AutoPartesApp.Web.Components;
using AutoPartesApp.Web.Services;
using AutoPartesApp.Shared.Services.Admin;
var builder = WebApplication.CreateBuilder(args);
// Componentes interactivos de Blazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();
// Servicios específicos del dispositivo
builder.Services.AddSingleton<IFormFactor, FormFactor>();
//HttpClient con BaseAddress explícita
builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
{
    //API
    client.BaseAddress = new Uri("https://localhost:7120/");
    client.Timeout = TimeSpan.FromSeconds(30);
});
//HttpClient genérico para otros usos
builder.Services.AddHttpClient();
// Casos de uso y servicios de presentación
builder.Services.AddScoped<LoginUseCase>();
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<AuthState>();
builder.Services.AddScoped<DashboardService>();
// ========== REPORTES Y EXPORTACIÓN ==========
builder.Services.AddHttpClient("AutoPartesAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7120/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddScoped<ReportService>(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient("AutoPartesAPI");
    return new ReportService(httpClient);
});

builder.Services.AddScoped<ExportService>();

// Extensión común para Web (sin repositorios ni DbContext)
builder.Services.AddAutoPartesWebServices();
builder.Services.AddScoped<UserManagementService>(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient("AutoPartesAPI");
    return new UserManagementService(httpClient);
});
var app = builder.Build();
// Configuración del pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(
        typeof(AutoPartesApp.Shared._Imports).Assembly,
        typeof(AutoPartesApp.Web.Client._Imports).Assembly);
app.Run();