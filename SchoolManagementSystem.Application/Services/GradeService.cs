using AutoMapper;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Application.Interfaces;
using SchoolManagementSystem.Core.DTOs;
using SchoolManagementSystem.Core.Entities;
using SchoolManagementSystem.Core.Interfaces;

namespace SchoolManagementSystem.Application.Services;

public class GradeService : IGradeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GradeService> _logger;
    private readonly INotificationService _notificationService;

    public GradeService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GradeService> logger,
        INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _notificationService = notificationService;
    }

    public async Task<ApiResponse<GradeDto>> CreateGradeAsync(
        CreateGradeRequestDto request,
        string gradedBy,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var student = await _unitOfWork.Students.GetByIdAsync(request.StudentId, cancellationToken);
            if (student == null)
                return ApiResponse<GradeDto>.ErrorResponse("Student not found", 404);

            var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId, cancellationToken);
            if (course == null)
                return ApiResponse<GradeDto>.ErrorResponse("Course not found", 404);

            // Check if student is enrolled in the course
            var enrollment = await _unitOfWork.Enrollments.FindAsync(
                e => e.StudentId == request.StudentId &&
                     e.CourseId == request.CourseId &&
                     e.Status == "Active",
                cancellationToken
            );

            if (!enrollment.Any())
                return ApiResponse<GradeDto>.ErrorResponse("Student not enrolled in this course", 400);

            // Calculate percentage and letter grade
            var percentage = (request.Value / request.MaxValue) * 100;
            var letterGrade = CalculateLetterGrade(percentage);

            var grade = new Grade
            {
                StudentId = request.StudentId,
                CourseId = request.CourseId,
                Value = request.Value,
                MaxValue = request.MaxValue,
                Percentage = percentage,
                LetterGrade = letterGrade,
                GradeType = request.GradeType,
                Weight = request.Weight,
                GradedBy = gradedBy,
                Comments = request.Comments,
                GradeDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Grades.AddAsync(grade, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Recalculate student GPA
            await RecalculateStudentGPAAsync(request.StudentId, cancellationToken);

            // Send notification to student
            await _notificationService.NotifyGradePostedAsync(
                request.StudentId,
                grade.Id,
                cancellationToken
            );

            _logger.LogInformation(
                "Grade created for student {StudentId} in course {CourseId} by {GradedBy}",
                request.StudentId,
                request.CourseId,
                gradedBy
            );

            var gradeDto = _mapper.Map<GradeDto>(grade);
            return ApiResponse<GradeDto>.SuccessResponse(gradeDto, "Grade created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating grade");
            return ApiResponse<GradeDto>.ErrorResponse("Error creating grade", 500);
        }
    }

    public async Task<ApiResponse<IEnumerable<GradeDto>>> BulkCreateGradesAsync(
        BulkGradeRequestDto request,
        string gradedBy,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId, cancellationToken);
            if (course == null)
                return ApiResponse<IEnumerable<GradeDto>>.ErrorResponse("Course not found", 404);

            var grades = new List<Grade>();
            var createdGrades = new List<GradeDto>();

            await _unitOfWork.BeginTransactionAsync();

            foreach (var studentGrade in request.StudentGrades)
            {
                var student = await _unitOfWork.Students.GetByIdAsync(studentGrade.StudentId, cancellationToken);
                if (student == null)
                {
                    _logger.LogWarning("Student {StudentId} not found, skipping", studentGrade.StudentId);
                    continue;
                }

                var percentage = (studentGrade.Value / request.MaxValue) * 100;
                var letterGrade = CalculateLetterGrade(percentage);

                var grade = new Grade
                {
                    StudentId = studentGrade.StudentId,
                    CourseId = request.CourseId,
                    Value = studentGrade.Value,
                    MaxValue = request.MaxValue,
                    Percentage = percentage,
                    LetterGrade = letterGrade,
                    GradeType = request.GradeType,
                    Weight = request.Weight,
                    GradedBy = gradedBy,
                    GradeDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Grades.AddAsync(grade, cancellationToken);
                grades.Add(grade);

                // Recalculate GPA for each student
                await RecalculateStudentGPAAsync(studentGrade.StudentId, cancellationToken);

                // Send notification
                await _notificationService.NotifyGradePostedAsync(
                    studentGrade.StudentId,
                    grade.Id,
                    cancellationToken
                );
            }

            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation(
                "Bulk created {Count} grades for course {CourseId} by {GradedBy}",
                grades.Count,
                request.CourseId,
                gradedBy
            );

            var gradeDtos = _mapper.Map<IEnumerable<GradeDto>>(grades);
            return ApiResponse<IEnumerable<GradeDto>>.SuccessResponse(
                gradeDtos,
                $"{grades.Count} grades created successfully"
            );
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error bulk creating grades");
            return ApiResponse<IEnumerable<GradeDto>>.ErrorResponse("Error creating grades", 500);
        }
    }

    public async Task<ApiResponse<GradeDto>> UpdateGradeAsync(
        int id,
        UpdateGradeRequestDto request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var grade = await _unitOfWork.Grades.GetByIdAsync(id, cancellationToken);
            if (grade == null)
                return ApiResponse<GradeDto>.ErrorResponse("Grade not found", 404);

            if (request.Value.HasValue)
                grade.Value = request.Value.Value;
            if (request.MaxValue.HasValue)
                grade.MaxValue = request.MaxValue.Value;

            // Recalculate percentage and letter grade
            grade.Percentage = (grade.Value / grade.MaxValue) * 100;
            grade.LetterGrade = CalculateLetterGrade(grade.Percentage);

            if (!string.IsNullOrEmpty(request.Comments))
                grade.Comments = request.Comments;

            grade.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Grades.Update(grade);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Recalculate student GPA
            await RecalculateStudentGPAAsync(grade.StudentId, cancellationToken);

            _logger.LogInformation("Updated grade {GradeId}", id);

            var gradeDto = _mapper.Map<GradeDto>(grade);
            return ApiResponse<GradeDto>.SuccessResponse(gradeDto, "Grade updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating grade {GradeId}", id);
            return ApiResponse<GradeDto>.ErrorResponse("Error updating grade", 500);
        }
    }

    public async Task<ApiResponse<bool>> DeleteGradeAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var grade = await _unitOfWork.Grades.GetByIdAsync(id, cancellationToken);
            if (grade == null)
                return ApiResponse<bool>.ErrorResponse("Grade not found", 404);

            var studentId = grade.StudentId;

            _unitOfWork.Grades.Remove(grade);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Recalculate student GPA
            await RecalculateStudentGPAAsync(studentId, cancellationToken);

            _logger.LogInformation("Deleted grade {GradeId}", id);

            return ApiResponse<bool>.SuccessResponse(true, "Grade deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting grade {GradeId}", id);
            return ApiResponse<bool>.ErrorResponse("Error deleting grade", 500);
        }
    }

    public async Task<ApiResponse<IEnumerable<GradeDto>>> GetStudentGradesAsync(
        int studentId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var grades = await _unitOfWork.Grades.FindAsync(
                g => g.StudentId == studentId,
                cancellationToken
            );

            var gradeDtos = _mapper.Map<IEnumerable<GradeDto>>(grades);
            return ApiResponse<IEnumerable<GradeDto>>.SuccessResponse(gradeDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting grades for student {StudentId}", studentId);
            return ApiResponse<IEnumerable<GradeDto>>.ErrorResponse("Error retrieving grades", 500);
        }
    }

    public async Task<ApiResponse<IEnumerable<GradeDto>>> GetCourseGradesAsync(
        int courseId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var grades = await _unitOfWork.Grades.FindAsync(
                g => g.CourseId == courseId,
                cancellationToken
            );

            var gradeDtos = _mapper.Map<IEnumerable<GradeDto>>(grades);
            return ApiResponse<IEnumerable<GradeDto>>.SuccessResponse(gradeDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting grades for course {CourseId}", courseId);
            return ApiResponse<IEnumerable<GradeDto>>.ErrorResponse("Error retrieving grades", 500);
        }
    }

    public async Task<ApiResponse<decimal>> CalculateStudentGPAAsync(
        int studentId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var student = await _unitOfWork.Students.GetByIdAsync(studentId, cancellationToken);
            if (student == null)
                return ApiResponse<decimal>.ErrorResponse("Student not found", 404);

            var gpa = await RecalculateStudentGPAAsync(studentId, cancellationToken);
            return ApiResponse<decimal>.SuccessResponse(gpa);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating GPA for student {StudentId}", studentId);
            return ApiResponse<decimal>.ErrorResponse("Error calculating GPA", 500);
        }
    }

    public async Task<ApiResponse<GradeDistributionDto>> GetGradeDistributionAsync(
        int courseId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var grades = await _unitOfWork.Grades.FindAsync(
                g => g.CourseId == courseId,
                cancellationToken
            );

            var distribution = new GradeDistributionDto
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

            return ApiResponse<GradeDistributionDto>.SuccessResponse(distribution);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting grade distribution for course {CourseId}", courseId);
            return ApiResponse<GradeDistributionDto>.ErrorResponse(
                "Error retrieving grade distribution",
                500
            );
        }
    }

    private string CalculateLetterGrade(decimal percentage)
    {
        return percentage switch
        {
            >= 93 => "A",
            >= 90 => "A-",
            >= 87 => "B+",
            >= 83 => "B",
            >= 80 => "B-",
            >= 77 => "C+",
            >= 73 => "C",
            >= 70 => "C-",
            >= 67 => "D+",
            >= 63 => "D",
            >= 60 => "D-",
            _ => "F"
        };
    }

    private decimal ConvertLetterGradeToGPA(string letterGrade)
    {
        return letterGrade switch
        {
            "A" => 4.0m,
            "A-" => 3.7m,
            "B+" => 3.3m,
            "B" => 3.0m,
            "B-" => 2.7m,
            "C+" => 2.3m,
            "C" => 2.0m,
            "C-" => 1.7m,
            "D+" => 1.3m,
            "D" => 1.0m,
            "D-" => 0.7m,
            "F" => 0.0m,
            _ => 0.0m
        };
    }

    private async Task<decimal> RecalculateStudentGPAAsync(
        int studentId,
        CancellationToken cancellationToken = default)
    {
        var enrollments = await _unitOfWork.Enrollments.FindAsync(
            e => e.StudentId == studentId && e.Status == "Completed",
            cancellationToken
        );

        if (!enrollments.Any())
            return 0.0m;

        decimal totalGradePoints = 0;
        int totalCredits = 0;

        foreach (var enrollment in enrollments)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(enrollment.CourseId, cancellationToken);
            if (course == null) continue;

            var finalGrade = await _unitOfWork.Grades.FindAsync(
                g => g.StudentId == studentId &&
                     g.CourseId == enrollment.CourseId &&
                     g.GradeType == "Final",
                cancellationToken
            );

            if (finalGrade.Any())
            {
                var grade = finalGrade.First();
                var gradePoint = ConvertLetterGradeToGPA(grade.LetterGrade ?? "F");
                totalGradePoints += gradePoint * course.Credits;
                totalCredits += course.Credits;
            }
        }

        var gpa = totalCredits > 0 ? Math.Round(totalGradePoints / totalCredits, 2) : 0.0m;

        // Update student GPA
        var student = await _unitOfWork.Students.GetByIdAsync(studentId, cancellationToken);
        if (student != null)
        {
            student.GPA = gpa;
            student.TotalCreditsEarned = totalCredits;
            student.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Students.Update(student);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return gpa;
    }
}
