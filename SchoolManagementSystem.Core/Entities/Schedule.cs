namespace SchoolManagementSystem.Core.Entities;

/// <summary>
/// Schedule entity for course timetables
/// </summary>
public class Schedule : BaseEntity
{
    public int CourseId { get; set; }
    public int TeacherId { get; set; }
    public int? SemesterId { get; set; }
    public int? RoomId { get; set; }
    public string DayOfWeek { get; set; } = string.Empty; // Monday, Tuesday, etc.
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string? Room { get; set; }
    public string? Building { get; set; }
    public string Type { get; set; } = "Lecture"; // Lecture, Lab, Tutorial, Seminar
    public bool IsRecurring { get; set; } = true;
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    public Course Course { get; set; } = null!;
    public Teacher Teacher { get; set; } = null!;
    public Semester? Semester { get; set; }
    public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
}
