using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Logging;
using Shared.Configurations;

namespace Shared.BaseClasses.Communication.Kafka
{
    public abstract class BaseKafkaTopicManager
    {
        protected readonly ILogger<BaseKafkaTopicManager> _logger;
        protected readonly IAdminClient _adminClient;

        public BaseKafkaTopicManager(ILogger<BaseKafkaTopicManager> logger, KafkaSettings kafkaSettings)
        {
            _logger = logger;
            _adminClient = CreateAdminClient(kafkaSettings);
        }

        protected IAdminClient CreateAdminClient(KafkaSettings kafkaSettings)
        {
            var config = new AdminClientConfig
            {
                BootstrapServers = kafkaSettings.BootstrapServers
            };

            return new AdminClientBuilder(config).Build();
        }

        public async Task EnsureTopicsExistAsync()
        {
            foreach (var topic in GetKafkaTopics())
            {
                var topicName = topic.ToString();
                var exists = TopicExists(topicName);
                if (!exists)
                {
                    await CreateTopicAsync(topicName);
                }
            }
        }

        protected abstract IEnumerable<Enum> GetKafkaTopics();

        private bool TopicExists(string topicName)
        {
            try
            {
                var metadata = _adminClient.GetMetadata(TimeSpan.FromSeconds(10));
                return metadata.Topics.Exists(topic => topic.Topic == topicName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if topic {Topic} exists", topicName);
                return false;
            }
        }

        private async Task CreateTopicAsync(string topicName)
        {
            try
            {
                var topicSpecification = new TopicSpecification
                {
                    Name = topicName,
                    ReplicationFactor = 1,
                    NumPartitions = 1
                };

                await _adminClient.CreateTopicsAsync([topicSpecification]);
                _logger.LogInformation("Successfully created Kafka topic: {Topic}", topicName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating topic {Topic}", topicName);
                throw new Exception($"Failed to create topic {topicName}", ex);
            }
        }
    }
}
