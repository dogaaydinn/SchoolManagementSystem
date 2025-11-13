namespace SchoolManagementSystem.Core.DTOs;

public class CourseDto
{
    public int Id { get; set; }
    public string CourseCode { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Credits { get; set; }
    public string? DepartmentName { get; set; }
    public string? TeacherName { get; set; }
    public int MaxStudents { get; set; }
    public int CurrentEnrollment { get; set; }
    public string Level { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string? SemesterName { get; set; }
}

public class CourseDetailDto : CourseDto
{
    public string? Syllabus { get; set; }
    public string? LearningOutcomes { get; set; }
    public List<string>? Prerequisites { get; set; }
    public decimal? CourseFee { get; set; }
    public TeacherDto? Teacher { get; set; }
    public List<StudentListDto> EnrolledStudents { get; set; } = new();
    public List<AssignmentDto> Assignments { get; set; } = new();
    public List<ScheduleDto> Schedules { get; set; } = new();
}

public class CreateCourseRequestDto
{
    public string CourseCode { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Credits { get; set; } = 3;
    public int? DepartmentId { get; set; }
    public int? TeacherId { get; set; }
    public int MaxStudents { get; set; } = 30;
    public string Level { get; set; } = "Undergraduate";
    public List<int>? PrerequisiteCourseIds { get; set; }
    public string? Syllabus { get; set; }
    public string? LearningOutcomes { get; set; }
    public int? SemesterId { get; set; }
    public decimal? CourseFee { get; set; }
}

public class UpdateCourseRequestDto
{
    public string? CourseName { get; set; }
    public string? Description { get; set; }
    public int? Credits { get; set; }
    public int? TeacherId { get; set; }
    public int? MaxStudents { get; set; }
    public List<int>? PrerequisiteCourseIds { get; set; }
    public string? Syllabus { get; set; }
    public string? LearningOutcomes { get; set; }
    public bool? IsActive { get; set; }
}

public class EnrollStudentRequestDto
{
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public int? SemesterId { get; set; }
}
