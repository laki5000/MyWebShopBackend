using AuthService.Shared.Interfaces.Communication.Kafka;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Shared.Configurations;

namespace AuthService.Shared.Communication.Kafka
{
    public class AuthServiceKafkaProducerImpl : IAuthServiceKafkaProducer
    {
        private readonly IProducer<string, string> _producer;

        public AuthServiceKafkaProducerImpl(IOptions<KafkaSettings> kafkaSettings)
        {
            var bootstrapServers = kafkaSettings.Value.BootstrapServers 
                ?? throw new ArgumentNullException(nameof(kafkaSettings.Value.BootstrapServers));
            var config = new ProducerConfig
            {
                BootstrapServers = bootstrapServers
            };
            _producer = new ProducerBuilder<string, string>(config).Build();
        }

        public async Task ForceDeleteAspNetUserAsync(string userId)
        {
            await _producer.ProduceAsync("aspnetuser-force-delete", new Message<string, string>
            {
                Key = "userId",
                Value = userId
            });
        }
    }
}
