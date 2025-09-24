namespace LigaLibre.API.Middlewares
{
    public class ErrorLoggingMiddleware(RequestDelegate next, ILogger<ErrorLoggingMiddleware> logger)
    {

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch(Exception ex)
            {
                logger.LogError(ex, $"Error no controlado en {context.Request.Method} {context.Request.Path} - usuario {context.User?.Identity?.Name ?? "Anonimo"}");
            }
        }
    }
}
