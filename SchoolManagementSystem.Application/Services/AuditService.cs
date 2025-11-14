using AutoMapper;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Application.Interfaces;
using SchoolManagementSystem.Core.DTOs;
using SchoolManagementSystem.Core.Entities;
using SchoolManagementSystem.Core.Interfaces;

namespace SchoolManagementSystem.Application.Services;

public class AuditService : IAuditService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<AuditService> _logger;

    public AuditService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<AuditService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task LogActivityAsync(
        string entityType,
        int entityId,
        string action,
        string userId,
        string? details = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var auditLog = new AuditLog
            {
                EntityType = entityType,
                EntityId = entityId,
                Action = action,
                PerformedBy = userId,
                Timestamp = DateTime.UtcNow,
                Details = details,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.AuditLogs.AddAsync(auditLog, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Audit: {Action} on {EntityType} {EntityId} by {UserId}",
                action,
                entityType,
                entityId,
                userId
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging audit activity");
            // Don't throw - audit logging failure shouldn't break the main operation
        }
    }

    public async Task<ApiResponse<PagedResult<AuditLogDto>>> GetAuditLogsAsync(
        PagedRequest request,
        string? entityType = null,
        int? entityId = null,
        string? userId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var (logs, totalCount) = await _unitOfWork.AuditLogs.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                filter: log => (string.IsNullOrEmpty(entityType) || log.EntityType == entityType) &&
                              (!entityId.HasValue || log.EntityId == entityId) &&
                              (string.IsNullOrEmpty(userId) || log.PerformedBy == userId) &&
                              (string.IsNullOrEmpty(request.SearchTerm) ||
                               log.Action.Contains(request.SearchTerm) ||
                               log.Details != null && log.Details.Contains(request.SearchTerm)),
                orderBy: query => request.SortOrder?.ToLower() == "asc"
                    ? query.OrderBy(log => log.Timestamp)
                    : query.OrderByDescending(log => log.Timestamp)
            );

            var auditDtos = _mapper.Map<List<AuditLogDto>>(logs);

            var pagedResult = new PagedResult<AuditLogDto>
            {
                Items = auditDtos,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };

            return ApiResponse<PagedResult<AuditLogDto>>.SuccessResponse(pagedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving audit logs");
            return ApiResponse<PagedResult<AuditLogDto>>.ErrorResponse("Error retrieving audit logs", 500);
        }
    }

    public async Task<ApiResponse<IEnumerable<AuditLogDto>>> GetUserActivityAsync(
        string userId,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var from = fromDate ?? DateTime.UtcNow.AddDays(-30);
            var to = toDate ?? DateTime.UtcNow;

            var logs = await _unitOfWork.AuditLogs.FindAsync(
                log => log.PerformedBy == userId &&
                       log.Timestamp >= from &&
                       log.Timestamp <= to,
                cancellationToken
            );

            var auditDtos = _mapper.Map<IEnumerable<AuditLogDto>>(logs.OrderByDescending(l => l.Timestamp));

            return ApiResponse<IEnumerable<AuditLogDto>>.SuccessResponse(auditDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user activity for {UserId}", userId);
            return ApiResponse<IEnumerable<AuditLogDto>>.ErrorResponse("Error retrieving user activity", 500);
        }
    }

    public async Task<ApiResponse<IEnumerable<AuditLogDto>>> GetEntityHistoryAsync(
        string entityType,
        int entityId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var logs = await _unitOfWork.AuditLogs.FindAsync(
                log => log.EntityType == entityType && log.EntityId == entityId,
                cancellationToken
            );

            var auditDtos = _mapper.Map<IEnumerable<AuditLogDto>>(logs.OrderBy(l => l.Timestamp));

            return ApiResponse<IEnumerable<AuditLogDto>>.SuccessResponse(auditDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving entity history for {EntityType} {EntityId}", entityType, entityId);
            return ApiResponse<IEnumerable<AuditLogDto>>.ErrorResponse("Error retrieving entity history", 500);
        }
    }

    public async Task<ApiResponse<AuditStatisticsDto>> GetAuditStatisticsAsync(
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var logs = await _unitOfWork.AuditLogs.FindAsync(
                log => log.Timestamp >= fromDate && log.Timestamp <= toDate,
                cancellationToken
            );

            var logsList = logs.ToList();

            var statistics = new AuditStatisticsDto
            {
                TotalActions = logsList.Count,
                FromDate = fromDate,
                ToDate = toDate,
                ActionsByType = logsList
                    .GroupBy(l => l.Action)
                    .ToDictionary(g => g.Key, g => g.Count()),
                ActionsByUser = logsList
                    .GroupBy(l => l.PerformedBy)
                    .ToDictionary(g => g.Key, g => g.Count()),
                ActionsByEntity = logsList
                    .GroupBy(l => l.EntityType)
                    .ToDictionary(g => g.Key, g => g.Count()),
                HourlyActivity = logsList
                    .GroupBy(l => new DateTime(l.Timestamp.Year, l.Timestamp.Month, l.Timestamp.Day, l.Timestamp.Hour, 0, 0))
                    .Select(g => new HourlyActivityDto
                    {
                        Hour = g.Key,
                        ActivityCount = g.Count()
                    })
                    .OrderBy(h => h.Hour)
                    .ToList()
            };

            return ApiResponse<AuditStatisticsDto>.SuccessResponse(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating audit statistics");
            return ApiResponse<AuditStatisticsDto>.ErrorResponse("Error calculating statistics", 500);
        }
    }
}
