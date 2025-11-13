namespace SchoolManagementSystem.Core.DTOs;

public class AuditLogDto
{
    public int Id { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public string Action { get; set; } = string.Empty; // Created, Updated, Deleted, Viewed, etc.
    public string PerformedBy { get; set; } = string.Empty;
    public string PerformedByName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? Details { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
}

public class AuditStatisticsDto
{
    public int TotalActions { get; set; }
    public Dictionary<string, int> ActionsByType { get; set; } = new();
    public Dictionary<string, int> ActionsByUser { get; set; } = new();
    public Dictionary<string, int> ActionsByEntity { get; set; } = new();
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public List<HourlyActivityDto> HourlyActivity { get; set; } = new();
}

public class HourlyActivityDto
{
    public DateTime Hour { get; set; }
    public int ActivityCount { get; set; }
}
