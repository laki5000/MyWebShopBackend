using AuthService.Configurations;
using AuthService.Interfaces.Services;
using AuthService.Shared.Communication.Kafka;
using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace AuthService.App.Communication.Kafka
{
    public class KafkaConsumerImpl : BackgroundService
    {
        private readonly ILogger<KafkaConsumerImpl> _logger;
        private readonly IConsumer<string, string> _consumer;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        private readonly List<string> _topics = new List<string>
        {
            AuthServiceKafkaTopics.AspNetUserForceDelete,
        };

        private readonly string _authServiceGroup = "auth-service-group";

        public KafkaConsumerImpl(
            ILogger<KafkaConsumerImpl> logger,
            IOptions<AppSettings> appSettings, 
            IServiceScopeFactory serviceScopeFactory
        )
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _consumer = CreateConsumer(appSettings);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return StartConsumerLoop(stoppingToken);
        }

        private IConsumer<string, string> CreateConsumer(IOptions<AppSettings> appSettings)
        {
            var kafkaSettings = appSettings.Value.KafkaSettings;
            var autoOffsetReset = Enum.Parse<AutoOffsetReset>(kafkaSettings.AutoOffsetReset);
            var config = new ConsumerConfig
            {
                BootstrapServers = kafkaSettings.BootstrapServers,
                GroupId = _authServiceGroup,
                AutoOffsetReset = autoOffsetReset,
            };

            var consumer = new ConsumerBuilder<string, string>(config).Build();
            consumer.Subscribe(_topics);

            return consumer;
        }

        private Task StartConsumerLoop(CancellationToken stoppingToken)
        {
            return Task.Run(() =>
            {
                try
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

                            _ = Task.Run(() => ProcessResultAsync(consumeResult), stoppingToken);
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

        private async Task ProcessResultAsync(ConsumeResult<string, string> result)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                try
                {
                    switch (result.Topic)
                    {
                        case AuthServiceKafkaTopics.AspNetUserForceDelete:
                            await HandleAspNetUserForceDeleteAsync(scope, result);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error processing message: {ex.Message}");
                }
            }
        }

        private async Task HandleAspNetUserForceDeleteAsync(IServiceScope scope, ConsumeResult<string, string> result)
        {
            var topic = result.Topic;
            var message = result.Message.Value;

            try
            {
                var aspNetUserService = scope.ServiceProvider.GetRequiredService<IAspNetUserService>();
                
                //todo 

                _logger.LogInformation("Successfully processed '{Topic}' message for user ID: {UserId}", topic, message);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error handling {Topic} message for user ID: {UserId}. Exception: {ExceptionMessage}", topic, message, ex.Message);
            }
        }
    }
}
