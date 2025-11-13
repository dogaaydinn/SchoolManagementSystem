namespace SchoolManagementSystem.Core.Entities;

/// <summary>
/// Semester entity for academic terms
/// </summary>
public class Semester : BaseEntity
{
    public string Name { get; set; } = string.Empty; // Fall 2024, Spring 2025
    public string Code { get; set; } = string.Empty; // F24, S25
    public int Year { get; set; }
    public string Term { get; set; } = string.Empty; // Fall, Spring, Summer
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; } = false;
    public bool RegistrationOpen { get; set; } = false;
    public DateTime? RegistrationStartDate { get; set; }
    public DateTime? RegistrationEndDate { get; set; }
    public DateTime? DropDeadline { get; set; }
    public DateTime? WithdrawalDeadline { get; set; }

    // Navigation properties
    public ICollection<Course> Courses { get; set; } = new List<Course>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
