using AuthService.Configurations;
using AuthService.Data;
using AuthService.GrpcServices;
using AuthService.Interfaces.Repositories;
using AuthService.Interfaces.Services;
using AuthService.Mapping;
using AuthService.Models;
using AuthService.Repositories;
using AuthService.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.jwt.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile("appsettings.grpc.json", optional: false, reloadOnChange: true);

ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

ConfigureEndpoints(app);
ApplyMigrations(app);

app.Run();

void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // App settings
    services.Configure<AppSettings>(configuration);

    // Database context
    services.AddDbContext<AuthDbContext>(options =>
        options.UseNpgsql(configuration.GetConnectionString("PostgresConnection")));

    // Services
    services.AddScoped<IAspNetUserService, AspNetUserServiceImpl>();

    services.AddSingleton<IJwtService, JwtServiceImpl>();

    // Repositories
    services.AddScoped<IAspNetUserRepository, AspNetUserRepositoryImpl>();

    // AutoMapper
    services.AddAutoMapper(typeof(MappingProfile).Assembly);

    // Identity
    services.AddIdentityCore<AspNetUser>()
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<AuthDbContext>()
        .AddDefaultTokenProviders();

    // Data Protection
    services.AddDataProtection();

    // HealthCheck
    services.AddHealthChecks();

    // Grpc
    services.AddGrpc();
}

void ConfigureEndpoints(WebApplication app)
{
    // Grpc
    app.MapGrpcService<AuthServiceUserImpl>();

    // HealthCheck
    app.MapHealthChecks("/health");
}

void ApplyMigrations(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();

    dbContext.Database.Migrate();
}
