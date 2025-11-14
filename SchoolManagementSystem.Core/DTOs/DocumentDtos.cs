namespace SchoolManagementSystem.Core.DTOs;

public class DocumentDto
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string FileExtension { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty; // Student, Course, Assignment, etc.
    public int? EntityId { get; set; }
    public int UploadedBy { get; set; }
    public string UploadedByName { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public string? Description { get; set; }
    public string? Tags { get; set; }
    public int? Version { get; set; }
    public bool IsPublic { get; set; }
}

public class UploadDocumentRequestDto
{
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public byte[] FileContent { get; set; } = Array.Empty<byte>();
    public string DocumentType { get; set; } = string.Empty; // Syllabus, Assignment, Transcript, Certificate, etc.
    public string EntityType { get; set; } = string.Empty;
    public int? EntityId { get; set; }
    public string? Description { get; set; }
    public string? Tags { get; set; }
    public bool IsPublic { get; set; } = false;
}

public class UpdateDocumentMetadataDto
{
    public string? Description { get; set; }
    public string? Tags { get; set; }
    public bool? IsPublic { get; set; }
    public string? DocumentType { get; set; }
}

public class BulkImportResultDto
{
    public int TotalRecords { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public List<ImportErrorDto> Errors { get; set; } = new();
    public string Message { get; set; } = string.Empty;
}

public class ImportErrorDto
{
    public int RowNumber { get; set; }
    public string Error { get; set; } = string.Empty;
    public Dictionary<string, string> RowData { get; set; } = new();
}
