namespace SchoolManagementSystem.Core.DTOs;

public class GradeDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string CourseCode { get; set; } = string.Empty;
    public string GradeType { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public decimal MaxValue { get; set; }
    public string? LetterGrade { get; set; }
    public decimal Weight { get; set; }
    public DateTime GradeDate { get; set; }
    public string? Comments { get; set; }
    public bool IsPublished { get; set; }
}

public class CreateGradeRequestDto
{
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public int? EnrollmentId { get; set; }
    public int? AssignmentId { get; set; }
    public string GradeType { get; set; } = "Assignment";
    public decimal Value { get; set; }
    public decimal MaxValue { get; set; } = 100;
    public decimal Weight { get; set; } = 1.0m;
    public string? Comments { get; set; }
    public bool IsPublished { get; set; } = false;
}

public class UpdateGradeRequestDto
{
    public decimal? Value { get; set; }
    public decimal? MaxValue { get; set; }
    public decimal? Weight { get; set; }
    public string? Comments { get; set; }
    public bool? IsPublished { get; set; }
}

public class BulkGradeRequestDto
{
    public int CourseId { get; set; }
    public int? AssignmentId { get; set; }
    public string GradeType { get; set; } = "Assignment";
    public decimal MaxValue { get; set; } = 100;
    public decimal Weight { get; set; } = 1.0m;
    public List<StudentGradeDto> StudentGrades { get; set; } = new();
}

public class StudentGradeDto
{
    public int StudentId { get; set; }
    public decimal Value { get; set; }
    public string? Comments { get; set; }
}
