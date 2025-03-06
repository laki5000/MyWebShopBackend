using AuthService.App.Communication.Kafka;
using AuthService.App.Interfaces.Services;
using AuthService.App.Services;
using AuthService.Configurations;
using AuthService.Data;
using AuthService.Interfaces.Services;
using AuthService.Mapping;
using AuthService.Models;
using AuthService.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthService.App.Extensions
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
            services.AddDbContext<AuthDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("PostgresConnection")));

            // Identity
            services.AddIdentityCore<AspNetUser>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 0;
                options.Password.RequiredUniqueChars = 0;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AuthDbContext>()
            .AddDefaultTokenProviders();

            // Services & repositories
            services.AddScoped<IAspNetUserService, AspNetUserServiceImpl>();
            services.AddScoped<IRoleService, RoleServiceImpl>();
            services.AddSingleton<IJwtService, JwtServiceImpl>();

            // AutoMapper
            services.AddAutoMapper(typeof(MappingProfile).Assembly);

            // Data Protection
            services.AddDataProtection();

            // HealthCheck
            services.AddHealthChecks();

            // Grpc
            services.AddGrpc();

            // Kafka
            services.AddSingleton<KafkaTopicManager>();
            services.AddHostedService<KafkaConsumerImpl>();
        }
    }
}
