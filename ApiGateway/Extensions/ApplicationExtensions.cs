using ApiGateway.Middleware;
using ApiGateway.Middlewares;

namespace ApiGateway.Extensions
{
    public static class ApplicationExtensions
    {
        public static void ConfigureMiddleware(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseMiddleware<RequestLoggingMiddleware>();
            app.UseMiddleware<JwtAuthMiddleware>();
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();
        }

        public static void ConfigureEndpoints(this WebApplication app)
        {
            app.MapControllers();
            app.MapHealthChecks("/health");
        }
    }
}
