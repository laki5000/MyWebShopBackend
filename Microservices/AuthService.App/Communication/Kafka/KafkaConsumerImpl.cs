using AuthService.Configurations;
using AuthService.Interfaces.Services;
using AuthService.Shared.Enums.AuthService.Shared.Communication.Kafka;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Shared.BaseClasses.Communication.Kafka;

namespace AuthService.App.Communication.Kafka
{
    public class KafkaConsumerImpl : BaseKafkaConsumer
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public KafkaConsumerImpl(
            ILogger<KafkaConsumerImpl> logger,
            IOptions<AppSettings> appSettings,
            IServiceScopeFactory serviceScopeFactory
        ) : base(logger, appSettings.Value.KafkaSettings, "auth-service-group")
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override string[] GetTopics() =>
            Enum.GetValues(typeof(AuthServiceKafkaTopic))
                .Cast<AuthServiceKafkaTopic>()
                .Select(topic => topic.ToString())
                .ToArray();

        protected override async Task ProcessMessageAsync(ConsumeResult<string, string> result)
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

                await aspNetUserService.DeleteAsync(message, true);

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

                await aspNetUserService.CompleteCreationAsync(message);

                LogProcessingResult(topic, message);
            }
            catch (Exception ex)
            {
                LogProcessingResult(topic, message, ex);
            }
        }
    }
}
