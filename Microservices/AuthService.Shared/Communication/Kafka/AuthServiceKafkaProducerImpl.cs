using AuthService.Shared.Enums.AuthService.Shared.Communication.Kafka;
using AuthService.Shared.Interfaces.Communication.Kafka;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Configurations;

namespace AuthService.Shared.Communication.Kafka
{
    public class AuthServiceKafkaProducerImpl : IAuthServiceKafkaProducer
    {
        private readonly ILogger<AuthServiceKafkaProducerImpl> _logger;
        private readonly IProducer<string, string> _producer;

        public AuthServiceKafkaProducerImpl(ILogger<AuthServiceKafkaProducerImpl> logger, IOptions<KafkaSettings> kafkaSettings)
        {
            _logger = logger;
            _producer = CreateKafkaProducer(kafkaSettings.Value);
        }

        private IProducer<string, string> CreateKafkaProducer(KafkaSettings kafkaSettings)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = kafkaSettings.BootstrapServers
            };

            return new ProducerBuilder<string, string>(config).Build();
        }

        private async Task SendKafkaMessageAsync(AuthServiceKafkaTopic topic, string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogError("Invalid userId: {UserId}", userId);
                throw new ArgumentException("UserId cannot be null or empty.", nameof(userId));
            }

            try
            {
                var message = new Message<string, string>
                {
                    Key = "userId",
                    Value = userId
                };

                var topicName = topic.ToString();
                var deliveryResult = await _producer.ProduceAsync(topicName, message);

                _logger.LogInformation("Successfully sent '{Topic}' message to Kafka for user ID: {UserId}", topicName, userId);
            }
            catch (ProduceException<string, string> ex)
            {
                _logger.LogError(ex, "Error producing message to Kafka: {Topic}", topic);
                throw new Exception("Error producing message to Kafka", ex);
            }
        }

        public async Task ForceDeleteAspNetUserAsync(string userId) =>
            await SendKafkaMessageAsync(AuthServiceKafkaTopic.ASPNETUSER_FORCE_DELETE, userId);

        public async Task CompleteCreationAspNetUserAsync(string userId) =>
            await SendKafkaMessageAsync(AuthServiceKafkaTopic.ASPNETUSER_COMPLETE_CREATION, userId);
    }
}
