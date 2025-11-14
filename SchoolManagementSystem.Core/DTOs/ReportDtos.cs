namespace SchoolManagementSystem.Core.DTOs;

public class StudentPerformanceReportDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentNumber { get; set; } = string.Empty;
    public decimal OverallGPA { get; set; }
    public int TotalCreditsEarned { get; set; }
    public int TotalCreditsRequired { get; set; }
    public decimal CompletionPercentage { get; set; }
    public List<CoursePerformanceDto> CoursePerformances { get; set; } = new();
    public AttendanceSummaryDto AttendanceSummary { get; set; } = new();
    public List<GradeDto> RecentGrades { get; set; } = new();
    public string AcademicStanding { get; set; } = string.Empty; // Good Standing, Probation, etc.
}

public class CoursePerformanceDto
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string CourseCode { get; set; } = string.Empty;
    public decimal? FinalGrade { get; set; }
    public string? LetterGrade { get; set; }
    public int Credits { get; set; }
    public decimal AttendancePercentage { get; set; }
    public int AssignmentsCompleted { get; set; }
    public int TotalAssignments { get; set; }
}

public class AttendanceSummaryDto
{
    public int TotalClasses { get; set; }
    public int Present { get; set; }
    public int Absent { get; set; }
    public int Late { get; set; }
    public int Excused { get; set; }
    public decimal AttendancePercentage { get; set; }
}

public class CoursePerformanceReportDto
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string CourseCode { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public int TotalStudents { get; set; }
    public int ActiveStudents { get; set; }
    public int DroppedStudents { get; set; }
    public decimal AverageGrade { get; set; }
    public decimal PassRate { get; set; }
    public GradeDistributionDto GradeDistribution { get; set; } = new();
    public decimal AverageAttendancePercentage { get; set; }
    public int TotalAssignments { get; set; }
    public decimal AverageSubmissionRate { get; set; }
}

public class GradeDistributionDto
{
    public int ACount { get; set; }
    public int BCount { get; set; }
    public int CCount { get; set; }
    public int DCount { get; set; }
    public int FCount { get; set; }
    public Dictionary<string, int> DetailedDistribution { get; set; } = new();
}

public class TeacherPerformanceReportDto
{
    public int TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public string EmployeeNumber { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public int TotalCourses { get; set; }
    public int TotalStudents { get; set; }
    public decimal AverageClassSize { get; set; }
    public decimal AverageStudentGrade { get; set; }
    public decimal StudentPassRate { get; set; }
    public List<CoursePerformanceReportDto> CourseReports { get; set; } = new();
    public int TotalAdvisees { get; set; }
    public decimal AssignmentCreationRate { get; set; }
    public decimal AssignmentGradingTimeliness { get; set; }
}

public class DepartmentReportDto
{
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public string DepartmentCode { get; set; } = string.Empty;
    public int TotalTeachers { get; set; }
    public int TotalStudents { get; set; }
    public int TotalCourses { get; set; }
    public int ActiveCourses { get; set; }
    public decimal AverageDepartmentGPA { get; set; }
    public decimal StudentRetentionRate { get; set; }
    public Dictionary<string, int> EnrollmentByLevel { get; set; } = new();
    public List<TopPerformingStudentDto> TopPerformingStudents { get; set; } = new();
    public List<CourseEnrollmentStatDto> CourseEnrollmentStats { get; set; } = new();
}

public class TopPerformingStudentDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public decimal GPA { get; set; }
    public int CreditsEarned { get; set; }
}

public class CourseEnrollmentStatDto
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int EnrolledCount { get; set; }
    public int MaxCapacity { get; set; }
    public decimal EnrollmentPercentage { get; set; }
}

public class EnrollmentReportDto
{
    public int TotalEnrollments { get; set; }
    public int ActiveEnrollments { get; set; }
    public int CompletedEnrollments { get; set; }
    public int DroppedEnrollments { get; set; }
    public Dictionary<string, int> EnrollmentsByDepartment { get; set; } = new();
    public Dictionary<string, int> EnrollmentsBySemester { get; set; } = new();
    public List<EnrollmentTrendDto> EnrollmentTrends { get; set; } = new();
}

public class EnrollmentTrendDto
{
    public DateTime Date { get; set; }
    public int NewEnrollments { get; set; }
    public int Drops { get; set; }
    public int NetChange { get; set; }
}

public class TranscriptDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentNumber { get; set; } = string.Empty;
    public string Major { get; set; } = string.Empty;
    public string Minor { get; set; } = string.Empty;
    public DateTime EnrollmentDate { get; set; }
    public decimal CumulativeGPA { get; set; }
    public int TotalCreditsEarned { get; set; }
    public List<SemesterTranscriptDto> Semesters { get; set; } = new();
}

public class SemesterTranscriptDto
{
    public string SemesterName { get; set; } = string.Empty;
    public List<CourseGradeDto> Courses { get; set; } = new();
    public decimal SemesterGPA { get; set; }
    public int CreditsEarned { get; set; }
}

public class CourseGradeDto
{
    public string CourseCode { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public int Credits { get; set; }
    public decimal Grade { get; set; }
    public string LetterGrade { get; set; } = string.Empty;
}

public class StudentStatisticsDto
{
    public int TotalCoursesEnrolled { get; set; }
    public int CompletedCourses { get; set; }
    public int InProgressCourses { get; set; }
    public decimal OverallGPA { get; set; }
    public decimal CurrentSemesterGPA { get; set; }
    public decimal AverageAttendanceRate { get; set; }
    public int TotalAssignmentsSubmitted { get; set; }
    public int TotalAssignmentsGraded { get; set; }
    public decimal AssignmentCompletionRate { get; set; }
}

public class CourseStatisticsDto
{
    public int TotalEnrolled { get; set; }
    public int ActiveStudents { get; set; }
    public decimal AverageGrade { get; set; }
    public decimal PassRate { get; set; }
    public decimal AverageAttendanceRate { get; set; }
    public int TotalAssignments { get; set; }
    public decimal AverageSubmissionRate { get; set; }
    public GradeDistributionDto GradeDistribution { get; set; } = new();
}
