using System.Collections.Concurrent;

namespace LigaLibre.API.Middlewares;

public class RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger)
{
    private static readonly ConcurrentDictionary<string, List<DateTime>> _request = new();
    private readonly int _maxRequests = 10;
    private readonly TimeSpan _timeWindow = TimeSpan.FromMinutes(1);

    public async Task InvokeAsync(HttpContext context)
    {
        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var now = DateTime.UtcNow;

        _request.AddOrUpdate(clientIp, new List<DateTime> { now }, 
            (key, existing) =>
        { 
            existing.RemoveAll(time => now - time > _timeWindow);
            existing.Add(now);
            return existing;
        });

        if (_request[clientIp].Count > _maxRequests)
        {
            logger.LogWarning("Rate limit excedido para IP: {IP}", clientIp);
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.Response.WriteAsync("Demasiadas solicitudes. Por favor, intente de nuevo más tarde.");
            return;
        }

        await next(context);
    }

}
