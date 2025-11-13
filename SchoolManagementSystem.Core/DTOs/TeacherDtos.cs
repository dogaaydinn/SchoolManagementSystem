namespace SchoolManagementSystem.Core.DTOs;

public class TeacherDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string EmployeeNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Specialization { get; set; }
    public string? Qualification { get; set; }
    public string? DepartmentName { get; set; }
    public DateTime HireDate { get; set; }
    public string EmploymentType { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class TeacherDetailDto : TeacherDto
{
    public string? OfficeLocation { get; set; }
    public string? OfficeHours { get; set; }
    public string? Biography { get; set; }
    public string? ResearchInterests { get; set; }
    public List<CourseDto> Courses { get; set; } = new();
    public List<StudentListDto> Advisees { get; set; } = new();
}

public class CreateTeacherRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string? Specialization { get; set; }
    public string? Qualification { get; set; }
    public int? DepartmentId { get; set; }
    public decimal Salary { get; set; }
    public string EmploymentType { get; set; } = "Full-Time";
    public string? OfficeLocation { get; set; }
    public string? OfficeHours { get; set; }
    public DateTime HireDate { get; set; }
}

public class UpdateTeacherRequestDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Specialization { get; set; }
    public string? Qualification { get; set; }
    public int? DepartmentId { get; set; }
    public decimal? Salary { get; set; }
    public string? EmploymentType { get; set; }
    public string? OfficeLocation { get; set; }
    public string? OfficeHours { get; set; }
    public string? Biography { get; set; }
    public string? ResearchInterests { get; set; }
    public bool? IsActive { get; set; }
}
