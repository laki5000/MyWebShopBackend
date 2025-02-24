using Shared.Dtos;
using Shared.Enums;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace ApiGateway.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next) {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var response = ApiResponseDto.Fail(ErrorCodeEnum.UNKNOWN_ERROR);
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
