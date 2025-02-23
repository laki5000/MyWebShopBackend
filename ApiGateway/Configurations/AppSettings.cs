using Shared.Configurations;

namespace ApiGateway.Configurations
{
    public class AppSettings
    {
        public required JwtSettings JwtSettings { get; set; }
    }
}
