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
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;
    private readonly ILogger<StudentsController> _logger;

    public StudentsController(IStudentService studentService, ILogger<StudentsController> logger)
    {
        _studentService = studentService;
        _logger = logger;
    }

    /// <summary>
    /// Gets a paginated list of all students with optional search and filtering
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <param name="searchTerm">Search term for filtering by name, email, or student number</param>
    /// <param name="status">Filter by student status (Active, Inactive, Graduated, Suspended, Withdrawn)</param>
    /// <returns>Paginated list of students</returns>
    [HttpGet]
    [Authorize(Policy = PolicyConstants.AdminOrTeacher)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<StudentListDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllStudents(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? status = null)
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

            var result = await _studentService.GetAllStudentsAsync(pageNumber, pageSize, searchTerm, status);

            _logger.LogInformation("Retrieved {Count} students (page {PageNumber}/{TotalPages})",
                result.Items.Count, result.PageNumber, result.TotalPages);

            return Ok(ApiResponse<PagedResult<StudentListDto>>.SuccessResponse(
                result,
                $"Retrieved {result.Items.Count} students"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving students list");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while retrieving students", 500));
        }
    }

    /// <summary>
    /// Gets detailed information about a specific student by ID
    /// </summary>
    /// <param name="id">Student ID</param>
    /// <returns>Student details including enrolled courses and grades</returns>
    [HttpGet("{id}")]
    [Authorize(Policy = PolicyConstants.AdminOrTeacher)]
    [ProducesResponseType(typeof(ApiResponse<StudentDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetStudentById(int id)
    {
        try
        {
            var student = await _studentService.GetStudentByIdAsync(id);

            if (student == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Student not found", 404));
            }

            _logger.LogInformation("Retrieved student {StudentId}", id);

            return Ok(ApiResponse<StudentDetailDto>.SuccessResponse(student, "Student retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving student {StudentId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while retrieving student", 500));
        }
    }

    /// <summary>
    /// Gets detailed information about a student by student number
    /// </summary>
    /// <param name="studentNumber">Student number (e.g., STU20241234)</param>
    /// <returns>Student details</returns>
    [HttpGet("number/{studentNumber}")]
    [Authorize(Policy = PolicyConstants.AdminOrTeacher)]
    [ProducesResponseType(typeof(ApiResponse<StudentDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudentByNumber(string studentNumber)
    {
        try
        {
            var student = await _studentService.GetStudentByNumberAsync(studentNumber);

            if (student == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Student not found", 404));
            }

            _logger.LogInformation("Retrieved student by number {StudentNumber}", studentNumber);

            return Ok(ApiResponse<StudentDetailDto>.SuccessResponse(student, "Student retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving student by number {StudentNumber}", studentNumber);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while retrieving student", 500));
        }
    }

    /// <summary>
    /// Creates a new student
    /// </summary>
    /// <param name="request">Student creation request</param>
    /// <returns>Created student information</returns>
    [HttpPost]
    [Authorize(Policy = PolicyConstants.AdminOnly)]
    [ProducesResponseType(typeof(ApiResponse<StudentDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateStudent([FromBody] CreateStudentRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Invalid request data", 400));
            }

            var student = await _studentService.CreateStudentAsync(request);

            _logger.LogInformation("Created new student {StudentNumber} with email {Email}",
                student.StudentNumber, student.Email);

            return CreatedAtAction(
                nameof(GetStudentById),
                new { id = student.Id },
                ApiResponse<StudentDto>.SuccessResponse(student, "Student created successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to create student: {Message}", ex.Message);
            return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message, 400));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating student");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while creating student", 500));
        }
    }

    /// <summary>
    /// Updates an existing student
    /// </summary>
    /// <param name="id">Student ID</param>
    /// <param name="request">Student update request</param>
    /// <returns>Updated student information</returns>
    [HttpPut("{id}")]
    [Authorize(Policy = PolicyConstants.AdminOnly)]
    [ProducesResponseType(typeof(ApiResponse<StudentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateStudent(int id, [FromBody] UpdateStudentRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Invalid request data", 400));
            }

            var student = await _studentService.UpdateStudentAsync(id, request);

            if (student == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Student not found", 404));
            }

            _logger.LogInformation("Updated student {StudentId}", id);

            return Ok(ApiResponse<StudentDto>.SuccessResponse(student, "Student updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating student {StudentId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while updating student", 500));
        }
    }

    /// <summary>
    /// Deletes a student (soft delete)
    /// </summary>
    /// <param name="id">Student ID</param>
    /// <returns>Success status</returns>
    [HttpDelete("{id}")]
    [Authorize(Policy = PolicyConstants.AdminOnly)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteStudent(int id)
    {
        try
        {
            var result = await _studentService.DeleteStudentAsync(id);

            if (!result)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Student not found", 404));
            }

            _logger.LogInformation("Deleted student {StudentId}", id);

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Student deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting student {StudentId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while deleting student", 500));
        }
    }

    /// <summary>
    /// Gets the transcript for a student including all grades grouped by semester
    /// </summary>
    /// <param name="id">Student ID</param>
    /// <returns>Student transcript with semester grades and GPA</returns>
    [HttpGet("{id}/transcript")]
    [Authorize(Policy = PolicyConstants.AdminOrTeacher)]
    [ProducesResponseType(typeof(ApiResponse<StudentTranscriptDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetStudentTranscript(int id)
    {
        try
        {
            // Allow students to view their own transcript
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var studentIdClaim = User.FindFirst(ClaimTypeConstants.StudentId)?.Value;
            var isOwnTranscript = !string.IsNullOrEmpty(studentIdClaim) && int.Parse(studentIdClaim) == id;

            if (!User.IsInRole(RoleConstants.Admin) &&
                !User.IsInRole(RoleConstants.Teacher) &&
                !isOwnTranscript)
            {
                return Forbid();
            }

            var transcript = await _studentService.GetStudentTranscriptAsync(id);

            if (transcript == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Student not found", 404));
            }

            _logger.LogInformation("Retrieved transcript for student {StudentId}", id);

            return Ok(ApiResponse<StudentTranscriptDto>.SuccessResponse(transcript, "Transcript retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transcript for student {StudentId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while retrieving transcript", 500));
        }
    }

    /// <summary>
    /// Enrolls a student in a course
    /// </summary>
    /// <param name="id">Student ID</param>
    /// <param name="courseId">Course ID to enroll in</param>
    /// <returns>Success status</returns>
    [HttpPost("{id}/enroll")]
    [Authorize(Policy = PolicyConstants.AdminOnly)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> EnrollInCourse(int id, [FromBody] int courseId)
    {
        try
        {
            var result = await _studentService.EnrollInCourseAsync(id, courseId);

            if (!result)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(
                    "Enrollment failed. Student or course may not exist, or student is already enrolled", 400));
            }

            _logger.LogInformation("Enrolled student {StudentId} in course {CourseId}", id, courseId);

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Student enrolled successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enrolling student {StudentId} in course {CourseId}", id, courseId);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while enrolling student", 500));
        }
    }

    /// <summary>
    /// Unenrolls a student from a course
    /// </summary>
    /// <param name="id">Student ID</param>
    /// <param name="courseId">Course ID to unenroll from</param>
    /// <returns>Success status</returns>
    [HttpDelete("{id}/courses/{courseId}")]
    [Authorize(Policy = PolicyConstants.AdminOnly)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UnenrollFromCourse(int id, int courseId)
    {
        try
        {
            var result = await _studentService.UnenrollFromCourseAsync(id, courseId);

            if (!result)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Enrollment not found", 404));
            }

            _logger.LogInformation("Unenrolled student {StudentId} from course {CourseId}", id, courseId);

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Student unenrolled successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unenrolling student {StudentId} from course {CourseId}", id, courseId);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while unenrolling student", 500));
        }
    }

    /// <summary>
    /// Gets all courses a student is enrolled in
    /// </summary>
    /// <param name="id">Student ID</param>
    /// <returns>List of enrolled courses</returns>
    [HttpGet("{id}/courses")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<List<CourseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetEnrolledCourses(int id)
    {
        try
        {
            // Allow students to view their own enrolled courses
            var studentIdClaim = User.FindFirst(ClaimTypeConstants.StudentId)?.Value;
            var isOwnCourses = !string.IsNullOrEmpty(studentIdClaim) && int.Parse(studentIdClaim) == id;

            if (!User.IsInRole(RoleConstants.Admin) &&
                !User.IsInRole(RoleConstants.Teacher) &&
                !isOwnCourses)
            {
                return Forbid();
            }

            var courses = await _studentService.GetEnrolledCoursesAsync(id);

            _logger.LogInformation("Retrieved {Count} enrolled courses for student {StudentId}", courses.Count, id);

            return Ok(ApiResponse<List<CourseDto>>.SuccessResponse(courses, $"Retrieved {courses.Count} enrolled courses"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving enrolled courses for student {StudentId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while retrieving courses", 500));
        }
    }

    /// <summary>
    /// Calculates and updates the GPA for a student
    /// </summary>
    /// <param name="id">Student ID</param>
    /// <returns>Calculated GPA value</returns>
    [HttpPost("{id}/calculate-gpa")]
    [Authorize(Policy = PolicyConstants.AdminOrTeacher)]
    [ProducesResponseType(typeof(ApiResponse<decimal>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CalculateGPA(int id)
    {
        try
        {
            var gpa = await _studentService.CalculateGPAAsync(id);

            _logger.LogInformation("Calculated GPA {GPA:F2} for student {StudentId}", gpa, id);

            return Ok(ApiResponse<decimal>.SuccessResponse(gpa, $"GPA calculated: {gpa:F2}"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating GPA for student {StudentId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while calculating GPA", 500));
        }
    }
}
