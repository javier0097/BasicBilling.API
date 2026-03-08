using System.Diagnostics;

namespace BasicBilling.API.Infrastructure.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var startTime = Stopwatch.GetTimestamp();

        await _next(context);

        var elapsedMs = Stopwatch.GetElapsedTime(startTime).TotalMilliseconds;

        _logger.LogInformation("HTTP {Method} {Path} responded {StatusCode} in {Elapsed:F0}ms",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            elapsedMs);
    }
}
