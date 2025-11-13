namespace SchoolManagementSystem.Core.Entities;

/// <summary>
/// Document entity for file management
/// </summary>
public class Document : BaseEntity
{
    public int UserId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string? OriginalFileName { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string? FileType { get; set; }
    public string? MimeType { get; set; }
    public string Category { get; set; } = "General"; // General, Transcript, Certificate, ID, Assignment, Syllabus
    public string? Description { get; set; }
    public bool IsPublic { get; set; } = false;
    public int DownloadCount { get; set; } = 0;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public int? RelatedEntityId { get; set; }
    public string? RelatedEntityType { get; set; }
    public int? VersionNumber { get; set; }
    public int? ParentDocumentId { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
}
