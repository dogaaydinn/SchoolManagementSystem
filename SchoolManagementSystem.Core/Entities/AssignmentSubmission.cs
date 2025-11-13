namespace SchoolManagementSystem.Core.Entities;

/// <summary>
/// Assignment submission entity
/// </summary>
public class AssignmentSubmission : BaseEntity
{
    public int AssignmentId { get; set; }
    public int StudentId { get; set; }
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public string? FileUrl { get; set; }
    public string? FileName { get; set; }
    public long? FileSize { get; set; }
    public string? SubmissionText { get; set; }
    public decimal? Score { get; set; }
    public string? Feedback { get; set; }
    public DateTime? GradedAt { get; set; }
    public int? GradedBy { get; set; }
    public string Status { get; set; } = "Submitted"; // Submitted, Graded, Returned, Late
    public bool IsLate { get; set; } = false;
    public int? PlagiarismScore { get; set; } // 0-100
    public string? PlagiarismReport { get; set; }

    // Navigation properties
    public Assignment Assignment { get; set; } = null!;
    public Student Student { get; set; } = null!;
}
