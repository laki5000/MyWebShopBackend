using AuthService.Configurations;
using AuthService.Shared.Enums.AuthService.Shared.Communication.Kafka;
using Microsoft.Extensions.Options;
using Shared.BaseClasses.Communication.Kafka;

namespace AuthService.App.Communication.Kafka
{
    public class KafkaTopicManager : BaseKafkaTopicManager
    {
        public KafkaTopicManager(ILogger<KafkaTopicManager> logger, IOptions<AppSettings> appSettings)
            : base(logger, appSettings.Value.KafkaSettings)
        {
        }

        protected override IEnumerable<Enum> GetKafkaTopics()
        {
            return Enum.GetValues(typeof(AuthServiceKafkaTopic)).Cast<Enum>();
        }
    }
}
