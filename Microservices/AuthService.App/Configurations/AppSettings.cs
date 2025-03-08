using Shared.Configurations;

namespace AuthService.Configurations
{
    public class AppSettings
    {
        public required string PostgresConnection { get; set; }
        public required JwtSettings JwtSettings { get; set; }
        public required KafkaSettings KafkaSettings { get; set; }
    }
}
