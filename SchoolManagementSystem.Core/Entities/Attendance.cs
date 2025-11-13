namespace SchoolManagementSystem.Core.Entities;

/// <summary>
/// Attendance entity for tracking student attendance
/// </summary>
public class Attendance : BaseEntity
{
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public int? ScheduleId { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Present"; // Present, Absent, Late, Excused
    public string? Remarks { get; set; }
    public TimeSpan? CheckInTime { get; set; }
    public TimeSpan? CheckOutTime { get; set; }
    public string? MarkedBy { get; set; }
    public bool IsExcused { get; set; } = false;
    public string? ExcuseReason { get; set; }
    public string? DocumentUrl { get; set; } // Medical certificate, etc.

    // Navigation properties
    public Student Student { get; set; } = null!;
    public Course Course { get; set; } = null!;
    public Schedule? Schedule { get; set; }
}
