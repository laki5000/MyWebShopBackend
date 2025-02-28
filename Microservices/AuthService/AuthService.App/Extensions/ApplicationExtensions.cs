using AuthService.App.Communication.Grpc;
using AuthService.App.Communication.Kafka;
using AuthService.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthService.App.Extensions
{
    public static class ApplicationExtensions
    {
        public static void ConfigureEndpoints(this WebApplication app)
        {
            app.MapGrpcService<AuthServiceUserImpl>();
            app.MapHealthChecks("/health");
        }

        public static void ApplyDatabaseMigrations(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();

            dbContext.Database.Migrate();
        }

        public static void ApplyKafkaTopicCreation(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var kafkaTopicManager = scope.ServiceProvider.GetRequiredService<KafkaTopicManager>();

            kafkaTopicManager.EnsureTopicsExistAsync().GetAwaiter().GetResult();
        }
    }
}
