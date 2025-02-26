using Microsoft.EntityFrameworkCore;
using UserService.App.Communication.Grpc.Server;
using UserService.App.Configurations;
using UserService.App.Data;
using UserService.App.Interfaces.Repositories;
using UserService.App.Interfaces.Services;
using UserService.App.Mapping;
using UserService.App.Repositories;
using UserService.App.Services;

var builder = WebApplication.CreateBuilder(args);

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
    services.AddDbContext<UserDbContext>(options =>
        options.UseNpgsql(configuration.GetConnectionString("PostgresConnection")));

    // Services
    services.AddScoped<IUserService, UserServiceImpl>();

    // Repositories
    services.AddScoped<IUserRepository, UserRepositoryImpl>();

    // AutoMapper
    services.AddAutoMapper(typeof(MappingProfile).Assembly);

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
    app.MapGrpcService<UserServiceUserImpl>();

    // HealthCheck
    app.MapHealthChecks("/health");
}

void ApplyMigrations(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();

    dbContext.Database.Migrate();
}