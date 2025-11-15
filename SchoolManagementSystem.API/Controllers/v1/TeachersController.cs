using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Core.Constants;
using SchoolManagementSystem.Core.DTOs;
using SchoolManagementSystem.Core.Interfaces;

namespace SchoolManagementSystem.API.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize]
public class TeachersController : ControllerBase
{
    private readonly ITeacherService _teacherService;
    private readonly ILogger<TeachersController> _logger;

    public TeachersController(ITeacherService teacherService, ILogger<TeachersController> logger)
    {
        _teacherService = teacherService;
        _logger = logger;
    }

    /// <summary>
    /// Gets a paginated list of all teachers with optional search and filtering
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <param name="searchTerm">Search term for filtering by name, email, employee number, or specialization</param>
    /// <param name="departmentId">Filter by department ID</param>
    /// <param name="isActive">Filter by active status</param>
    /// <returns>Paginated list of teachers</returns>
    [HttpGet]
    [Authorize(Policy = PolicyConstants.AdminOrTeacher)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<TeacherDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllTeachers(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int? departmentId = null,
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

            var result = await _teacherService.GetAllTeachersAsync(pageNumber, pageSize, searchTerm, departmentId, isActive);

            _logger.LogInformation("Retrieved {Count} teachers (page {PageNumber}/{TotalPages})",
                result.Items.Count, result.PageNumber, result.TotalPages);

            return Ok(ApiResponse<PagedResult<TeacherDto>>.SuccessResponse(
                result,
                $"Retrieved {result.Items.Count} teachers"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving teachers list");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while retrieving teachers", 500));
        }
    }

    /// <summary>
    /// Gets detailed information about a specific teacher by ID
    /// </summary>
    /// <param name="id">Teacher ID</param>
    /// <returns>Teacher details including courses and advisees</returns>
    [HttpGet("{id}")]
    [Authorize(Policy = PolicyConstants.AdminOrTeacher)]
    [ProducesResponseType(typeof(ApiResponse<TeacherDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetTeacherById(int id)
    {
        try
        {
            var teacher = await _teacherService.GetTeacherByIdAsync(id);

            if (teacher == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Teacher not found", 404));
            }

            _logger.LogInformation("Retrieved teacher {TeacherId}", id);

            return Ok(ApiResponse<TeacherDetailDto>.SuccessResponse(teacher, "Teacher retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving teacher {TeacherId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while retrieving teacher", 500));
        }
    }

    /// <summary>
    /// Gets detailed information about a teacher by employee number
    /// </summary>
    /// <param name="employeeNumber">Employee number (e.g., EMP20241234)</param>
    /// <returns>Teacher details</returns>
    [HttpGet("number/{employeeNumber}")]
    [Authorize(Policy = PolicyConstants.AdminOrTeacher)]
    [ProducesResponseType(typeof(ApiResponse<TeacherDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTeacherByEmployeeNumber(string employeeNumber)
    {
        try
        {
            var teacher = await _teacherService.GetTeacherByEmployeeNumberAsync(employeeNumber);

            if (teacher == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Teacher not found", 404));
            }

            _logger.LogInformation("Retrieved teacher by employee number {EmployeeNumber}", employeeNumber);

            return Ok(ApiResponse<TeacherDetailDto>.SuccessResponse(teacher, "Teacher retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving teacher by employee number {EmployeeNumber}", employeeNumber);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while retrieving teacher", 500));
        }
    }

    /// <summary>
    /// Creates a new teacher
    /// </summary>
    /// <param name="request">Teacher creation request</param>
    /// <returns>Created teacher information</returns>
    [HttpPost]
    [Authorize(Policy = PolicyConstants.AdminOnly)]
    [ProducesResponseType(typeof(ApiResponse<TeacherDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateTeacher([FromBody] CreateTeacherRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Invalid request data", 400));
            }

            var teacher = await _teacherService.CreateTeacherAsync(request);

            _logger.LogInformation("Created new teacher {EmployeeNumber} with email {Email}",
                teacher.EmployeeNumber, teacher.Email);

            return CreatedAtAction(
                nameof(GetTeacherById),
                new { id = teacher.Id },
                ApiResponse<TeacherDto>.SuccessResponse(teacher, "Teacher created successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to create teacher: {Message}", ex.Message);
            return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message, 400));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating teacher");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while creating teacher", 500));
        }
    }

    /// <summary>
    /// Updates an existing teacher
    /// </summary>
    /// <param name="id">Teacher ID</param>
    /// <param name="request">Teacher update request</param>
    /// <returns>Updated teacher information</returns>
    [HttpPut("{id}")]
    [Authorize(Policy = PolicyConstants.AdminOnly)]
    [ProducesResponseType(typeof(ApiResponse<TeacherDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateTeacher(int id, [FromBody] UpdateTeacherRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Invalid request data", 400));
            }

            var teacher = await _teacherService.UpdateTeacherAsync(id, request);

            if (teacher == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Teacher not found", 404));
            }

            _logger.LogInformation("Updated teacher {TeacherId}", id);

            return Ok(ApiResponse<TeacherDto>.SuccessResponse(teacher, "Teacher updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating teacher {TeacherId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while updating teacher", 500));
        }
    }

    /// <summary>
    /// Deletes a teacher (soft delete)
    /// </summary>
    /// <param name="id">Teacher ID</param>
    /// <returns>Success status</returns>
    [HttpDelete("{id}")]
    [Authorize(Policy = PolicyConstants.AdminOnly)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteTeacher(int id)
    {
        try
        {
            var result = await _teacherService.DeleteTeacherAsync(id);

            if (!result)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Teacher not found", 404));
            }

            _logger.LogInformation("Deleted teacher {TeacherId}", id);

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Teacher deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting teacher {TeacherId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while deleting teacher", 500));
        }
    }

    /// <summary>
    /// Gets all courses taught by a teacher
    /// </summary>
    /// <param name="id">Teacher ID</param>
    /// <returns>List of courses</returns>
    [HttpGet("{id}/courses")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<List<CourseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetTeacherCourses(int id)
    {
        try
        {
            var courses = await _teacherService.GetTeacherCoursesAsync(id);

            _logger.LogInformation("Retrieved {Count} courses for teacher {TeacherId}", courses.Count, id);

            return Ok(ApiResponse<List<CourseDto>>.SuccessResponse(courses, $"Retrieved {courses.Count} courses"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving courses for teacher {TeacherId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while retrieving courses", 500));
        }
    }

    /// <summary>
    /// Gets all students advised by a teacher
    /// </summary>
    /// <param name="id">Teacher ID</param>
    /// <returns>List of advisees</returns>
    [HttpGet("{id}/advisees")]
    [Authorize(Policy = PolicyConstants.AdminOrTeacher)]
    [ProducesResponseType(typeof(ApiResponse<List<StudentListDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetTeacherAdvisees(int id)
    {
        try
        {
            var advisees = await _teacherService.GetTeacherAdviseesAsync(id);

            _logger.LogInformation("Retrieved {Count} advisees for teacher {TeacherId}", advisees.Count, id);

            return Ok(ApiResponse<List<StudentListDto>>.SuccessResponse(advisees, $"Retrieved {advisees.Count} advisees"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving advisees for teacher {TeacherId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while retrieving advisees", 500));
        }
    }

    /// <summary>
    /// Assigns a course to a teacher
    /// </summary>
    /// <param name="id">Teacher ID</param>
    /// <param name="courseId">Course ID to assign</param>
    /// <returns>Success status</returns>
    [HttpPost("{id}/assign-course")]
    [Authorize(Policy = PolicyConstants.AdminOnly)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AssignCourse(int id, [FromBody] int courseId)
    {
        try
        {
            var result = await _teacherService.AssignCourseAsync(id, courseId);

            if (!result)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(
                    "Assignment failed. Teacher or course may not exist", 400));
            }

            _logger.LogInformation("Assigned course {CourseId} to teacher {TeacherId}", courseId, id);

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Course assigned successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning course {CourseId} to teacher {TeacherId}", courseId, id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while assigning course", 500));
        }
    }

    /// <summary>
    /// Unassigns a course from a teacher
    /// </summary>
    /// <param name="id">Teacher ID</param>
    /// <param name="courseId">Course ID to unassign</param>
    /// <returns>Success status</returns>
    [HttpDelete("{id}/courses/{courseId}")]
    [Authorize(Policy = PolicyConstants.AdminOnly)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UnassignCourse(int id, int courseId)
    {
        try
        {
            var result = await _teacherService.UnassignCourseAsync(id, courseId);

            if (!result)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Course assignment not found", 404));
            }

            _logger.LogInformation("Unassigned course {CourseId} from teacher {TeacherId}", courseId, id);

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Course unassigned successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unassigning course {CourseId} from teacher {TeacherId}", courseId, id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while unassigning course", 500));
        }
    }
}
