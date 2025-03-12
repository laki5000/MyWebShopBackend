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

        protected async Task SendKafkaMessageAsync(string topic, string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                _logger.LogError("Invalid message: {Message}", message);
                throw new ArgumentException("Message cannot be null or empty.", nameof(message));
            }

            try
            {
                var messageObject = new Message<string, string>
                {
                    Value = message
                };

                var deliveryResult = await _producer.ProduceAsync(topic, messageObject);

                _logger.LogInformation("Successfully sent '{Topic}' message to Kafka. Message: {Message}", topic, message);
            }
            catch (ProduceException<string, string> ex)
            {
                _logger.LogError(ex, "Error producing message to Kafka: {Topic}", topic);
                throw new Exception("Error producing message to Kafka", ex);
            }
        }
    }
}
