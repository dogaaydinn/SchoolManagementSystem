namespace SchoolManagementSystem.Core.Entities;

/// <summary>
/// Admin entity for system administrators
/// </summary>
public class Admin : BaseEntity
{
    public int UserId { get; set; }
    public string EmployeeNumber { get; set; } = string.Empty;
    public string Role { get; set; } = "Admin"; // Admin, SuperAdmin, Manager
    public string? Department { get; set; }
    public string Permissions { get; set; } = "All"; // JSON string or comma-separated values
    public bool IsSuperAdmin { get; set; } = false;
    public DateTime HireDate { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User User { get; set; } = null!;
}
