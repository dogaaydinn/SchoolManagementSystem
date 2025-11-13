namespace SchoolManagementSystem.Core.Entities;

/// <summary>
/// System settings entity for application configuration
/// </summary>
public class SystemSetting : BaseEntity
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? Description { get; set; }
    public string DataType { get; set; } = "String"; // String, Int, Bool, JSON
    public bool IsEncrypted { get; set; } = false;
    public bool IsSystem { get; set; } = false; // System settings cannot be deleted
    public string? ValidValues { get; set; } // JSON array of allowed values
}
