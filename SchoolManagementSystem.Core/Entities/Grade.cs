namespace SchoolManagementSystem.Core.Entities;

/// <summary>
/// Grade entity for student assessments
/// </summary>
public class Grade : BaseEntity
{
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public int? EnrollmentId { get; set; }
    public int? AssignmentId { get; set; }
    public string GradeType { get; set; } = "Assignment"; // Assignment, Exam, Midterm, Final, Project
    public decimal Value { get; set; } // 0-100
    public decimal MaxValue { get; set; } = 100;
    public string? LetterGrade { get; set; }
    public decimal Weight { get; set; } = 1.0m; // Weightage in final grade
    public DateTime GradeDate { get; set; } = DateTime.UtcNow;
    public int? GradedBy { get; set; }
    public string? Comments { get; set; }
    public bool IsPublished { get; set; } = false;
    public DateTime? PublishedDate { get; set; }

    // Navigation properties
    public Student Student { get; set; } = null!;
    public Course Course { get; set; } = null!;
    public Enrollment? Enrollment { get; set; }
    public Assignment? Assignment { get; set; }
}
