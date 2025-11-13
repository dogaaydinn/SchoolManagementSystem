namespace SchoolManagementSystem.Core.Entities;

/// <summary>
/// Enrollment entity linking students to courses
/// </summary>
public class Enrollment : BaseEntity
{
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public int? SemesterId { get; set; }
    public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Active"; // Active, Dropped, Completed, Withdrawn
    public DateTime? CompletionDate { get; set; }
    public decimal? FinalGrade { get; set; }
    public string? LetterGrade { get; set; }
    public bool IsWaitlisted { get; set; } = false;
    public int? WaitlistPosition { get; set; }

    // Navigation properties
    public Student Student { get; set; } = null!;
    public Course Course { get; set; } = null!;
    public Semester? Semester { get; set; }
    public ICollection<Grade> Grades { get; set; } = new List<Grade>();
}
