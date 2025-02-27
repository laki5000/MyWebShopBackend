using Confluent.Kafka.Admin;
using Confluent.Kafka;
using AuthService.Configurations;
using Microsoft.Extensions.Options;
using AuthService.Shared.Communication.Kafka;

namespace AuthService.App.Communication.Kafka
{
    public class KafkaTopicManager
    {
        private readonly ILogger<KafkaTopicManager> _logger;
        private readonly IAdminClient _adminClient;

        private readonly List<string> _topicsToCreate = new List<string>
        {
            AuthServiceKafkaTopics.AspNetUserForceDelete,
        };

        public KafkaTopicManager(ILogger<KafkaTopicManager> logger, IOptions<AppSettings> appSettings)
        {
            _logger = logger;
            _adminClient = CreateAdminClient(appSettings);
        }

        private IAdminClient CreateAdminClient(IOptions<AppSettings> appSettings)
        {
            var kafkaSettings = appSettings.Value.KafkaSettings;
            var config = new AdminClientConfig
            {
                BootstrapServers = kafkaSettings.BootstrapServers
            };

            return new AdminClientBuilder(config).Build();
        }

        public async Task EnsureTopicsExistAsync()
        {
            foreach (var topicName in _topicsToCreate)
            {
                var exists = TopicExists(topicName);
                if (!exists)
                {
                    await CreateTopicAsync(topicName);
                }
            }
        }

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
