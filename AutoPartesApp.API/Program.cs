using AutoPartesApp.Core.Application.Admin.Dashboard;
using AutoPartesApp.Core.Application.Inventory;
using AutoPartesApp.Core.Application.Orders;
using AutoPartesApp.Core.Application.Reports;
using AutoPartesApp.Core.Application.Users;
using AutoPartesApp.Domain.Interfaces;
using AutoPartesApp.Infrastructure.Persistence;
using AutoPartesApp.Infrastructure.Persistence.Repositories;
using AutoPartesApp.Infrastructure.Persistence.Seed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ========================================
// CONFIGURACIÓN DE SERVICIOS
// ========================================

// Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AutoPartes API",
        Version = "v1",
        Description = "API REST para sistema de venta de autopartes",
        Contact = new OpenApiContact
        {
            Name = "Tu Nombre",
            Email = "tu@email.com"
        }
    });

    // Configuración JWT en Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header. Ejemplo: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Database (PostgreSQL)
builder.Services.AddDbContext<AutoPartesDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("AutoPartesApp.Infrastructure")
    ));

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// Use Cases
builder.Services.AddScoped<GetLowStockUseCase>();
builder.Services.AddScoped<CreateProductUseCase>();
builder.Services.AddScoped<UpdateProductUseCase>();

// Use Cases - Inventory
builder.Services.AddScoped<GetAllProductsUseCase>();
builder.Services.AddScoped<GetProductByIdUseCase>();
builder.Services.AddScoped<CreateProductUseCase>();
builder.Services.AddScoped<UpdateProductUseCase>();
builder.Services.AddScoped<UpdateProductStockUseCase>();
builder.Services.AddScoped<ToggleProductStatusUseCase>();
builder.Services.AddScoped<GetLowStockUseCase>();
builder.Services.AddScoped<SearchProductsUseCase>();
builder.Services.AddScoped<GetInventoryStatsUseCase>();

// Use Cases - Admin
builder.Services.AddScoped<GetAdminDashboardUseCase>();

// Use Cases - Orders
//builder.Services.AddScoped<AssignDeliveryUseCase>();
//builder.Services.AddScoped<ChangeOrderStatusUseCase>();
//builder.Services.AddScoped<CreateOrderUseCase>();

// Use Cases - Reports
//builder.Services.AddScoped<GetSalesReportUseCase>();

// Use Cases - Users
//builder.Services.AddScoped<BlockUserUseCase>();
//builder.Services.AddScoped<GetCustomersUseCase>();

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey no configurada");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secretKey)
            ),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// CORS - IMPORTANTE para Blazor
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
    {
        policy.WithOrigins(
            "https://localhost:7192",  // Puerto CORRECTO de tu Blazor Web
            "http://localhost:5192",   // Puerto HTTP alternativo
            "https://localhost:7159",  // Mantener por si cambias de puerto
            "http://localhost:5159"    // Puerto HTTP alternativo
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

var app = builder.Build();

// ========================================
// CONFIGURACIÓN DEL PIPELINE
// ========================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AutoPartes API v1");
        c.RoutePrefix = string.Empty; // Swagger en la raíz
    });
}

app.UseHttpsRedirection();

// IMPORTANTE: CORS debe ir antes de Authentication/Authorization
app.UseCors("AllowBlazor");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

//Llamada al seeder
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AutoPartesDbContext>();
    await DatabaseSeeder.SeedAsync(db);
}

app.Run();