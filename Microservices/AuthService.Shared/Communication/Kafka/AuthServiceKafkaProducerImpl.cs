using AuthService.Shared.Enums.AuthService.Shared.Communication.Kafka;
using AuthService.Shared.Interfaces.Communication.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.BaseClasses.Communication.Kafka;
using Shared.Configurations;

namespace AuthService.Shared.Communication.Kafka
{
    public class AuthServiceKafkaProducerImpl : BaseKafkaProducer, IAuthServiceKafkaProducer
    {
        public AuthServiceKafkaProducerImpl(ILogger<AuthServiceKafkaProducerImpl> logger, IOptions<KafkaSettings> kafkaSettings)
            : base(logger, kafkaSettings.Value) { }

        public async Task ForceDeleteAspNetUserAsync(string userId) =>
            await SendKafkaMessageAsync(AuthServiceKafkaTopic.ASPNETUSER_FORCE_DELETE.ToString(), userId);

        public async Task CompleteCreationAspNetUserAsync(string userId) =>
            await SendKafkaMessageAsync(AuthServiceKafkaTopic.ASPNETUSER_COMPLETE_CREATION.ToString(), userId);
    }
}
