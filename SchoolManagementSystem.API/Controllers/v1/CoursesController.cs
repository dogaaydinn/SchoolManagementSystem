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
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;
    private readonly ILogger<CoursesController> _logger;

    public CoursesController(ICourseService courseService, ILogger<CoursesController> logger)
    {
        _courseService = courseService;
        _logger = logger;
    }

    /// <summary>
    /// Gets a paginated list of all courses with optional search and filtering
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <param name="searchTerm">Search term for filtering by course code, name, or description</param>
    /// <param name="departmentId">Filter by department ID</param>
    /// <param name="semesterId">Filter by semester ID</param>
    /// <param name="isActive">Filter by active status</param>
    /// <returns>Paginated list of courses</returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<CourseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllCourses(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int? departmentId = null,
        [FromQuery] int? semesterId = null,
        [FromQuery] bool? isActive = null)
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

            var result = await _courseService.GetAllCoursesAsync(pageNumber, pageSize, searchTerm, departmentId, semesterId, isActive);

            _logger.LogInformation("Retrieved {Count} courses (page {PageNumber}/{TotalPages})",
                result.Items.Count, result.PageNumber, result.TotalPages);

            return Ok(ApiResponse<PagedResult<CourseDto>>.SuccessResponse(
                result,
                $"Retrieved {result.Items.Count} courses"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving courses list");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while retrieving courses", 500));
        }
    }

    /// <summary>
    /// Gets detailed information about a specific course by ID
    /// </summary>
    /// <param name="id">Course ID</param>
    /// <returns>Course details including enrolled students, assignments, and schedules</returns>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<CourseDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCourseById(int id)
    {
        try
        {
            var course = await _courseService.GetCourseByIdAsync(id);

            if (course == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Course not found", 404));
            }

            _logger.LogInformation("Retrieved course {CourseId}", id);

            return Ok(ApiResponse<CourseDetailDto>.SuccessResponse(course, "Course retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving course {CourseId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while retrieving course", 500));
        }
    }

    /// <summary>
    /// Gets detailed information about a course by course code
    /// </summary>
    /// <param name="courseCode">Course code (e.g., CS101)</param>
    /// <returns>Course details</returns>
    [HttpGet("code/{courseCode}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<CourseDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCourseByCourseCode(string courseCode)
    {
        try
        {
            var course = await _courseService.GetCourseByCourseCodeAsync(courseCode);

            if (course == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Course not found", 404));
            }

            _logger.LogInformation("Retrieved course by code {CourseCode}", courseCode);

            return Ok(ApiResponse<CourseDetailDto>.SuccessResponse(course, "Course retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving course by code {CourseCode}", courseCode);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while retrieving course", 500));
        }
    }

    /// <summary>
    /// Creates a new course
    /// </summary>
    /// <param name="request">Course creation request</param>
    /// <returns>Created course information</returns>
    [HttpPost]
    [Authorize(Policy = PolicyConstants.AdminOnly)]
    [ProducesResponseType(typeof(ApiResponse<CourseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateCourse([FromBody] CreateCourseRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Invalid request data", 400));
            }

            var course = await _courseService.CreateCourseAsync(request);

            _logger.LogInformation("Created new course {CourseCode}", course.CourseCode);

            return CreatedAtAction(
                nameof(GetCourseById),
                new { id = course.Id },
                ApiResponse<CourseDto>.SuccessResponse(course, "Course created successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to create course: {Message}", ex.Message);
            return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message, 400));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating course");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while creating course", 500));
        }
    }

    /// <summary>
    /// Updates an existing course
    /// </summary>
    /// <param name="id">Course ID</param>
    /// <param name="request">Course update request</param>
    /// <returns>Updated course information</returns>
    [HttpPut("{id}")]
    [Authorize(Policy = PolicyConstants.AdminOrTeacher)]
    [ProducesResponseType(typeof(ApiResponse<CourseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateCourse(int id, [FromBody] UpdateCourseRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Invalid request data", 400));
            }

            var course = await _courseService.UpdateCourseAsync(id, request);

            if (course == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Course not found", 404));
            }

            _logger.LogInformation("Updated course {CourseId}", id);

            return Ok(ApiResponse<CourseDto>.SuccessResponse(course, "Course updated successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to update course: {Message}", ex.Message);
            return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message, 400));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating course {CourseId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while updating course", 500));
        }
    }

    /// <summary>
    /// Deletes a course (soft delete)
    /// </summary>
    /// <param name="id">Course ID</param>
    /// <returns>Success status</returns>
    [HttpDelete("{id}")]
    [Authorize(Policy = PolicyConstants.AdminOnly)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteCourse(int id)
    {
        try
        {
            var result = await _courseService.DeleteCourseAsync(id);

            if (!result)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Course not found", 404));
            }

            _logger.LogInformation("Deleted course {CourseId}", id);

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Course deleted successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to delete course: {Message}", ex.Message);
            return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message, 400));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting course {CourseId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while deleting course", 500));
        }
    }

    /// <summary>
    /// Gets all students enrolled in a course
    /// </summary>
    /// <param name="id">Course ID</param>
    /// <returns>List of enrolled students</returns>
    [HttpGet("{id}/students")]
    [Authorize(Policy = PolicyConstants.AdminOrTeacher)]
    [ProducesResponseType(typeof(ApiResponse<List<StudentListDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetCourseEnrolledStudents(int id)
    {
        try
        {
            var students = await _courseService.GetCourseEnrolledStudentsAsync(id);

            _logger.LogInformation("Retrieved {Count} enrolled students for course {CourseId}", students.Count, id);

            return Ok(ApiResponse<List<StudentListDto>>.SuccessResponse(students, $"Retrieved {students.Count} enrolled students"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving enrolled students for course {CourseId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while retrieving students", 500));
        }
    }

    /// <summary>
    /// Gets available courses for a student (not enrolled, prerequisites met, not full)
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <returns>List of available courses</returns>
    [HttpGet("available/{studentId}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<List<CourseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAvailableCoursesForStudent(int studentId)
    {
        try
        {
            // Allow students to view their own available courses
            var studentIdClaim = User.FindFirst(ClaimTypeConstants.StudentId)?.Value;
            var isOwnCourses = !string.IsNullOrEmpty(studentIdClaim) && int.Parse(studentIdClaim) == studentId;

            if (!User.IsInRole(RoleConstants.Admin) &&
                !User.IsInRole(RoleConstants.Teacher) &&
                !isOwnCourses)
            {
                return Forbid();
            }

            var courses = await _courseService.GetAvailableCoursesForStudentAsync(studentId);

            _logger.LogInformation("Retrieved {Count} available courses for student {StudentId}", courses.Count, studentId);

            return Ok(ApiResponse<List<CourseDto>>.SuccessResponse(courses, $"Retrieved {courses.Count} available courses"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving available courses for student {StudentId}", studentId);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while retrieving courses", 500));
        }
    }

    /// <summary>
    /// Checks if a student can enroll in a course
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <param name="courseId">Course ID</param>
    /// <returns>Enrollment eligibility status with reason if not eligible</returns>
    [HttpGet("can-enroll")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CanEnrollInCourse([FromQuery] int studentId, [FromQuery] int courseId)
    {
        try
        {
            // Allow students to check their own enrollment eligibility
            var studentIdClaim = User.FindFirst(ClaimTypeConstants.StudentId)?.Value;
            var isOwnCheck = !string.IsNullOrEmpty(studentIdClaim) && int.Parse(studentIdClaim) == studentId;

            if (!User.IsInRole(RoleConstants.Admin) &&
                !User.IsInRole(RoleConstants.Teacher) &&
                !isOwnCheck)
            {
                return Forbid();
            }

            var (canEnroll, reason) = await _courseService.CanEnrollInCourseAsync(studentId, courseId);

            var response = new
            {
                CanEnroll = canEnroll,
                Reason = reason
            };

            _logger.LogInformation("Checked enrollment eligibility for student {StudentId} in course {CourseId}: {CanEnroll}",
                studentId, courseId, canEnroll);

            return Ok(ApiResponse<object>.SuccessResponse(response,
                canEnroll ? "Student can enroll in this course" : $"Cannot enroll: {reason}"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking enrollment eligibility for student {StudentId} in course {CourseId}",
                studentId, courseId);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while checking eligibility", 500));
        }
    }
}
