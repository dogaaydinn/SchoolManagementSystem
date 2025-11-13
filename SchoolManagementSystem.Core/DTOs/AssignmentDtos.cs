namespace SchoolManagementSystem.Core.DTOs;

public class AssignmentDto
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
    public decimal MaxScore { get; set; }
    public decimal Weight { get; set; }
    public string Type { get; set; } = string.Empty;
    public bool AllowLateSubmission { get; set; }
    public decimal? LatePenaltyPercentage { get; set; }
    public bool IsPublished { get; set; }
    public int TotalSubmissions { get; set; }
    public int GradedSubmissions { get; set; }
}

public class AssignmentDetailDto : AssignmentDto
{
    public string? Instructions { get; set; }
    public string? AttachmentUrl { get; set; }
    public string? TeacherName { get; set; }
    public DateTime? PublishedDate { get; set; }
    public List<AssignmentSubmissionDto> Submissions { get; set; } = new();
}

public class CreateAssignmentRequestDto
{
    public int CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Instructions { get; set; }
    public DateTime DueDate { get; set; }
    public decimal MaxScore { get; set; } = 100;
    public decimal Weight { get; set; } = 1.0m;
    public string Type { get; set; } = "Assignment";
    public bool AllowLateSubmission { get; set; } = false;
    public decimal? LatePenaltyPercentage { get; set; }
    public bool IsPublished { get; set; } = false;
}

public class UpdateAssignmentRequestDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Instructions { get; set; }
    public DateTime? DueDate { get; set; }
    public decimal? MaxScore { get; set; }
    public decimal? Weight { get; set; }
    public bool? AllowLateSubmission { get; set; }
    public decimal? LatePenaltyPercentage { get; set; }
    public bool? IsPublished { get; set; }
}

public class AssignmentSubmissionDto
{
    public int Id { get; set; }
    public int AssignmentId { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
    public string? FileUrl { get; set; }
    public string? FileName { get; set; }
    public long? FileSize { get; set; }
    public decimal? Score { get; set; }
    public string? Feedback { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsLate { get; set; }
    public int? PlagiarismScore { get; set; }
}

public class SubmitAssignmentRequestDto
{
    public int AssignmentId { get; set; }
    public string? SubmissionText { get; set; }
    public string? FileUrl { get; set; }
    public string? FileName { get; set; }
    public long? FileSize { get; set; }
}

public class GradeSubmissionRequestDto
{
    public int SubmissionId { get; set; }
    public decimal Score { get; set; }
    public string? Feedback { get; set; }
}
