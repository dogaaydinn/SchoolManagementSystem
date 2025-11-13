using SchoolManagementSystem.Core.DTOs;

namespace SchoolManagementSystem.Application.Interfaces;

public interface IAuditService
{
    Task LogActivityAsync(string entityType, int entityId, string action, string userId, string? details = null, CancellationToken cancellationToken = default);
    Task<ApiResponse<PagedResult<AuditLogDto>>> GetAuditLogsAsync(PagedRequest request, string? entityType = null, int? entityId = null, string? userId = null, CancellationToken cancellationToken = default);
    Task<ApiResponse<IEnumerable<AuditLogDto>>> GetUserActivityAsync(string userId, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default);
    Task<ApiResponse<IEnumerable<AuditLogDto>>> GetEntityHistoryAsync(string entityType, int entityId, CancellationToken cancellationToken = default);
    Task<ApiResponse<AuditStatisticsDto>> GetAuditStatisticsAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
}
