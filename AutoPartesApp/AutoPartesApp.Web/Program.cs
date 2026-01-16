using AutoPartesApp.Core.Application.Auth;
using AutoPartesApp.Domain.Interfaces;
using AutoPartesApp.Shared.Services;
using AutoPartesApp.Web.Components;
using AutoPartesApp.Web.Services;
using AutoPartesApp.Infrastructure.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Add device-specific services used by the AutoPartesApp.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();

//added
builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
{
    client.BaseAddress = new Uri("https://69691d6d69178471522ca1bb.mockapi.io/api/v1/");
});

builder.Services.AddScoped<LoginUseCase>();
builder.Services.AddScoped<LoginService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
