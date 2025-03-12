using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Shared.Configurations;

namespace Shared.BaseClasses.Communication.Kafka
{
    public abstract class BaseKafkaProducer
    {
        private readonly ILogger _logger;
        private readonly IProducer<string, string> _producer;

        public BaseKafkaProducer(ILogger logger, KafkaSettings kafkaSettings)
        {
            _logger = logger;
            _producer = CreateKafkaProducer(kafkaSettings);
        }

        private IProducer<string, string> CreateKafkaProducer(KafkaSettings kafkaSettings)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = kafkaSettings.BootstrapServers
            };

            return new ProducerBuilder<string, string>(config).Build();
        }

        protected async Task SendKafkaMessageAsync(string topic, string userId)
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

                var deliveryResult = await _producer.ProduceAsync(topic, message);

                _logger.LogInformation("Successfully sent '{Topic}' message to Kafka for user ID: {UserId}", topic, userId);
            }
            catch (ProduceException<string, string> ex)
            {
                _logger.LogError(ex, "Error producing message to Kafka: {Topic}", topic);
                throw new Exception("Error producing message to Kafka", ex);
            }
        }
    }
}
