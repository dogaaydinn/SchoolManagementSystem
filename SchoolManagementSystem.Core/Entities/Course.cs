namespace SchoolManagementSystem.Core.Entities;

/// <summary>
/// Course entity with comprehensive course information
/// </summary>
public class Course : BaseEntity
{
    public string CourseCode { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Credits { get; set; } = 3;
    public int? DepartmentId { get; set; }
    public int? TeacherId { get; set; }
    public int MaxStudents { get; set; } = 30;
    public int CurrentEnrollment { get; set; } = 0;
    public string Level { get; set; } = "Undergraduate"; // Undergraduate, Graduate, Doctoral
    public string? Prerequisites { get; set; } // JSON array of prerequisite course IDs
    public bool IsActive { get; set; } = true;
    public string? Syllabus { get; set; }
    public string? LearningOutcomes { get; set; }
    public int? SemesterId { get; set; }
    public decimal? CourseFee { get; set; }

    // Navigation properties
    public Department? Department { get; set; }
    public Teacher? Teacher { get; set; }
    public Semester? Semester { get; set; }
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    public ICollection<Grade> Grades { get; set; } = new List<Grade>();
}
