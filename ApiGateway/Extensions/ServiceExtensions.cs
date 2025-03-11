using ApiGateway.Communication.Grpc;
using ApiGateway.Configurations;
using ApiGateway.Interfaces.Grpc;
using ApiGateway.Mapping;
using Authservice.Proto;
using AuthService.Shared.Communication.Kafka;
using AuthService.Shared.Interfaces.Communication.Kafka;
using Grpc.Core;
using Microsoft.IdentityModel.Tokens;
using Productservice.Proto;
using Shared.Configurations;
using System.Text;
using System.Text.Json.Serialization;
using Userservice.Proto;

namespace ApiGateway.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureServices(this WebApplicationBuilder builder)
        {
            var services = builder.Services;
            var configuration = builder.Configuration;

            // App settings
            services.Configure<AppSettings>(configuration);
            services.Configure<KafkaSettings>(configuration.GetSection("KafkaSettings"));

            // Services
            services.AddScoped<IAuthServiceUserClientAdapter, AuthServiceUserClientAdapterImpl>();
            services.AddScoped<IAuthServiceRoleClientAdapter, AuthServiceRoleClientAdapterImpl>();
            services.AddScoped<IUserServiceUserClientAdapter, UserServiceUserClientAdapterImpl>();
            services.AddScoped<IProductServiceCategoryClientAdapter, ProductServiceCategoryClientAdapterImpl>();
            services.AddScoped<IProductServiceProductClientAdapter, ProductServiceProductClientAdapterImpl>();

            // Controller settings
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            // AutoMapper
            services.AddAutoMapper(typeof(MappingProfile).Assembly);

            // Grpc
            services.AddGrpcClient<AuthServiceUser.AuthServiceUserClient>(options =>
            {
                options.Address = new Uri("http://authservice:5000");
            })
            .ConfigureChannel(options =>
            {
                options.Credentials = ChannelCredentials.Insecure;
                options.HttpHandler = new SocketsHttpHandler()
                {
                    EnableMultipleHttp2Connections = true
                };
            });

            services.AddGrpcClient<AuthServiceRole.AuthServiceRoleClient>(options =>
            {
                options.Address = new Uri("http://authservice:5000");
            })
            .ConfigureChannel(options =>
            {
                options.Credentials = ChannelCredentials.Insecure;
                options.HttpHandler = new SocketsHttpHandler()
                {
                    EnableMultipleHttp2Connections = true
                };
            });

            services.AddGrpcClient<UserServiceUser.UserServiceUserClient>(options =>
            {
                options.Address = new Uri("http://userservice:5000");
            })
            .ConfigureChannel(options =>
            {
                options.Credentials = ChannelCredentials.Insecure;
                options.HttpHandler = new SocketsHttpHandler()
                {
                    EnableMultipleHttp2Connections = true
                };
            });

            services.AddGrpcClient<ProductServiceCategory.ProductServiceCategoryClient>(options =>
            {
                options.Address = new Uri("http://productservice:5000");
            })
            .ConfigureChannel(options =>
            {
                options.Credentials = ChannelCredentials.Insecure;
                options.HttpHandler = new SocketsHttpHandler()
                {
                    EnableMultipleHttp2Connections = true
                };
            });

            services.AddGrpcClient<ProductServiceProduct.ProductServiceProductClient>(options =>
            {
                options.Address = new Uri("http://productservice:5000");
            })
            .ConfigureChannel(options =>
            {
                options.Credentials = ChannelCredentials.Insecure;
                options.HttpHandler = new SocketsHttpHandler()
                {
                    EnableMultipleHttp2Connections = true
                };
            });

            // Jwt authentication configuration
            services.AddAuthentication("Bearer")
                .AddJwtBearer(options =>
                {
                    var jwtSettings = configuration.GetSection("JwtSettings");
                    var secret = jwtSettings["Secret"]!;
                    var issuer = jwtSettings["Issuer"]!;
                    var audience = jwtSettings["Audience"]!;

                    options.RequireHttpsMetadata = true;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                    };
                });

            // HealthCheck
            services.AddHealthChecks();

            // OpenApi
            services.AddOpenApi();

            // Kafka
            services.AddSingleton<IAuthServiceKafkaProducer, AuthServiceKafkaProducerImpl>();
        }
    }
}
