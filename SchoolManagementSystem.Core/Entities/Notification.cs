namespace SchoolManagementSystem.Core.Entities;

/// <summary>
/// Notification entity for system notifications
/// </summary>
public class Notification : BaseEntity
{
    public int UserId { get; set; }
    public string Type { get; set; } = "Info"; // Info, Warning, Error, Success
    public string Category { get; set; } = "General"; // General, Grade, Attendance, Assignment, System
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }
    public string? ActionUrl { get; set; }
    public string? ActionText { get; set; }
    public string Priority { get; set; } = "Normal"; // Low, Normal, High, Urgent
    public DateTime? ExpiresAt { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
}
