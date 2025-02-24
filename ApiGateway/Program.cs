using ApiGateway.Configurations;
using ApiGateway.Grpc;
using ApiGateway.Interfaces.Grpc;
using ApiGateway.Mapping;
using ApiGateway.Middleware;
using ApiGateway.Middlewares;
using Auth;
using Grpc.Core;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using User;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.jwt.json", optional: false, reloadOnChange: true);

ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

ConfigureMiddleware(app);
ConfigureEndpoints(app);

app.Run();

void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // App settings
    services.Configure<AppSettings>(configuration);

    // Services
    services.AddScoped<IAuthServiceUserClientAdapter, AuthServiceUserClientAdapterImpl>();
    services.AddScoped<IUserServiceUserClientAdapter, UserServiceUserClientAdapterImpl>();

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

    // Jwt authentication configuration
    services.AddAuthentication("Bearer")
        .AddJwtBearer(options =>
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secret = jwtSettings["Secret"] ?? throw new ArgumentNullException("JwtSettings:Secret");
            var issuer = jwtSettings["Issuer"] ?? throw new ArgumentNullException("JwtSettings:Issuer");
            var audience = jwtSettings["Audience"] ?? throw new ArgumentNullException("JwtSettings:Audience");

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
}

void ConfigureMiddleware(WebApplication app)
{
    // Enable OpenAPI in development mode
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    // Middlewares 
    app.UseMiddleware<JwtAuthMiddleware>();
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    // Enforce HTTPS redirection
    app.UseHttpsRedirection();

    // Enable authentication and authorization
    app.UseAuthentication();
    app.UseAuthorization();
}

void ConfigureEndpoints(WebApplication app)
{
    // Controllers
    app.MapControllers();

    // HealthCheck
    app.MapHealthChecks("/health");
}
