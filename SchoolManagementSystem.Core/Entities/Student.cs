namespace SchoolManagementSystem.Core.Entities;

/// <summary>
/// Student entity with comprehensive academic information
/// </summary>
public class Student : BaseEntity
{
    public int UserId { get; set; }
    public string StudentNumber { get; set; } = string.Empty;
    public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
    public decimal GPA { get; set; } = 0.0m;
    public int CurrentSemester { get; set; } = 1;
    public string? Major { get; set; }
    public string? Minor { get; set; }
    public int? AdvisorId { get; set; }
    public string Status { get; set; } = "Active"; // Active, Probation, Suspended, Graduated
    public DateTime? GraduationDate { get; set; }
    public int TotalCreditsEarned { get; set; } = 0;
    public int TotalCreditsRequired { get; set; } = 120;
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public Teacher? Advisor { get; set; }
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<Grade> Grades { get; set; } = new List<Grade>();
    public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    public ICollection<AssignmentSubmission> AssignmentSubmissions { get; set; } = new List<AssignmentSubmission>();
    public ICollection<Document> Documents { get; set; } = new List<Document>();
}
