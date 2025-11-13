namespace SchoolManagementSystem.Core.Entities;

/// <summary>
/// Teacher entity with professional information
/// </summary>
public class Teacher : BaseEntity
{
    public int UserId { get; set; }
    public string EmployeeNumber { get; set; } = string.Empty;
    public DateTime HireDate { get; set; } = DateTime.UtcNow;
    public string? Specialization { get; set; }
    public string? Qualification { get; set; }
    public int? DepartmentId { get; set; }
    public decimal Salary { get; set; }
    public string EmploymentType { get; set; } = "Full-Time"; // Full-Time, Part-Time, Contract
    public string? OfficeLocation { get; set; }
    public string? OfficeHours { get; set; }
    public string? Biography { get; set; }
    public string? ResearchInterests { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public User User { get; set; } = null!;
    public Department? Department { get; set; }
    public ICollection<Course> Courses { get; set; } = new List<Course>();
    public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    public ICollection<Student> Advisees { get; set; } = new List<Student>();
    public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    public ICollection<Document> Documents { get; set; } = new List<Document>();
}
