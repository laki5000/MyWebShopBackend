namespace ApiGateway.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly ILogger<RequestLoggingMiddleware> _logger;
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(ILogger<RequestLoggingMiddleware> logger, RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        { 
            var request = context.Request;
            _logger.LogInformation("Incoming request: {Method} {Url}", request.Method, request.Path);

            await _next(context);

            var response = context.Response;
            _logger.LogInformation("Response for {Method} {Url} with status code {StatusCode}", request.Method, request.Path, response.StatusCode);
        }
    }
}
