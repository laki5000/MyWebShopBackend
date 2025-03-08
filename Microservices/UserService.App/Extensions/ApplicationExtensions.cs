using Microsoft.EntityFrameworkCore;
using UserService.App.Communication.Grpc;
using UserService.App.Data;

namespace UserService.App.Extensions
{
    public static class ApplicationExtensions
    {
        public static void ConfigureEndpoints(this WebApplication app)
        {
            app.MapGrpcService<UserServiceUserImpl>();
            app.MapHealthChecks("/health");
        }

        public static void ApplyDatabaseMigrations(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();

            dbContext.Database.Migrate();
        }
    }
}
