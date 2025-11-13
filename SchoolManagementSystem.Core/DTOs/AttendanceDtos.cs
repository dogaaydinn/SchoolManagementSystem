namespace SchoolManagementSystem.Core.DTOs;

public class AttendanceDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public bool IsExcused { get; set; }
}

public class CreateAttendanceRequestDto
{
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public int? ScheduleId { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; } = "Present";
    public string? Remarks { get; set; }
    public bool IsExcused { get; set; } = false;
    public string? ExcuseReason { get; set; }
}

public class BulkAttendanceRequestDto
{
    public int CourseId { get; set; }
    public int? ScheduleId { get; set; }
    public DateTime Date { get; set; }
    public List<StudentAttendanceDto> Attendances { get; set; } = new();
}

public class StudentAttendanceDto
{
    public int StudentId { get; set; }
    public string Status { get; set; } = "Present";
    public string? Remarks { get; set; }
}

public class AttendanceReportDto
{
    public StudentDto Student { get; set; } = null!;
    public CourseDto Course { get; set; } = null!;
    public int TotalClasses { get; set; }
    public int Present { get; set; }
    public int Absent { get; set; }
    public int Late { get; set; }
    public int Excused { get; set; }
    public decimal AttendancePercentage { get; set; }
    public List<AttendanceDto> AttendanceRecords { get; set; } = new();
}
