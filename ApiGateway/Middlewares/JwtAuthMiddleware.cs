using ApiGateway.Constants;
using Shared.Dtos;
using Shared.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ApiGateway.Middleware
{
    public class JwtAuthMiddleware
    {
        private readonly ILogger<JwtAuthMiddleware> _logger;
        private readonly RequestDelegate _next;

        private readonly string CONTENT_TYPE = "application/json";

        public JwtAuthMiddleware(ILogger<JwtAuthMiddleware> logger, RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Cookies[AuthConstants.ACCESS_TOKEN_COOKIE_NAME];

            if (!string.IsNullOrEmpty(token))
            {
                context.Request.Headers.Append("Authorization", $"Bearer {token}");
            }

            await _next(context);

            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized && !context.Response.HasStarted)
            {
                _logger.LogWarning("Unauthorized access attempt detected.");

                context.Response.ContentType = CONTENT_TYPE;

                var response = ApiResponseDto.Fail(ErrorCode.INVALID_TOKEN);
                var jsonOptions = new JsonSerializerOptions
                {
                    Converters = { new JsonStringEnumConverter() }
                };
                var json = JsonSerializer.Serialize(response, jsonOptions);

                await context.Response.WriteAsync(json);
            }
            else if (context.Response.StatusCode == StatusCodes.Status403Forbidden && !context.Response.HasStarted)
            {
                _logger.LogWarning("Forbidden access attempt detected.");

                context.Response.ContentType = CONTENT_TYPE;

                var response = ApiResponseDto.Fail(ErrorCode.FORBIDDEN);
                var jsonOptions = new JsonSerializerOptions
                {
                    Converters = { new JsonStringEnumConverter() }
                };
                var json = JsonSerializer.Serialize(response, jsonOptions);

                await context.Response.WriteAsync(json);
            }
        }
    }
}
