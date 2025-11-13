namespace SchoolManagementSystem.Core.DTOs;

public class StudentDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string StudentNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public int? Age { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public decimal GPA { get; set; }
    public int CurrentSemester { get; set; }
    public string? Major { get; set; }
    public string? Minor { get; set; }
    public string Status { get; set; } = string.Empty;
    public int TotalCreditsEarned { get; set; }
    public int TotalCreditsRequired { get; set; }
    public string? ProfilePictureUrl { get; set; }
}

public class StudentDetailDto : StudentDto
{
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public TeacherDto? Advisor { get; set; }
    public List<CourseDto> EnrolledCourses { get; set; } = new();
    public List<GradeDto> Grades { get; set; } = new();
}

public class CreateStudentRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string? Major { get; set; }
    public string? Minor { get; set; }
    public int? AdvisorId { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
}

public class UpdateStudentRequestDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Major { get; set; }
    public string? Minor { get; set; }
    public int? AdvisorId { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string? Status { get; set; }
}

public class StudentListDto
{
    public int Id { get; set; }
    public string StudentNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal GPA { get; set; }
    public int CurrentSemester { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Major { get; set; }
}

public class StudentTranscriptDto
{
    public StudentDto Student { get; set; } = null!;
    public List<SemesterGradesDto> SemesterGrades { get; set; } = new();
    public decimal OverallGPA { get; set; }
    public int TotalCreditsEarned { get; set; }
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}

public class SemesterGradesDto
{
    public string SemesterName { get; set; } = string.Empty;
    public List<GradeDto> Grades { get; set; } = new();
    public decimal SemesterGPA { get; set; }
    public int CreditsEarned { get; set; }
}
