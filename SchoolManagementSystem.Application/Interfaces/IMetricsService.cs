using SchoolManagementSystem.Core.DTOs;

namespace SchoolManagementSystem.Application.Interfaces;

public interface IMetricsService
{
    Task RecordApiCallAsync(string endpoint, string method, int statusCode, long responseTimeMs, CancellationToken cancellationToken = default);
    Task<ApiResponse<SystemMetricsDto>> GetSystemMetricsAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<ApiMetricsDto>> GetApiMetricsAsync(DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default);
    Task<ApiResponse<DatabaseMetricsDto>> GetDatabaseMetricsAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<CacheMetricsDto>> GetCacheMetricsAsync(CancellationToken cancellationToken = default);
    Task IncrementCounterAsync(string counterName, int value = 1, CancellationToken cancellationToken = default);
    Task RecordGaugeAsync(string gaugeName, double value, CancellationToken cancellationToken = default);
}
