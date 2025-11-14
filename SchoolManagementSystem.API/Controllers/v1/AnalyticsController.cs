using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Application.Interfaces;
using SchoolManagementSystem.Core.Interfaces;

namespace SchoolManagementSystem.API.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class AnalyticsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMetricsService _metricsService;
        private readonly IAuditService _auditService;
        private readonly ILogger<AnalyticsController> _logger;

        public AnalyticsController(
            IUnitOfWork unitOfWork,
            IMetricsService metricsService,
            IAuditService auditService,
            ILogger<AnalyticsController> logger)
        {
            _unitOfWork = unitOfWork;
            _metricsService = metricsService;
            _auditService = auditService;
            _logger = logger;
        }

        /// <summary>
        /// Get comprehensive dashboard analytics
        /// </summary>
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardAnalytics(CancellationToken cancellationToken)
        {
            var students = await _unitOfWork.Students.GetAllAsync(cancellationToken);
            var courses = await _unitOfWork.Courses.GetAllAsync(cancellationToken);
            var enrollments = await _unitOfWork.Enrollments.GetAllAsync(cancellationToken);
            var grades = await _unitOfWork.Grades.GetAllAsync(cancellationToken);

            var analytics = new
            {
                Overview = new
                {
                    TotalStudents = students.Count(),
                    ActiveStudents = students.Count(s => s.IsActive),
                    TotalCourses = courses.Count(),
                    ActiveCourses = courses.Count(c => c.IsActive),
                    TotalEnrollments = enrollments.Count(),
                    TotalGrades = grades.Count(),
                    AverageGPA = students.Where(s => s.GPA.HasValue).Average(s => s.GPA ?? 0),
                    GradesThisSemester = grades.Count(g => g.CreatedAt >= DateTime.UtcNow.AddMonths(-4))
                },
                EnrollmentTrend = enrollments
                    .GroupBy(e => new { e.EnrollmentDate.Year, e.EnrollmentDate.Month })
                    .Select(g => new
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Count = g.Count()
                    })
                    .OrderByDescending(x => x.Year).ThenByDescending(x => x.Month)
                    .Take(12)
                    .OrderBy(x => x.Year).ThenBy(x => x.Month)
                    .ToList(),
                GradeDistribution = grades
                    .GroupBy(g => g.LetterGrade)
                    .Select(g => new
                    {
                        LetterGrade = g.Key,
                        Count = g.Count(),
                        Percentage = Math.Round((double)g.Count() / grades.Count() * 100, 2)
                    })
                    .OrderBy(x => x.LetterGrade)
                    .ToList(),
                TopPerformingStudents = students
                    .Where(s => s.GPA.HasValue && s.IsActive)
                    .OrderByDescending(s => s.GPA)
                    .Take(10)
                    .Select(s => new
                    {
                        s.StudentNumber,
                        s.FullName,
                        GPA = s.GPA,
                        EnrollmentDate = s.EnrollmentDate
                    })
                    .ToList(),
                MostPopularCourses = enrollments
                    .GroupBy(e => e.CourseId)
                    .Select(g => new
                    {
                        CourseId = g.Key,
                        CourseName = courses.FirstOrDefault(c => c.Id == g.Key)?.Title ?? "Unknown",
                        EnrollmentCount = g.Count()
                    })
                    .OrderByDescending(x => x.EnrollmentCount)
                    .Take(10)
                    .ToList()
            };

            return Ok(analytics);
        }

        /// <summary>
        /// Get enrollment analytics for a specific period
        /// </summary>
        [HttpGet("enrollments")]
        public async Task<IActionResult> GetEnrollmentAnalytics(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            CancellationToken cancellationToken)
        {
            var from = fromDate ?? DateTime.UtcNow.AddMonths(-6);
            var to = toDate ?? DateTime.UtcNow;

            var enrollments = await _unitOfWork.Enrollments.GetAllAsync(cancellationToken);
            var filteredEnrollments = enrollments
                .Where(e => e.EnrollmentDate >= from && e.EnrollmentDate <= to)
                .ToList();

            var analytics = new
            {
                TotalEnrollments = filteredEnrollments.Count,
                ByMonth = filteredEnrollments
                    .GroupBy(e => new { e.EnrollmentDate.Year, e.EnrollmentDate.Month })
                    .Select(g => new
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        MonthName = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM yyyy"),
                        Count = g.Count()
                    })
                    .OrderBy(x => x.Year).ThenBy(x => x.Month)
                    .ToList(),
                ByStatus = filteredEnrollments
                    .GroupBy(e => e.Status)
                    .Select(g => new
                    {
                        Status = g.Key.ToString(),
                        Count = g.Count(),
                        Percentage = Math.Round((double)g.Count() / filteredEnrollments.Count * 100, 2)
                    })
                    .ToList(),
                AverageEnrollmentsPerStudent = filteredEnrollments
                    .GroupBy(e => e.StudentId)
                    .Average(g => g.Count())
            };

            return Ok(analytics);
        }

        /// <summary>
        /// Get grade performance analytics
        /// </summary>
        [HttpGet("grades/performance")]
        public async Task<IActionResult> GetGradePerformanceAnalytics(
            [FromQuery] int? courseId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            CancellationToken cancellationToken)
        {
            var from = fromDate ?? DateTime.UtcNow.AddMonths(-4);
            var to = toDate ?? DateTime.UtcNow;

            var grades = await _unitOfWork.Grades.GetAllAsync(cancellationToken);
            var filteredGrades = grades
                .Where(g => g.GradedAt >= from && g.GradedAt <= to)
                .Where(g => !courseId.HasValue || g.CourseId == courseId.Value)
                .ToList();

            if (!filteredGrades.Any())
            {
                return Ok(new { Message = "No grades found for the specified criteria" });
            }

            var analytics = new
            {
                TotalGrades = filteredGrades.Count,
                AverageScore = Math.Round(filteredGrades.Average(g => g.Percentage), 2),
                HighestScore = filteredGrades.Max(g => g.Percentage),
                LowestScore = filteredGrades.Min(g => g.Percentage),
                MedianScore = CalculateMedian(filteredGrades.Select(g => g.Percentage).ToList()),
                GradeDistribution = filteredGrades
                    .GroupBy(g => g.LetterGrade)
                    .Select(g => new
                    {
                        LetterGrade = g.Key,
                        Count = g.Count(),
                        Percentage = Math.Round((double)g.Count() / filteredGrades.Count * 100, 2),
                        AverageScore = Math.Round(g.Average(x => x.Percentage), 2)
                    })
                    .OrderBy(x => x.LetterGrade)
                    .ToList(),
                ByMonth = filteredGrades
                    .GroupBy(g => new { g.GradedAt.Year, g.GradedAt.Month })
                    .Select(g => new
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        MonthName = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM yyyy"),
                        AverageScore = Math.Round(g.Average(x => x.Percentage), 2),
                        GradeCount = g.Count()
                    })
                    .OrderBy(x => x.Year).ThenBy(x => x.Month)
                    .ToList()
            };

            return Ok(analytics);
        }

        /// <summary>
        /// Get student retention analytics
        /// </summary>
        [HttpGet("retention")]
        public async Task<IActionResult> GetRetentionAnalytics(CancellationToken cancellationToken)
        {
            var students = await _unitOfWork.Students.GetAllAsync(cancellationToken);
            var enrollments = await _unitOfWork.Enrollments.GetAllAsync(cancellationToken);

            var currentSemester = DateTime.UtcNow.AddMonths(-4);
            var previousSemester = DateTime.UtcNow.AddMonths(-8);

            var currentEnrolled = enrollments
                .Where(e => e.EnrollmentDate >= currentSemester)
                .Select(e => e.StudentId)
                .Distinct()
                .ToList();

            var previousEnrolled = enrollments
                .Where(e => e.EnrollmentDate >= previousSemester && e.EnrollmentDate < currentSemester)
                .Select(e => e.StudentId)
                .Distinct()
                .ToList();

            var retained = currentEnrolled.Intersect(previousEnrolled).Count();
            var dropped = previousEnrolled.Except(currentEnrolled).Count();
            var newStudents = currentEnrolled.Except(previousEnrolled).Count();

            var retentionRate = previousEnrolled.Any()
                ? Math.Round((double)retained / previousEnrolled.Count * 100, 2)
                : 0;

            var analytics = new
            {
                CurrentSemesterStudents = currentEnrolled.Count,
                PreviousSemesterStudents = previousEnrolled.Count,
                RetainedStudents = retained,
                DroppedStudents = dropped,
                NewStudents = newStudents,
                RetentionRate = retentionRate,
                DropoutRate = Math.Round(100 - retentionRate, 2),
                GrowthRate = previousEnrolled.Any()
                    ? Math.Round(((double)currentEnrolled.Count - previousEnrolled.Count) / previousEnrolled.Count * 100, 2)
                    : 0
            };

            return Ok(analytics);
        }

        /// <summary>
        /// Get course completion analytics
        /// </summary>
        [HttpGet("completion")]
        public async Task<IActionResult> GetCompletionAnalytics(CancellationToken cancellationToken)
        {
            var enrollments = await _unitOfWork.Enrollments.GetAllAsync(cancellationToken);
            var courses = await _unitOfWork.Courses.GetAllAsync(cancellationToken);

            var completionData = courses.Select(c => new
            {
                CourseId = c.Id,
                CourseCode = c.CourseCode,
                CourseTitle = c.Title,
                TotalEnrollments = enrollments.Count(e => e.CourseId == c.Id),
                Completed = enrollments.Count(e => e.CourseId == c.Id && e.Status == Core.Enums.EnrollmentStatus.Completed),
                Active = enrollments.Count(e => e.CourseId == c.Id && e.Status == Core.Enums.EnrollmentStatus.Active),
                Dropped = enrollments.Count(e => e.CourseId == c.Id && e.Status == Core.Enums.EnrollmentStatus.Dropped),
                CompletionRate = enrollments.Count(e => e.CourseId == c.Id) > 0
                    ? Math.Round((double)enrollments.Count(e => e.CourseId == c.Id && e.Status == Core.Enums.EnrollmentStatus.Completed)
                        / enrollments.Count(e => e.CourseId == c.Id) * 100, 2)
                    : 0
            })
            .Where(c => c.TotalEnrollments > 0)
            .OrderByDescending(c => c.CompletionRate)
            .ToList();

            var analytics = new
            {
                OverallCompletionRate = enrollments.Any()
                    ? Math.Round((double)enrollments.Count(e => e.Status == Core.Enums.EnrollmentStatus.Completed)
                        / enrollments.Count() * 100, 2)
                    : 0,
                TotalCompletedCourses = enrollments.Count(e => e.Status == Core.Enums.EnrollmentStatus.Completed),
                TotalActiveCourses = enrollments.Count(e => e.Status == Core.Enums.EnrollmentStatus.Active),
                TotalDropped = enrollments.Count(e => e.Status == Core.Enums.EnrollmentStatus.Dropped),
                ByCourse = completionData
            };

            return Ok(analytics);
        }

        /// <summary>
        /// Get system usage analytics
        /// </summary>
        [HttpGet("system/usage")]
        public async Task<IActionResult> GetSystemUsageAnalytics(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            CancellationToken cancellationToken)
        {
            var from = fromDate ?? DateTime.UtcNow.AddDays(-30);
            var to = toDate ?? DateTime.UtcNow;

            var auditLogs = await _auditService.GetAuditStatisticsAsync(from, to, cancellationToken);
            var apiMetrics = await _metricsService.GetApiMetricsAsync(from, to, cancellationToken);

            return Ok(new
            {
                Period = new { From = from, To = to },
                AuditStatistics = auditLogs.Data,
                ApiMetrics = apiMetrics.Data
            });
        }

        private double CalculateMedian(List<decimal> values)
        {
            if (!values.Any()) return 0;

            var sorted = values.OrderBy(x => x).ToList();
            var count = sorted.Count;

            if (count % 2 == 0)
            {
                return (double)(sorted[count / 2 - 1] + sorted[count / 2]) / 2;
            }
            else
            {
                return (double)sorted[count / 2];
            }
        }
    }
}
