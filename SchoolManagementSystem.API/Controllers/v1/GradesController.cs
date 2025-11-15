using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Core.Constants;
using SchoolManagementSystem.Core.DTOs;
using SchoolManagementSystem.Core.Interfaces;
using System.Security.Claims;

namespace SchoolManagementSystem.API.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize]
public class GradesController : ControllerBase
{
    private readonly IGradeService _gradeService;
    private readonly ILogger<GradesController> _logger;

    public GradesController(IGradeService gradeService, ILogger<GradesController> logger)
    {
        _gradeService = gradeService;
        _logger = logger;
    }

    /// <summary>
    /// Gets a paginated list of all grades with optional filtering
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <param name="studentId">Filter by student ID</param>
    /// <param name="courseId">Filter by course ID</param>
    /// <param name="assignmentId">Filter by assignment ID</param>
    /// <param name="isPublished">Filter by published status</param>
    /// <returns>Paginated list of grades</returns>
    [HttpGet]
    [Authorize(Policy = PolicyConstants.AdminOrTeacher)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<GradeDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllGrades(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? studentId = null,
        [FromQuery] int? courseId = null,
        [FromQuery] int? assignmentId = null,
        [FromQuery] bool? isPublished = null)
    {
        try
        {
            // Validate pagination parameters
            if (pageNumber < 1)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Page number must be greater than 0", 400));
            }

            if (pageSize < 1 || pageSize > 100)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Page size must be between 1 and 100", 400));
            }

            var result = await _gradeService.GetAllGradesAsync(pageNumber, pageSize, studentId, courseId, assignmentId, isPublished);

            _logger.LogInformation("Retrieved {Count} grades (page {PageNumber}/{TotalPages})",
                result.Items.Count, result.PageNumber, result.TotalPages);

            return Ok(ApiResponse<PagedResult<GradeDto>>.SuccessResponse(
                result,
                $"Retrieved {result.Items.Count} grades"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving grades list");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while retrieving grades", 500));
        }
    }

    /// <summary>
    /// Gets detailed information about a specific grade by ID
    /// </summary>
    /// <param name="id">Grade ID</param>
    /// <returns>Grade details</returns>
    [HttpGet("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<GradeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetGradeById(int id)
    {
        try
        {
            var grade = await _gradeService.GetGradeByIdAsync(id);

            if (grade == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Grade not found", 404));
            }

            // Allow students to view their own grades
            var studentIdClaim = User.FindFirst(ClaimTypeConstants.StudentId)?.Value;
            var isOwnGrade = !string.IsNullOrEmpty(studentIdClaim) && int.Parse(studentIdClaim) == grade.StudentId;

            if (!User.IsInRole(RoleConstants.Admin) &&
                !User.IsInRole(RoleConstants.Teacher) &&
                !isOwnGrade)
            {
                return Forbid();
            }

            _logger.LogInformation("Retrieved grade {GradeId}", id);

            return Ok(ApiResponse<GradeDto>.SuccessResponse(grade, "Grade retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving grade {GradeId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while retrieving grade", 500));
        }
    }

    /// <summary>
    /// Gets all grades for a specific student
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <returns>List of student's grades</returns>
    [HttpGet("student/{studentId}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<List<GradeDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetGradesByStudentId(int studentId)
    {
        try
        {
            // Allow students to view their own grades
            var studentIdClaim = User.FindFirst(ClaimTypeConstants.StudentId)?.Value;
            var isOwnGrades = !string.IsNullOrEmpty(studentIdClaim) && int.Parse(studentIdClaim) == studentId;

            if (!User.IsInRole(RoleConstants.Admin) &&
                !User.IsInRole(RoleConstants.Teacher) &&
                !isOwnGrades)
            {
                return Forbid();
            }

            var grades = await _gradeService.GetGradesByStudentIdAsync(studentId);

            _logger.LogInformation("Retrieved {Count} grades for student {StudentId}", grades.Count, studentId);

            return Ok(ApiResponse<List<GradeDto>>.SuccessResponse(grades, $"Retrieved {grades.Count} grades"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving grades for student {StudentId}", studentId);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while retrieving grades", 500));
        }
    }

    /// <summary>
    /// Gets all grades for a specific course
    /// </summary>
    /// <param name="courseId">Course ID</param>
    /// <returns>List of course grades</returns>
    [HttpGet("course/{courseId}")]
    [Authorize(Policy = PolicyConstants.AdminOrTeacher)]
    [ProducesResponseType(typeof(ApiResponse<List<GradeDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetGradesByCourseId(int courseId)
    {
        try
        {
            var grades = await _gradeService.GetGradesByCourseIdAsync(courseId);

            _logger.LogInformation("Retrieved {Count} grades for course {CourseId}", grades.Count, courseId);

            return Ok(ApiResponse<List<GradeDto>>.SuccessResponse(grades, $"Retrieved {grades.Count} grades"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving grades for course {CourseId}", courseId);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while retrieving grades", 500));
        }
    }

    /// <summary>
    /// Gets all grades for a specific assignment
    /// </summary>
    /// <param name="assignmentId">Assignment ID</param>
    /// <returns>List of assignment grades</returns>
    [HttpGet("assignment/{assignmentId}")]
    [Authorize(Policy = PolicyConstants.AdminOrTeacher)]
    [ProducesResponseType(typeof(ApiResponse<List<GradeDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetGradesByAssignmentId(int assignmentId)
    {
        try
        {
            var grades = await _gradeService.GetGradesByAssignmentIdAsync(assignmentId);

            _logger.LogInformation("Retrieved {Count} grades for assignment {AssignmentId}", grades.Count, assignmentId);

            return Ok(ApiResponse<List<GradeDto>>.SuccessResponse(grades, $"Retrieved {grades.Count} grades"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving grades for assignment {AssignmentId}", assignmentId);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while retrieving grades", 500));
        }
    }

    /// <summary>
    /// Creates a new grade
    /// </summary>
    /// <param name="request">Grade creation request</param>
    /// <returns>Created grade information</returns>
    [HttpPost]
    [Authorize(Policy = PolicyConstants.AdminOrTeacher)]
    [ProducesResponseType(typeof(ApiResponse<GradeDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateGrade([FromBody] CreateGradeRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Invalid request data", 400));
            }

            var grade = await _gradeService.CreateGradeAsync(request);

            _logger.LogInformation("Created new grade for student {StudentId} in course {CourseId}",
                request.StudentId, request.CourseId);

            return CreatedAtAction(
                nameof(GetGradeById),
                new { id = grade.Id },
                ApiResponse<GradeDto>.SuccessResponse(grade, "Grade created successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to create grade: {Message}", ex.Message);
            return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message, 400));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating grade");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while creating grade", 500));
        }
    }

    /// <summary>
    /// Creates multiple grades in bulk (e.g., for an assignment)
    /// </summary>
    /// <param name="request">Bulk grade creation request</param>
    /// <returns>List of created grades</returns>
    [HttpPost("bulk")]
    [Authorize(Policy = PolicyConstants.AdminOrTeacher)]
    [ProducesResponseType(typeof(ApiResponse<List<GradeDto>>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateBulkGrades([FromBody] BulkGradeRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Invalid request data", 400));
            }

            var grades = await _gradeService.CreateBulkGradesAsync(request);

            _logger.LogInformation("Created {Count} bulk grades for course {CourseId}", grades.Count, request.CourseId);

            return CreatedAtAction(
                nameof(GetGradesByCourseId),
                new { courseId = request.CourseId },
                ApiResponse<List<GradeDto>>.SuccessResponse(grades, $"{grades.Count} grades created successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to create bulk grades: {Message}", ex.Message);
            return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message, 400));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating bulk grades");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while creating grades", 500));
        }
    }

    /// <summary>
    /// Updates an existing grade
    /// </summary>
    /// <param name="id">Grade ID</param>
    /// <param name="request">Grade update request</param>
    /// <returns>Updated grade information</returns>
    [HttpPut("{id}")]
    [Authorize(Policy = PolicyConstants.AdminOrTeacher)]
    [ProducesResponseType(typeof(ApiResponse<GradeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateGrade(int id, [FromBody] UpdateGradeRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Invalid request data", 400));
            }

            var grade = await _gradeService.UpdateGradeAsync(id, request);

            if (grade == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Grade not found", 404));
            }

            _logger.LogInformation("Updated grade {GradeId}", id);

            return Ok(ApiResponse<GradeDto>.SuccessResponse(grade, "Grade updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating grade {GradeId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while updating grade", 500));
        }
    }

    /// <summary>
    /// Deletes a grade (soft delete)
    /// </summary>
    /// <param name="id">Grade ID</param>
    /// <returns>Success status</returns>
    [HttpDelete("{id}")]
    [Authorize(Policy = PolicyConstants.AdminOrTeacher)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteGrade(int id)
    {
        try
        {
            var result = await _gradeService.DeleteGradeAsync(id);

            if (!result)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Grade not found", 404));
            }

            _logger.LogInformation("Deleted grade {GradeId}", id);

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Grade deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting grade {GradeId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while deleting grade", 500));
        }
    }

    /// <summary>
    /// Publishes a grade to make it visible to the student
    /// </summary>
    /// <param name="id">Grade ID</param>
    /// <returns>Success status</returns>
    [HttpPost("{id}/publish")]
    [Authorize(Policy = PolicyConstants.AdminOrTeacher)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> PublishGrade(int id)
    {
        try
        {
            var result = await _gradeService.PublishGradeAsync(id);

            if (!result)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Grade not found", 404));
            }

            _logger.LogInformation("Published grade {GradeId}", id);

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Grade published successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing grade {GradeId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while publishing grade", 500));
        }
    }
}
