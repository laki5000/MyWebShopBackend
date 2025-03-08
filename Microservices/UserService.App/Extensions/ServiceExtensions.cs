using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using UserService.App.Configurations;
using UserService.App.Data;
using UserService.App.Interfaces.Repositories;
using UserService.App.Interfaces.Services;
using UserService.App.Mapping;
using UserService.App.Repositories;
using UserService.App.Services;

namespace UserService.App.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureServices(this WebApplicationBuilder builder)
        {
            var services = builder.Services;
            var configuration = builder.Configuration;

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
    }
}
