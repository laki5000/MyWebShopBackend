namespace Shared.Configurations
{
    public class KafkaSettings
    {
        public required string BootstrapServers { get; set; }
        public required string AutoOffsetReset { get; set; }
    }
}
