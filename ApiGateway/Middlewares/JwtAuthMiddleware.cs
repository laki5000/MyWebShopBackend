using ApiGateway.Constants;
using Shared.Dtos;
using Shared.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ApiGateway.Middleware
{
    public class JwtAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtAuthMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
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
                context.Response.ContentType = "application/json";

                var response = ApiResponseDto.Fail(ErrorCodeEnum.INVALID_TOKEN);
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
