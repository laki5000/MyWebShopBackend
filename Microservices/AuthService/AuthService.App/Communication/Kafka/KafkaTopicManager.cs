using Confluent.Kafka.Admin;
using Confluent.Kafka;
using AuthService.Configurations;
using Microsoft.Extensions.Options;

namespace AuthService.App.Communication.Kafka
{
    public class KafkaTopicManager
    {
        private readonly IAdminClient _adminClient;

        private readonly List<string> _topicsToCreate = new List<string>
        {
            "aspnetuser-force-delete"
        };

        public KafkaTopicManager(IOptions<AppSettings> _appSettings)
        {
            var kafkaSettings = _appSettings.Value.KafkaSettings;
            var config = new AdminClientConfig
            {
                BootstrapServers = kafkaSettings.BootstrapServers
            };

            _adminClient = new AdminClientBuilder(config).Build();
        }

        public async Task EnsureTopicsExistAsync()
        {
            foreach (var topicName in _topicsToCreate)
            {
                bool exists = TopicExistsAsync(topicName);

                if (!exists)
                {
                    await CreateTopicAsync(topicName);
                }
            }
        }

        private bool TopicExistsAsync(string topicName)
        {
            try
            {
                var metadata = _adminClient.GetMetadata(TimeSpan.FromSeconds(10));

                return metadata.Topics.Exists(topic => topic.Topic == topicName);

            }
            catch (Exception ex)
            {
                throw;
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
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
