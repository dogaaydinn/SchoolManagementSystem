using AutoMapper;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Application.Interfaces;
using SchoolManagementSystem.Core.DTOs;
using SchoolManagementSystem.Core.Interfaces;

namespace SchoolManagementSystem.Application.Services;

public class ReportingService : IReportingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ReportingService> _logger;

    public ReportingService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<ReportingService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<StudentPerformanceReportDto>> GetStudentPerformanceReportAsync(
        int studentId,
        int? semesterId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var student = await _unitOfWork.Students.GetByIdAsync(studentId, cancellationToken);
            if (student == null)
                return ApiResponse<StudentPerformanceReportDto>.ErrorResponse("Student not found", 404);

            var enrollments = await _unitOfWork.Enrollments.FindAsync(
                e => e.StudentId == studentId &&
                     (!semesterId.HasValue || e.Course.SemesterId == semesterId),
                cancellationToken
            );

            var grades = await _unitOfWork.Grades.FindAsync(
                g => g.StudentId == studentId,
                cancellationToken
            );

            var attendances = await _unitOfWork.Attendances.FindAsync(
                a => a.StudentId == studentId,
                cancellationToken
            );

            var coursePerformances = new List<CoursePerformanceDto>();

            foreach (var enrollment in enrollments)
            {
                var course = await _unitOfWork.Courses.GetByIdAsync(enrollment.CourseId, cancellationToken);
                if (course == null) continue;

                var courseGrades = grades.Where(g => g.CourseId == enrollment.CourseId).ToList();
                var courseAttendances = attendances.Where(a => a.CourseId == enrollment.CourseId).ToList();

                var finalGrade = courseGrades.FirstOrDefault(g => g.GradeType == "Final");

                var coursePerformance = new CoursePerformanceDto
                {
                    CourseId = course.Id,
                    CourseName = course.CourseName,
                    CourseCode = course.CourseCode,
                    FinalGrade = finalGrade?.Percentage,
                    LetterGrade = finalGrade?.LetterGrade,
                    Credits = course.Credits,
                    AttendancePercentage = courseAttendances.Any()
                        ? (decimal)courseAttendances.Count(a => a.Status == "Present") / courseAttendances.Count * 100
                        : 0,
                    AssignmentsCompleted = 0, // Would calculate from submissions
                    TotalAssignments = 0 // Would get from course assignments
                };

                coursePerformances.Add(coursePerformance);
            }

            var report = new StudentPerformanceReportDto
            {
                StudentId = student.Id,
                StudentName = student.User.FullName,
                StudentNumber = student.StudentNumber,
                OverallGPA = student.GPA,
                TotalCreditsEarned = student.TotalCreditsEarned,
                TotalCreditsRequired = student.TotalCreditsRequired,
                CompletionPercentage = student.TotalCreditsRequired > 0
                    ? (decimal)student.TotalCreditsEarned / student.TotalCreditsRequired * 100
                    : 0,
                CoursePerformances = coursePerformances,
                AttendanceSummary = new AttendanceSummaryDto
                {
                    TotalClasses = attendances.Count(),
                    Present = attendances.Count(a => a.Status == "Present"),
                    Absent = attendances.Count(a => a.Status == "Absent"),
                    Late = attendances.Count(a => a.Status == "Late"),
                    Excused = attendances.Count(a => a.Status == "Excused"),
                    AttendancePercentage = attendances.Any()
                        ? (decimal)attendances.Count(a => a.Status == "Present") / attendances.Count() * 100
                        : 0
                },
                RecentGrades = _mapper.Map<List<GradeDto>>(grades.OrderByDescending(g => g.GradeDate).Take(10)),
                AcademicStanding = DetermineAcademicStanding(student.GPA)
            };

            return ApiResponse<StudentPerformanceReportDto>.SuccessResponse(report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating student performance report for {StudentId}", studentId);
            return ApiResponse<StudentPerformanceReportDto>.ErrorResponse(
                "Error generating performance report",
                500
            );
        }
    }

    public async Task<ApiResponse<CoursePerformanceReportDto>> GetCoursePerformanceReportAsync(
        int courseId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(courseId, cancellationToken);
            if (course == null)
                return ApiResponse<CoursePerformanceReportDto>.ErrorResponse("Course not found", 404);

            var enrollments = await _unitOfWork.Enrollments.FindAsync(
                e => e.CourseId == courseId,
                cancellationToken
            );

            var grades = await _unitOfWork.Grades.FindAsync(
                g => g.CourseId == courseId,
                cancellationToken
            );

            var attendances = await _unitOfWork.Attendances.FindAsync(
                a => a.CourseId == courseId,
                cancellationToken
            );

            var assignments = await _unitOfWork.Assignments.FindAsync(
                a => a.CourseId == courseId,
                cancellationToken
            );

            var gradeDistribution = new GradeDistributionDto
            {
                ACount = grades.Count(g => g.LetterGrade?.StartsWith("A") == true),
                BCount = grades.Count(g => g.LetterGrade?.StartsWith("B") == true),
                CCount = grades.Count(g => g.LetterGrade?.StartsWith("C") == true),
                DCount = grades.Count(g => g.LetterGrade?.StartsWith("D") == true),
                FCount = grades.Count(g => g.LetterGrade == "F"),
                DetailedDistribution = grades
                    .GroupBy(g => g.LetterGrade ?? "N/A")
                    .ToDictionary(g => g.Key, g => g.Count())
            };

            var report = new CoursePerformanceReportDto
            {
                CourseId = course.Id,
                CourseName = course.CourseName,
                CourseCode = course.CourseCode,
                TeacherName = course.Teacher?.User.FullName ?? "Not Assigned",
                TotalStudents = enrollments.Count(),
                ActiveStudents = enrollments.Count(e => e.Status == "Active"),
                DroppedStudents = enrollments.Count(e => e.Status == "Dropped"),
                AverageGrade = grades.Any() ? grades.Average(g => g.Percentage) : 0,
                PassRate = grades.Any()
                    ? (decimal)grades.Count(g => g.LetterGrade != "F") / grades.Count() * 100
                    : 0,
                GradeDistribution = gradeDistribution,
                AverageAttendancePercentage = attendances.Any()
                    ? (decimal)attendances.Count(a => a.Status == "Present") / attendances.Count() * 100
                    : 0,
                TotalAssignments = assignments.Count(),
                AverageSubmissionRate = 0 // Would calculate from submissions
            };

            return ApiResponse<CoursePerformanceReportDto>.SuccessResponse(report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating course performance report for {CourseId}", courseId);
            return ApiResponse<CoursePerformanceReportDto>.ErrorResponse(
                "Error generating course report",
                500
            );
        }
    }

    public async Task<ApiResponse<TeacherPerformanceReportDto>> GetTeacherPerformanceReportAsync(
        int teacherId,
        int? semesterId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var teacher = await _unitOfWork.Teachers.GetByIdAsync(teacherId, cancellationToken);
            if (teacher == null)
                return ApiResponse<TeacherPerformanceReportDto>.ErrorResponse("Teacher not found", 404);

            var courses = await _unitOfWork.Courses.FindAsync(
                c => c.TeacherId == teacherId &&
                     (!semesterId.HasValue || c.SemesterId == semesterId),
                cancellationToken
            );

            var courseReports = new List<CoursePerformanceReportDto>();
            int totalStudents = 0;

            foreach (var course in courses)
            {
                var courseReport = await GetCoursePerformanceReportAsync(course.Id, cancellationToken);
                if (courseReport.Success && courseReport.Data != null)
                {
                    courseReports.Add(courseReport.Data);
                    totalStudents += courseReport.Data.TotalStudents;
                }
            }

            var advisees = await _unitOfWork.Students.FindAsync(
                s => s.AdvisorId == teacherId,
                cancellationToken
            );

            var report = new TeacherPerformanceReportDto
            {
                TeacherId = teacher.Id,
                TeacherName = teacher.User.FullName,
                EmployeeNumber = teacher.EmployeeNumber,
                DepartmentName = teacher.Department?.Name ?? "Not Assigned",
                TotalCourses = courses.Count(),
                TotalStudents = totalStudents,
                AverageClassSize = courses.Any() ? (decimal)totalStudents / courses.Count() : 0,
                AverageStudentGrade = courseReports.Any()
                    ? courseReports.Average(r => r.AverageGrade)
                    : 0,
                StudentPassRate = courseReports.Any()
                    ? courseReports.Average(r => r.PassRate)
                    : 0,
                CourseReports = courseReports,
                TotalAdvisees = advisees.Count(),
                AssignmentCreationRate = 0, // Would calculate assignments per course
                AssignmentGradingTimeliness = 0 // Would calculate average grading time
            };

            return ApiResponse<TeacherPerformanceReportDto>.SuccessResponse(report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating teacher performance report for {TeacherId}", teacherId);
            return ApiResponse<TeacherPerformanceReportDto>.ErrorResponse(
                "Error generating teacher report",
                500
            );
        }
    }

    public async Task<ApiResponse<DepartmentReportDto>> GetDepartmentReportAsync(
        int departmentId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var department = await _unitOfWork.Departments.GetByIdAsync(departmentId, cancellationToken);
            if (department == null)
                return ApiResponse<DepartmentReportDto>.ErrorResponse("Department not found", 404);

            var teachers = await _unitOfWork.Teachers.FindAsync(
                t => t.DepartmentId == departmentId,
                cancellationToken
            );

            var courses = await _unitOfWork.Courses.FindAsync(
                c => c.DepartmentId == departmentId,
                cancellationToken
            );

            var students = await _unitOfWork.Students.FindAsync(
                s => s.Major == department.Name || s.Minor == department.Name,
                cancellationToken
            );

            var report = new DepartmentReportDto
            {
                DepartmentId = department.Id,
                DepartmentName = department.Name,
                DepartmentCode = department.DepartmentCode,
                TotalTeachers = teachers.Count(),
                TotalStudents = students.Count(),
                TotalCourses = courses.Count(),
                ActiveCourses = courses.Count(c => c.Status == "Active"),
                AverageDepartmentGPA = students.Any() ? students.Average(s => s.GPA) : 0,
                StudentRetentionRate = 0, // Would calculate based on enrollment history
                EnrollmentByLevel = courses
                    .GroupBy(c => c.Level)
                    .ToDictionary(g => g.Key, g => g.Count()),
                TopPerformingStudents = students
                    .OrderByDescending(s => s.GPA)
                    .Take(10)
                    .Select(s => new TopPerformingStudentDto
                    {
                        StudentId = s.Id,
                        StudentName = s.User.FullName,
                        GPA = s.GPA,
                        CreditsEarned = s.TotalCreditsEarned
                    })
                    .ToList(),
                CourseEnrollmentStats = new List<CourseEnrollmentStatDto>()
            };

            return ApiResponse<DepartmentReportDto>.SuccessResponse(report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating department report for {DepartmentId}", departmentId);
            return ApiResponse<DepartmentReportDto>.ErrorResponse(
                "Error generating department report",
                500
            );
        }
    }

    public async Task<ApiResponse<AttendanceReportDto>> GetAttendanceReportAsync(
        int studentId,
        int? courseId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var student = await _unitOfWork.Students.GetByIdAsync(studentId, cancellationToken);
            if (student == null)
                return ApiResponse<AttendanceReportDto>.ErrorResponse("Student not found", 404);

            var attendances = await _unitOfWork.Attendances.FindAsync(
                a => a.StudentId == studentId &&
                     (!courseId.HasValue || a.CourseId == courseId),
                cancellationToken
            );

            var report = new AttendanceReportDto
            {
                TotalClasses = attendances.Count(),
                Present = attendances.Count(a => a.Status == "Present"),
                Absent = attendances.Count(a => a.Status == "Absent"),
                Late = attendances.Count(a => a.Status == "Late"),
                Excused = attendances.Count(a => a.Status == "Excused"),
                AttendancePercentage = attendances.Any()
                    ? (decimal)attendances.Count(a => a.Status == "Present") / attendances.Count() * 100
                    : 0
            };

            return ApiResponse<AttendanceReportDto>.SuccessResponse(report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating attendance report for student {StudentId}", studentId);
            return ApiResponse<AttendanceReportDto>.ErrorResponse(
                "Error generating attendance report",
                500
            );
        }
    }

    public async Task<ApiResponse<EnrollmentReportDto>> GetEnrollmentReportAsync(
        int? departmentId = null,
        int? semesterId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var enrollments = await _unitOfWork.Enrollments.FindAsync(
                e => (!semesterId.HasValue || e.Course.SemesterId == semesterId) &&
                     (!departmentId.HasValue || e.Course.DepartmentId == departmentId),
                cancellationToken
            );

            var report = new EnrollmentReportDto
            {
                TotalEnrollments = enrollments.Count(),
                ActiveEnrollments = enrollments.Count(e => e.Status == "Active"),
                CompletedEnrollments = enrollments.Count(e => e.Status == "Completed"),
                DroppedEnrollments = enrollments.Count(e => e.Status == "Dropped"),
                EnrollmentsByDepartment = new Dictionary<string, int>(),
                EnrollmentsBySemester = new Dictionary<string, int>(),
                EnrollmentTrends = new List<EnrollmentTrendDto>()
            };

            return ApiResponse<EnrollmentReportDto>.SuccessResponse(report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating enrollment report");
            return ApiResponse<EnrollmentReportDto>.ErrorResponse(
                "Error generating enrollment report",
                500
            );
        }
    }

    public async Task<ApiResponse<byte[]>> ExportReportToPdfAsync(
        string reportType,
        int entityId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // This would use a PDF library like iTextSharp or QuestPDF
            // For now, returning a placeholder
            _logger.LogInformation("Exporting {ReportType} to PDF for entity {EntityId}", reportType, entityId);

            return ApiResponse<byte[]>.ErrorResponse(
                "PDF export not implemented yet",
                501
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting report to PDF");
            return ApiResponse<byte[]>.ErrorResponse("Error exporting to PDF", 500);
        }
    }

    public async Task<ApiResponse<byte[]>> ExportReportToExcelAsync(
        string reportType,
        int entityId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // This would use a library like EPPlus or ClosedXML
            // For now, returning a placeholder
            _logger.LogInformation("Exporting {ReportType} to Excel for entity {EntityId}", reportType, entityId);

            return ApiResponse<byte[]>.ErrorResponse(
                "Excel export not implemented yet",
                501
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting report to Excel");
            return ApiResponse<byte[]>.ErrorResponse("Error exporting to Excel", 500);
        }
    }

    private string DetermineAcademicStanding(decimal gpa)
    {
        return gpa switch
        {
            >= 3.5m => "Dean's List",
            >= 3.0m => "Good Standing",
            >= 2.0m => "Satisfactory",
            >= 1.0m => "Academic Probation",
            _ => "Academic Warning"
        };
    }
}
