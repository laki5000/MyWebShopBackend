namespace ApiGateway.Extensions
{
    public static class ConfigurationExtensions
    {
        public static void ConfigureConfiguration(this WebApplicationBuilder builder)
        {
            builder.Configuration
                .AddJsonFile("appsettings.jwt.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.kafka.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
        }
    }
}
