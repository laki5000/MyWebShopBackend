using AuthService.Configurations;
using AuthService.Interfaces.Services;
using AuthService.Shared.Enums.AuthService.Shared.Communication.Kafka;
using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace AuthService.App.Communication.Kafka
{
    public class KafkaConsumerImpl : BackgroundService
    {
        private readonly ILogger<KafkaConsumerImpl> _logger;
        private readonly IConsumer<string, string> _consumer;
        private readonly IServiceScopeFactory _serviceScopeFactory;

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
            var topics = Enum.GetValues(typeof(AuthServiceKafkaTopic))
                 .Cast<AuthServiceKafkaTopic>()
                 .Select(topic => topic.ToString());

            consumer.Subscribe(topics);

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

                            Task.Run(() => ProcessResultAsync(consumeResult), stoppingToken);
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
                        case var topic when topic == AuthServiceKafkaTopic.ASPNETUSER_FORCE_DELETE.ToString():
                            await HandleAspNetUserForceDeleteAsync(scope, result);
                            break;
                        case var topic when topic == AuthServiceKafkaTopic.ASPNETUSER_COMPLETE_CREATION.ToString():
                            await HandleAspNetUserCompleteCreationAsync(scope, result);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error processing message: {ex.Message}");
                }
            }
        }

        private void LogProcessingResult(string topic, string userId, Exception? ex = null)
        {
            if (ex == null)
            {
                _logger.LogInformation("Successfully processed '{Topic}' message for user ID: {UserId}", topic, userId);
            }
            else
            {
                _logger.LogError("Error handling {Topic} message for user ID: {UserId}. Exception: {ExceptionMessage}", topic, userId, ex.Message);
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

                LogProcessingResult(topic, message);
            }
            catch (Exception ex)
            {
                LogProcessingResult(topic, message, ex);
            }
        }

        private async Task HandleAspNetUserCompleteCreationAsync(IServiceScope scope, ConsumeResult<string, string> result)
        {
            var topic = result.Topic;
            var message = result.Message.Value;

            try
            {
                var aspNetUserService = scope.ServiceProvider.GetRequiredService<IAspNetUserService>();

                //todo 

                LogProcessingResult(topic, message);
            }
            catch (Exception ex)
            {
                LogProcessingResult(topic, message, ex);
            }
        }
    }
}
