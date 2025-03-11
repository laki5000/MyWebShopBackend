using Microsoft.EntityFrameworkCore;
using ProductService.App.Configurations;
using ProductService.App.Data;
using ProductService.App.Interfaces.Repositories;
using ProductService.App.Interfaces.Services;
using ProductService.App.Mapping;
using ProductService.App.Repositories;
using ProductService.App.Services;

namespace ProductService.App.Extensions
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
            services.AddDbContext<ProductDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("PostgresConnection")));

            // Services
            services.AddScoped<ICategoryService, CategoryServiceImpl>();
            services.AddScoped<IProductService, ProductServiceImpl>();

            // Repositories
            services.AddScoped<ICategoryRepository, CategoryRepositoryImpl>();
            services.AddScoped<IProductRepository, ProductRepositoryImpl>();

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
