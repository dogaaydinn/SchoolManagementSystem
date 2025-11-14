using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Core.DTOs;
using SchoolManagementSystem.Core.Interfaces;

namespace SchoolManagementSystem.API.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
public class GradesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GradesController> _logger;

    public GradesController(IUnitOfWork unitOfWork, ILogger<GradesController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
    [ProducesResponseType(typeof(ApiResponse<GradeDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateGrade([FromBody] CreateGradeRequestDto request)
    {
        try
        {
            var student = await _unitOfWork.Students.GetByIdAsync(request.StudentId);
            if (student == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Student not found", 404));
            }

            var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);
            if (course == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Course not found", 404));
            }

            var letterGrade = CalculateLetterGrade(request.Value);

            var grade = new Core.Entities.Grade
            {
                StudentId = request.StudentId,
                CourseId = request.CourseId,
                EnrollmentId = request.EnrollmentId,
                AssignmentId = request.AssignmentId,
                GradeType = request.GradeType,
                Value = request.Value,
                MaxValue = request.MaxValue,
                LetterGrade = letterGrade,
                Weight = request.Weight,
                GradeDate = DateTime.UtcNow,
                Comments = request.Comments,
                IsPublished = request.IsPublished,
                GradedBy = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0")
            };

            await _unitOfWork.Grades.AddAsync(grade);
            await _unitOfWork.SaveChangesAsync();

            // Recalculate student GPA
            await RecalculateStudentGPA(request.StudentId);

            _logger.LogInformation("Grade created for student {StudentId} in course {CourseId}", request.StudentId, request.CourseId);

            var gradeDto = new GradeDto
            {
                Id = grade.Id,
                StudentId = grade.StudentId,
                CourseId = grade.CourseId,
                CourseName = course.CourseName,
                CourseCode = course.CourseCode,
                GradeType = grade.GradeType,
                Value = grade.Value,
                MaxValue = grade.MaxValue,
                LetterGrade = grade.LetterGrade,
                Weight = grade.Weight,
                GradeDate = grade.GradeDate,
                IsPublished = grade.IsPublished
            };

            return CreatedAtAction(nameof(GetGrade), new { id = grade.Id },
                ApiResponse<GradeDto>.SuccessResponse(gradeDto, "Grade created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating grade");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error creating grade", 500));
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<GradeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetGrade(int id)
    {
        try
        {
            var grade = await _unitOfWork.Grades.GetByIdAsync(id);
            if (grade == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Grade not found", 404));
            }

            var course = await _unitOfWork.Courses.GetByIdAsync(grade.CourseId);

            var gradeDto = new GradeDto
            {
                Id = grade.Id,
                StudentId = grade.StudentId,
                CourseId = grade.CourseId,
                CourseName = course?.CourseName ?? "Unknown",
                CourseCode = course?.CourseCode ?? "Unknown",
                GradeType = grade.GradeType,
                Value = grade.Value,
                MaxValue = grade.MaxValue,
                LetterGrade = grade.LetterGrade,
                Weight = grade.Weight,
                GradeDate = grade.GradeDate,
                Comments = grade.Comments,
                IsPublished = grade.IsPublished
            };

            return Ok(ApiResponse<GradeDto>.SuccessResponse(gradeDto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving grade {GradeId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error retrieving grade", 500));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
    [ProducesResponseType(typeof(ApiResponse<GradeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateGrade(int id, [FromBody] UpdateGradeRequestDto request)
    {
        try
        {
            var grade = await _unitOfWork.Grades.GetByIdAsync(id);
            if (grade == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Grade not found", 404));
            }

            if (request.Value.HasValue)
            {
                grade.Value = request.Value.Value;
                grade.LetterGrade = CalculateLetterGrade(grade.Value);
            }
            if (request.MaxValue.HasValue)
                grade.MaxValue = request.MaxValue.Value;
            if (request.Weight.HasValue)
                grade.Weight = request.Weight.Value;
            if (!string.IsNullOrEmpty(request.Comments))
                grade.Comments = request.Comments;
            if (request.IsPublished.HasValue)
                grade.IsPublished = request.IsPublished.Value;

            _unitOfWork.Grades.Update(grade);
            await _unitOfWork.SaveChangesAsync();

            // Recalculate student GPA
            await RecalculateStudentGPA(grade.StudentId);

            _logger.LogInformation("Grade updated: {GradeId}", id);

            return Ok(ApiResponse<GradeDto>.SuccessResponse(new GradeDto
            {
                Id = grade.Id,
                Value = grade.Value,
                LetterGrade = grade.LetterGrade
            }, "Grade updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating grade {GradeId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error updating grade", 500));
        }
    }

    [HttpPost("bulk")]
    [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status201Created)]
    public async Task<IActionResult> BulkCreateGrades([FromBody] BulkGradeRequestDto request)
    {
        try
        {
            var successCount = 0;
            var failedCount = 0;

            await _unitOfWork.BeginTransactionAsync();

            foreach (var studentGrade in request.StudentGrades)
            {
                try
                {
                    var grade = new Core.Entities.Grade
                    {
                        StudentId = studentGrade.StudentId,
                        CourseId = request.CourseId,
                        AssignmentId = request.AssignmentId,
                        GradeType = request.GradeType,
                        Value = studentGrade.Value,
                        MaxValue = request.MaxValue,
                        LetterGrade = CalculateLetterGrade(studentGrade.Value),
                        Weight = request.Weight,
                        GradeDate = DateTime.UtcNow,
                        Comments = studentGrade.Comments,
                        IsPublished = true,
                        GradedBy = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0")
                    };

                    await _unitOfWork.Grades.AddAsync(grade);
                    successCount++;
                }
                catch
                {
                    failedCount++;
                }
            }

            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation("Bulk grades created: {SuccessCount} successful, {FailedCount} failed", successCount, failedCount);

            return Ok(ApiResponse<object>.SuccessResponse(new
            {
                totalGrades = request.StudentGrades.Count,
                successfulSubmissions = successCount,
                failedSubmissions = failedCount
            }, "Grades submitted successfully"));
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error in bulk grade creation");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error creating grades", 500));
        }
    }

    [HttpGet("student/{studentId}")]
    [ProducesResponseType(typeof(ApiResponse<List<GradeDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStudentGrades(int studentId)
    {
        try
        {
            var grades = await _unitOfWork.Grades.FindAsync(g => g.StudentId == studentId && g.IsPublished);

            var gradeDtos = new List<GradeDto>();
            foreach (var grade in grades)
            {
                var course = await _unitOfWork.Courses.GetByIdAsync(grade.CourseId);
                gradeDtos.Add(new GradeDto
                {
                    Id = grade.Id,
                    StudentId = grade.StudentId,
                    CourseId = grade.CourseId,
                    CourseName = course?.CourseName ?? "Unknown",
                    CourseCode = course?.CourseCode ?? "Unknown",
                    GradeType = grade.GradeType,
                    Value = grade.Value,
                    MaxValue = grade.MaxValue,
                    LetterGrade = grade.LetterGrade,
                    Weight = grade.Weight,
                    GradeDate = grade.GradeDate,
                    IsPublished = grade.IsPublished
                });
            }

            return Ok(ApiResponse<List<GradeDto>>.SuccessResponse(gradeDtos));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving grades for student {StudentId}", studentId);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error retrieving grades", 500));
        }
    }

    [HttpGet("course/{courseId}")]
    [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
    [ProducesResponseType(typeof(ApiResponse<List<GradeDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCourseGrades(int courseId)
    {
        try
        {
            var grades = await _unitOfWork.Grades.FindAsync(g => g.CourseId == courseId);

            var gradeDtos = new List<GradeDto>();
            foreach (var grade in grades)
            {
                var student = await _unitOfWork.Students.GetByIdWithIncludesAsync(grade.StudentId, s => s.User);
                gradeDtos.Add(new GradeDto
                {
                    Id = grade.Id,
                    StudentId = grade.StudentId,
                    StudentName = student?.User.FullName ?? "Unknown",
                    CourseId = grade.CourseId,
                    GradeType = grade.GradeType,
                    Value = grade.Value,
                    MaxValue = grade.MaxValue,
                    LetterGrade = grade.LetterGrade,
                    IsPublished = grade.IsPublished
                });
            }

            return Ok(ApiResponse<List<GradeDto>>.SuccessResponse(gradeDtos));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving grades for course {CourseId}", courseId);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error retrieving grades", 500));
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

    private async Task RecalculateStudentGPA(int studentId)
    {
        var student = await _unitOfWork.Students.GetByIdAsync(studentId);
        if (student == null) return;

        var enrollments = await _unitOfWork.Enrollments.FindAsync(
            e => e.StudentId == studentId && e.Status == "Completed"
        );

        decimal totalPoints = 0;
        int totalCredits = 0;

        foreach (var enrollment in enrollments)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(enrollment.CourseId);
            if (course == null || !enrollment.FinalGrade.HasValue) continue;

            var gradePoint = enrollment.FinalGrade.Value / 25; // Convert 0-100 to 0-4 scale
            totalPoints += gradePoint * course.Credits;
            totalCredits += course.Credits;
        }

        if (totalCredits > 0)
        {
            student.GPA = totalPoints / totalCredits;
            _unitOfWork.Students.Update(student);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
