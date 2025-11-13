using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Application.Interfaces;
using SchoolManagementSystem.Core.DTOs;
using System.Diagnostics;

namespace SchoolManagementSystem.Application.Services;

public class MetricsService : IMetricsService
{
    private readonly ILogger<MetricsService> _logger;
    private readonly ICacheService _cacheService;
    private static readonly Dictionary<string, long> _counters = new();
    private static readonly Dictionary<string, double> _gauges = new();
    private static readonly List<ApiCallMetric> _apiCallMetrics = new();
    private static readonly object _lock = new();

    public MetricsService(
        ILogger<MetricsService> logger,
        ICacheService cacheService)
    {
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task RecordApiCallAsync(
        string endpoint,
        string method,
        int statusCode,
        long responseTimeMs,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var metric = new ApiCallMetric
            {
                Endpoint = endpoint,
                Method = method,
                StatusCode = statusCode,
                ResponseTimeMs = responseTimeMs,
                Timestamp = DateTime.UtcNow
            };

            lock (_lock)
            {
                _apiCallMetrics.Add(metric);

                // Keep only last 10,000 metrics in memory
                if (_apiCallMetrics.Count > 10000)
                {
                    _apiCallMetrics.RemoveRange(0, 1000);
                }
            }

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording API call metric");
        }
    }

    public async Task<ApiResponse<SystemMetricsDto>> GetSystemMetricsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var process = Process.GetCurrentProcess();

            var metrics = new SystemMetricsDto
            {
                MemoryUsedMB = process.WorkingSet64 / (1024 * 1024),
                Uptime = DateTime.UtcNow - Process.GetCurrentProcess().StartTime.ToUniversalTime(),
                Timestamp = DateTime.UtcNow,
                ActiveConnections = 0 // Would integrate with actual connection pool
            };

            // CPU usage calculation (simplified)
            var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            metrics.CpuUsagePercent = cpuCounter.NextValue();

            // Memory metrics
            var availableMemory = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes / (1024 * 1024);
            metrics.MemoryTotalMB = availableMemory;
            metrics.MemoryUsagePercent = (double)metrics.MemoryUsedMB / metrics.MemoryTotalMB * 100;

            return await Task.FromResult(ApiResponse<SystemMetricsDto>.SuccessResponse(metrics));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving system metrics");
            return ApiResponse<SystemMetricsDto>.ErrorResponse("Error retrieving system metrics", 500);
        }
    }

    public async Task<ApiResponse<ApiMetricsDto>> GetApiMetricsAsync(
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var from = fromDate ?? DateTime.UtcNow.AddHours(-1);
            var to = toDate ?? DateTime.UtcNow;

            List<ApiCallMetric> metrics;
            lock (_lock)
            {
                metrics = _apiCallMetrics
                    .Where(m => m.Timestamp >= from && m.Timestamp <= to)
                    .ToList();
            }

            if (!metrics.Any())
            {
                return ApiResponse<ApiMetricsDto>.SuccessResponse(new ApiMetricsDto());
            }

            var apiMetrics = new ApiMetricsDto
            {
                TotalRequests = metrics.Count,
                SuccessfulRequests = metrics.Count(m => m.StatusCode >= 200 && m.StatusCode < 300),
                FailedRequests = metrics.Count(m => m.StatusCode >= 400),
                AverageResponseTimeMs = metrics.Average(m => m.ResponseTimeMs),
                RequestsByEndpoint = metrics
                    .GroupBy(m => m.Endpoint)
                    .ToDictionary(g => g.Key, g => (long)g.Count()),
                RequestsByStatusCode = metrics
                    .GroupBy(m => m.StatusCode)
                    .ToDictionary(g => g.Key, g => (long)g.Count())
            };

            apiMetrics.SuccessRate = apiMetrics.TotalRequests > 0
                ? (double)apiMetrics.SuccessfulRequests / apiMetrics.TotalRequests * 100
                : 0;

            // Calculate percentiles
            var sortedResponseTimes = metrics.Select(m => m.ResponseTimeMs).OrderBy(t => t).ToList();
            apiMetrics.MedianResponseTimeMs = CalculatePercentile(sortedResponseTimes, 50);
            apiMetrics.P95ResponseTimeMs = CalculatePercentile(sortedResponseTimes, 95);
            apiMetrics.P99ResponseTimeMs = CalculatePercentile(sortedResponseTimes, 99);

            // Top endpoints
            apiMetrics.TopEndpoints = metrics
                .GroupBy(m => m.Endpoint)
                .Select(g => new TopEndpointDto
                {
                    Endpoint = g.Key,
                    RequestCount = g.Count(),
                    AverageResponseTimeMs = g.Average(m => m.ResponseTimeMs)
                })
                .OrderByDescending(e => e.RequestCount)
                .Take(10)
                .ToList();

            // Slowest endpoints
            apiMetrics.SlowestEndpoints = metrics
                .GroupBy(m => m.Endpoint)
                .Select(g => new SlowEndpointDto
                {
                    Endpoint = g.Key,
                    AverageResponseTimeMs = g.Average(m => m.ResponseTimeMs),
                    MaxResponseTimeMs = g.Max(m => m.ResponseTimeMs)
                })
                .OrderByDescending(e => e.AverageResponseTimeMs)
                .Take(10)
                .ToList();

            return await Task.FromResult(ApiResponse<ApiMetricsDto>.SuccessResponse(apiMetrics));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving API metrics");
            return ApiResponse<ApiMetricsDto>.ErrorResponse("Error retrieving API metrics", 500);
        }
    }

    public async Task<ApiResponse<DatabaseMetricsDto>> GetDatabaseMetricsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // This would integrate with actual database connection pool and query statistics
            var metrics = new DatabaseMetricsDto
            {
                TotalConnections = 100, // Placeholder
                ActiveConnections = 10,
                IdleConnections = 90,
                TotalQueries = 50000,
                AverageQueryTimeMs = 15.5,
                TableCount = 20,
                RecordCounts = new Dictionary<string, int>
                {
                    { "Students", 1000 },
                    { "Courses", 500 },
                    { "Teachers", 100 },
                    { "Enrollments", 5000 }
                }
            };

            return await Task.FromResult(ApiResponse<DatabaseMetricsDto>.SuccessResponse(metrics));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving database metrics");
            return ApiResponse<DatabaseMetricsDto>.ErrorResponse("Error retrieving database metrics", 500);
        }
    }

    public async Task<ApiResponse<CacheMetricsDto>> GetCacheMetricsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // This would integrate with actual Redis cache statistics
            var metrics = new CacheMetricsDto
            {
                TotalHits = 10000,
                TotalMisses = 1000,
                CachedItemsCount = 500,
                CacheMemoryUsedMB = 50,
                AverageGetTimeMs = 2.5
            };

            metrics.HitRate = metrics.TotalHits + metrics.TotalMisses > 0
                ? (double)metrics.TotalHits / (metrics.TotalHits + metrics.TotalMisses) * 100
                : 0;

            return await Task.FromResult(ApiResponse<CacheMetricsDto>.SuccessResponse(metrics));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving cache metrics");
            return ApiResponse<CacheMetricsDto>.ErrorResponse("Error retrieving cache metrics", 500);
        }
    }

    public async Task IncrementCounterAsync(string counterName, int value = 1, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (_counters.ContainsKey(counterName))
                _counters[counterName] += value;
            else
                _counters[counterName] = value;
        }

        await Task.CompletedTask;
    }

    public async Task RecordGaugeAsync(string gaugeName, double value, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _gauges[gaugeName] = value;
        }

        await Task.CompletedTask;
    }

    private double CalculatePercentile(List<long> sortedValues, int percentile)
    {
        if (!sortedValues.Any())
            return 0;

        var index = (int)Math.Ceiling(percentile / 100.0 * sortedValues.Count) - 1;
        return sortedValues[Math.Max(0, Math.Min(index, sortedValues.Count - 1))];
    }

    private class ApiCallMetric
    {
        public string Endpoint { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public long ResponseTimeMs { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
