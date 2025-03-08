using AuthService.App.Communication.Grpc;
using AuthService.App.Communication.Kafka;
using AuthService.Data;
using AuthService.Shared.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthService.App.Extensions
{
    public static class ApplicationExtensions
    {
        public static void ConfigureEndpoints(this WebApplication app)
        {
            app.MapGrpcService<AuthServiceUserImpl>();
            app.MapGrpcService<AuthServiceRoleImpl>();
            app.MapHealthChecks("/health");
        }

        public static void ApplyDatabaseMigrations(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();

            dbContext.Database.Migrate();
        }

        public static void ApplyRoleInitialization(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

            if (roleManager == null)
            {
                throw new ArgumentNullException(nameof(roleManager));
            }

            foreach (Role role in Enum.GetValues(typeof(Role)))
            {
                var roleExist = roleManager.RoleExistsAsync(role.ToString()).GetAwaiter().GetResult();
                if (!roleExist)
                {
                    var identityRole = new IdentityRole(role.ToString());

                    roleManager.CreateAsync(identityRole).GetAwaiter().GetResult();
                }
            }
        }

        public static void ApplyKafkaTopicCreation(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var kafkaTopicManager = scope.ServiceProvider.GetRequiredService<KafkaTopicManager>();

            kafkaTopicManager.EnsureTopicsExistAsync().GetAwaiter().GetResult();
        }
    }
}
