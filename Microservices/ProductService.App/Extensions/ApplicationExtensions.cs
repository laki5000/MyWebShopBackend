using Microsoft.EntityFrameworkCore;
using ProductService.App.Communication.Grpc;
using ProductService.App.Data;

namespace ProductService.App.Extensions
{
    public static class ApplicationExtensions
    {
        public static void ConfigureEndpoints(this WebApplication app)
        {
            app.MapGrpcService<ProductServiceCategoryImpl>();
            app.MapHealthChecks("/health");
        }

        public static void ApplyDatabaseMigrations(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ProductDbContext>();

            dbContext.Database.Migrate();
        }
    }
}
