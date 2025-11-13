namespace SchoolManagementSystem.Core.Entities;

/// <summary>
/// Assignment entity for course assignments
/// </summary>
public class Assignment : BaseEntity
{
    public int CourseId { get; set; }
    public int TeacherId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Instructions { get; set; }
    public DateTime DueDate { get; set; }
    public decimal MaxScore { get; set; } = 100;
    public decimal Weight { get; set; } = 1.0m;
    public string Type { get; set; } = "Assignment"; // Assignment, Quiz, Exam, Project, Essay
    public bool AllowLateSubmission { get; set; } = false;
    public decimal? LatePenaltyPercentage { get; set; }
    public string? AttachmentUrl { get; set; }
    public bool IsPublished { get; set; } = false;
    public DateTime? PublishedDate { get; set; }
    public int TotalSubmissions { get; set; } = 0;
    public int GradedSubmissions { get; set; } = 0;

    // Navigation properties
    public Course Course { get; set; } = null!;
    public Teacher Teacher { get; set; } = null!;
    public ICollection<AssignmentSubmission> Submissions { get; set; } = new List<AssignmentSubmission>();
    public ICollection<Grade> Grades { get; set; } = new List<Grade>();
}
