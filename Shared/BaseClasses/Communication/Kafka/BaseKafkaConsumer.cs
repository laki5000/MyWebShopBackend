using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Configurations;

namespace Shared.BaseClasses.Communication.Kafka
{
    public abstract class BaseKafkaConsumer : BackgroundService
    {
        protected readonly ILogger _logger;
        private readonly IConsumer<string, string> _consumer;
        private readonly string _groupId;

        protected BaseKafkaConsumer(ILogger logger, KafkaSettings kafkaSettings, string groupId)
        {
            _logger = logger;
            _groupId = groupId;
            _consumer = CreateConsumer(kafkaSettings);
        }

        private IConsumer<string, string> CreateConsumer(KafkaSettings kafkaSettings)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = kafkaSettings.BootstrapServers,
                GroupId = _groupId,
                AutoOffsetReset = Enum.Parse<AutoOffsetReset>(kafkaSettings.AutoOffsetReset)
            };

            var consumer = new ConsumerBuilder<string, string>(config).Build();
            consumer.Subscribe(GetTopics());
            return consumer;
        }

        protected abstract string[] GetTopics();
        protected abstract Task ProcessMessageAsync(ConsumeResult<string, string> result);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Run(async () =>
            {
                try
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        try
                        {
                            var consumeResult = _consumer.Consume(stoppingToken);
                            if (!consumeResult.IsPartitionEOF)
                            {
                                await ProcessMessageAsync(consumeResult);
                            }
                        }
                        catch (ConsumeException ex)
                        {
                            _logger.LogError($"Error consuming message: {ex.Error.Reason}");
                        }
                    }
                }
                finally
                {
                    _consumer.Close();
                }
            }, stoppingToken);
        }
    }
}
