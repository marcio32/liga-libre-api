using System.Diagnostics;

namespace LigaLibre.API.Middlewares;

public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        logger.LogInformation($"Iniciando {context.Request.Method} {context.Request.Path} " +
            $"- Usuario: {context.User?.Identity?.Name ?? "Anonimo"} " +
            $"- IP : {context.Connection.RemoteIpAddress.ToString()}");

        await next(context);

        stopwatch.Stop();

        var logLevel = context.Response.StatusCode >= 400 ? LogLevel.Warning : LogLevel.Information;
        logger.Log(logLevel, $"Completado {context.Request.Method} {context.Request.Path} " +
            $"- Status {context.Response.StatusCode} " +
            $"- Tiempo : {stopwatch.ElapsedMilliseconds}");
    }


}

