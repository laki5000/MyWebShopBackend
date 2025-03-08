namespace ProductService.App.Extensions
{
    public static class ConfigurationExtensions
    {
        public static void ConfigureConfiguration(this WebApplicationBuilder builder)
        {
            builder.Configuration
                .AddJsonFile("appsettings.grpc.json", optional: false, reloadOnChange: true);
        }
    }
}
