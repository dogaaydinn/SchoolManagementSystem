namespace SchoolManagementSystem.Core.DTOs;

public class ScheduleDto
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string CourseCode { get; set; } = string.Empty;
    public int TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public string DayOfWeek { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string? Room { get; set; }
    public string? Building { get; set; }
    public string Type { get; set; } = string.Empty;
}

public class CreateScheduleRequestDto
{
    public int CourseId { get; set; }
    public int TeacherId { get; set; }
    public int? SemesterId { get; set; }
    public string DayOfWeek { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string? Room { get; set; }
    public string? Building { get; set; }
    public string Type { get; set; } = "Lecture";
    public bool IsRecurring { get; set; } = true;
}

public class TimetableDto
{
    public string EntityType { get; set; } = string.Empty; // Student, Teacher
    public int EntityId { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public List<DayScheduleDto> DailySchedules { get; set; } = new();
}

public class DayScheduleDto
{
    public string DayOfWeek { get; set; } = string.Empty;
    public List<ScheduleDto> Classes { get; set; } = new();
}
