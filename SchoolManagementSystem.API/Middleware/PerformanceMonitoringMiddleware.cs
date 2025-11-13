using SchoolManagementSystem.Application.Interfaces;
using System.Diagnostics;

namespace SchoolManagementSystem.API.Middleware;

public class PerformanceMonitoringMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PerformanceMonitoringMiddleware> _logger;

    public PerformanceMonitoringMiddleware(
        RequestDelegate next,
        ILogger<PerformanceMonitoringMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IMetricsService metricsService)
    {
        var stopwatch = Stopwatch.StartNew();
        var endpoint = $"{context.Request.Method} {context.Request.Path}";

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            var responseTimeMs = stopwatch.ElapsedMilliseconds;

            // Record metrics
            await metricsService.RecordApiCallAsync(
                endpoint,
                context.Request.Method,
                context.Response.StatusCode,
                responseTimeMs
            );

            // Log slow requests (>1000ms)
            if (responseTimeMs > 1000)
            {
                _logger.LogWarning(
                    "Slow request detected: {Method} {Path} took {ResponseTime}ms - Status: {StatusCode}",
                    context.Request.Method,
                    context.Request.Path,
                    responseTimeMs,
                    context.Response.StatusCode
                );
            }

            // Add response time header
            context.Response.Headers.Add("X-Response-Time-Ms", responseTimeMs.ToString());
        }
    }
}
