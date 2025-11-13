namespace SchoolManagementSystem.Core.Entities;

/// <summary>
/// Audit log entity for tracking system activities
/// </summary>
public class AuditLog : BaseEntity
{
    public int? UserId { get; set; }
    public string Action { get; set; } = string.Empty; // Create, Update, Delete, Login, Logout, etc.
    public string EntityType { get; set; } = string.Empty; // Student, Teacher, Course, etc.
    public int? EntityId { get; set; }
    public string? OldValues { get; set; } // JSON
    public string? NewValues { get; set; } // JSON
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? Details { get; set; }
    public string Severity { get; set; } = "Info"; // Info, Warning, Error, Critical

    // Navigation properties
    public User? User { get; set; }
}
