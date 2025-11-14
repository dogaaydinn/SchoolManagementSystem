namespace SchoolManagementSystem.Core.DTOs;

public class SystemMetricsDto
{
    public double CpuUsagePercent { get; set; }
    public long MemoryUsedMB { get; set; }
    public long MemoryTotalMB { get; set; }
    public double MemoryUsagePercent { get; set; }
    public long DiskUsedGB { get; set; }
    public long DiskTotalGB { get; set; }
    public double DiskUsagePercent { get; set; }
    public TimeSpan Uptime { get; set; }
    public int ActiveConnections { get; set; }
    public DateTime Timestamp { get; set; }
}

public class ApiMetricsDto
{
    public long TotalRequests { get; set; }
    public long SuccessfulRequests { get; set; }
    public long FailedRequests { get; set; }
    public double SuccessRate { get; set; }
    public double AverageResponseTimeMs { get; set; }
    public double MedianResponseTimeMs { get; set; }
    public double P95ResponseTimeMs { get; set; }
    public double P99ResponseTimeMs { get; set; }
    public Dictionary<string, long> RequestsByEndpoint { get; set; } = new();
    public Dictionary<int, long> RequestsByStatusCode { get; set; } = new();
    public List<TopEndpointDto> TopEndpoints { get; set; } = new();
    public List<SlowEndpointDto> SlowestEndpoints { get; set; } = new();
}

public class TopEndpointDto
{
    public string Endpoint { get; set; } = string.Empty;
    public long RequestCount { get; set; }
    public double AverageResponseTimeMs { get; set; }
}

public class SlowEndpointDto
{
    public string Endpoint { get; set; } = string.Empty;
    public double AverageResponseTimeMs { get; set; }
    public double MaxResponseTimeMs { get; set; }
}

public class DatabaseMetricsDto
{
    public int TotalConnections { get; set; }
    public int ActiveConnections { get; set; }
    public int IdleConnections { get; set; }
    public long TotalQueries { get; set; }
    public double AverageQueryTimeMs { get; set; }
    public List<SlowQueryDto> SlowQueries { get; set; } = new();
    public long DatabaseSizeMB { get; set; }
    public int TableCount { get; set; }
    public Dictionary<string, int> RecordCounts { get; set; } = new();
}

public class SlowQueryDto
{
    public string Query { get; set; } = string.Empty;
    public double ExecutionTimeMs { get; set; }
    public DateTime ExecutedAt { get; set; }
}

public class CacheMetricsDto
{
    public long TotalHits { get; set; }
    public long TotalMisses { get; set; }
    public double HitRate { get; set; }
    public long CachedItemsCount { get; set; }
    public long CacheMemoryUsedMB { get; set; }
    public double AverageGetTimeMs { get; set; }
    public Dictionary<string, long> KeysByPattern { get; set; } = new();
}
