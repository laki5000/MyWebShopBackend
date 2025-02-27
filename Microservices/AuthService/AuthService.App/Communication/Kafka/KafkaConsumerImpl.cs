using AuthService.Configurations;
using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace AuthService.App.Communication.Kafka
{
    public class KafkaConsumerImpl : BackgroundService
    {
        private readonly IConsumer<string, string> _consumer;

        public KafkaConsumerImpl(IOptions<AppSettings> appSettings)
        {
            var kafkaSettings = appSettings.Value.KafkaSettings ?? throw new ArgumentNullException(nameof(appSettings.Value.KafkaSettings));
            var autoOffSetReset = Enum.Parse<AutoOffsetReset>(kafkaSettings.AutoOffsetReset);
            var config = new ConsumerConfig
            {
                BootstrapServers = kafkaSettings.BootstrapServers,
                GroupId = "auth-service-group",
                AutoOffsetReset = autoOffSetReset,
            };
            var topics = new[] { "aspnetuser-force-delete" };

            _consumer = new ConsumerBuilder<string, string>(config).Build();
            _consumer.Subscribe(topics);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return StartConsumerLoop(stoppingToken);
        }

        private async Task StartConsumerLoop(CancellationToken stoppingToken)
        {
            try
            {
                await Task.Run(() =>
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        try
                        {
                            var consumeResult = _consumer.Consume(stoppingToken);

                            if (consumeResult.IsPartitionEOF)
                            {
                                continue;
                            }

                            var message = consumeResult.Message.Value;

                            // Üzenet feldolgozás implementálása
                        }
                        catch (ConsumeException ex)
                        {
                        }
                    }
                }, stoppingToken);
            }
            finally
            {
                _consumer.Close();
            }
        }
    }
}
