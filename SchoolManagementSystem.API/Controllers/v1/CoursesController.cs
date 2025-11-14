using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Core.DTOs;
using SchoolManagementSystem.Core.Interfaces;

namespace SchoolManagementSystem.API.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
public class CoursesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CoursesController> _logger;

    public CoursesController(IUnitOfWork unitOfWork, ILogger<CoursesController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Get all courses with pagination and filtering
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<CourseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCourses([FromQuery] PagedRequest request)
    {
        try
        {
            var (courses, totalCount) = await _unitOfWork.Courses.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                filter: c => (string.IsNullOrEmpty(request.SearchTerm) ||
                             c.CourseName.Contains(request.SearchTerm) ||
                             c.CourseCode.Contains(request.SearchTerm)) &&
                             c.IsActive,
                orderBy: query => request.SortOrder?.ToLower() == "desc"
                    ? query.OrderByDescending(c => c.CourseName)
                    : query.OrderBy(c => c.CourseName),
                includes: c => c.Teacher!, c => c.Department!
            );

            var courseDtos = courses.Select(c => new CourseDto
            {
                Id = c.Id,
                CourseCode = c.CourseCode,
                CourseName = c.CourseName,
                Description = c.Description,
                Credits = c.Credits,
                DepartmentName = c.Department?.Name,
                TeacherName = c.Teacher?.User.FullName,
                MaxStudents = c.MaxStudents,
                CurrentEnrollment = c.CurrentEnrollment,
                Level = c.Level,
                IsActive = c.IsActive,
                SemesterName = c.Semester?.Name
            }).ToList();

            var pagedResult = new PagedResult<CourseDto>
            {
                Items = courseDtos,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };

            return Ok(ApiResponse<PagedResult<CourseDto>>.SuccessResponse(pagedResult));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving courses");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error retrieving courses", 500));
        }
    }

    /// <summary>
    /// Get course by ID with full details
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<CourseDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCourse(int id)
    {
        try
        {
            var course = await _unitOfWork.Courses.GetByIdWithIncludesAsync(
                id,
                c => c.Teacher!,
                c => c.Department!,
                c => c.Enrollments,
                c => c.Assignments,
                c => c.Schedules
            );

            if (course == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Course not found", 404));
            }

            var courseDetailDto = new CourseDetailDto
            {
                Id = course.Id,
                CourseCode = course.CourseCode,
                CourseName = course.CourseName,
                Description = course.Description,
                Credits = course.Credits,
                DepartmentName = course.Department?.Name,
                TeacherName = course.Teacher?.User.FullName,
                MaxStudents = course.MaxStudents,
                CurrentEnrollment = course.CurrentEnrollment,
                Level = course.Level,
                IsActive = course.IsActive,
                Syllabus = course.Syllabus,
                LearningOutcomes = course.LearningOutcomes,
                CourseFee = course.CourseFee,
                Teacher = course.Teacher != null ? new TeacherDto
                {
                    Id = course.Teacher.Id,
                    FullName = course.Teacher.User.FullName,
                    Email = course.Teacher.User.Email!,
                    Specialization = course.Teacher.Specialization
                } : null,
                Assignments = course.Assignments.Select(a => new AssignmentDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    DueDate = a.DueDate,
                    MaxScore = a.MaxScore
                }).ToList(),
                Schedules = course.Schedules.Select(s => new ScheduleDto
                {
                    Id = s.Id,
                    DayOfWeek = s.DayOfWeek,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    Room = s.Room,
                    Building = s.Building
                }).ToList()
            };

            return Ok(ApiResponse<CourseDetailDto>.SuccessResponse(courseDetailDto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving course {CourseId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error retrieving course", 500));
        }
    }

    /// <summary>
    /// Create a new course
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin")]
    [ProducesResponseType(typeof(ApiResponse<CourseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCourse([FromBody] CreateCourseRequestDto request)
    {
        try
        {
            // Check if course code already exists
            var existingCourse = await _unitOfWork.Courses.FirstOrDefaultAsync(c => c.CourseCode == request.CourseCode);
            if (existingCourse != null)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Course code already exists", 400));
            }

            var course = new Core.Entities.Course
            {
                CourseCode = request.CourseCode,
                CourseName = request.CourseName,
                Description = request.Description,
                Credits = request.Credits,
                DepartmentId = request.DepartmentId,
                TeacherId = request.TeacherId,
                MaxStudents = request.MaxStudents,
                Level = request.Level,
                Prerequisites = request.PrerequisiteCourseIds != null
                    ? string.Join(",", request.PrerequisiteCourseIds)
                    : null,
                Syllabus = request.Syllabus,
                LearningOutcomes = request.LearningOutcomes,
                SemesterId = request.SemesterId,
                CourseFee = request.CourseFee,
                IsActive = true,
                CurrentEnrollment = 0
            };

            await _unitOfWork.Courses.AddAsync(course);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Course created: {CourseCode}", request.CourseCode);

            var courseDto = new CourseDto
            {
                Id = course.Id,
                CourseCode = course.CourseCode,
                CourseName = course.CourseName,
                Credits = course.Credits,
                IsActive = true
            };

            return CreatedAtAction(nameof(GetCourse), new { id = course.Id },
                ApiResponse<CourseDto>.SuccessResponse(courseDto, "Course created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating course");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error creating course", 500));
        }
    }

    /// <summary>
    /// Update course information
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    [ProducesResponseType(typeof(ApiResponse<CourseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCourse(int id, [FromBody] UpdateCourseRequestDto request)
    {
        try
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(id);
            if (course == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Course not found", 404));
            }

            if (!string.IsNullOrEmpty(request.CourseName))
                course.CourseName = request.CourseName;
            if (!string.IsNullOrEmpty(request.Description))
                course.Description = request.Description;
            if (request.Credits.HasValue)
                course.Credits = request.Credits.Value;
            if (request.TeacherId.HasValue)
                course.TeacherId = request.TeacherId;
            if (request.MaxStudents.HasValue)
                course.MaxStudents = request.MaxStudents.Value;
            if (request.PrerequisiteCourseIds != null)
                course.Prerequisites = string.Join(",", request.PrerequisiteCourseIds);
            if (!string.IsNullOrEmpty(request.Syllabus))
                course.Syllabus = request.Syllabus;
            if (!string.IsNullOrEmpty(request.LearningOutcomes))
                course.LearningOutcomes = request.LearningOutcomes;
            if (request.IsActive.HasValue)
                course.IsActive = request.IsActive.Value;

            _unitOfWork.Courses.Update(course);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Course updated: {CourseId}", id);

            var courseDto = new CourseDto
            {
                Id = course.Id,
                CourseCode = course.CourseCode,
                CourseName = course.CourseName,
                Credits = course.Credits,
                IsActive = course.IsActive
            };

            return Ok(ApiResponse<CourseDto>.SuccessResponse(courseDto, "Course updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating course {CourseId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error updating course", 500));
        }
    }

    /// <summary>
    /// Delete course (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteCourse(int id)
    {
        try
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(id);
            if (course == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Course not found", 404));
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            await _unitOfWork.Courses.SoftDeleteAsync(id, userId ?? "System");
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Course deleted: {CourseId}", id);

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Course deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting course {CourseId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error deleting course", 500));
        }
    }

    /// <summary>
    /// Get enrolled students in a course
    /// </summary>
    [HttpGet("{id}/students")]
    [ProducesResponseType(typeof(ApiResponse<List<StudentListDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEnrolledStudents(int id)
    {
        try
        {
            var enrollments = await _unitOfWork.Enrollments.FindAsync(
                e => e.CourseId == id && e.Status == "Active"
            );

            var studentDtos = new List<StudentListDto>();
            foreach (var enrollment in enrollments)
            {
                var student = await _unitOfWork.Students.GetByIdWithIncludesAsync(enrollment.StudentId, s => s.User);
                if (student != null)
                {
                    studentDtos.Add(new StudentListDto
                    {
                        Id = student.Id,
                        StudentNumber = student.StudentNumber,
                        FullName = student.User.FullName,
                        Email = student.User.Email!,
                        GPA = student.GPA,
                        CurrentSemester = student.CurrentSemester,
                        Status = student.Status,
                        Major = student.Major
                    });
                }
            }

            return Ok(ApiResponse<List<StudentListDto>>.SuccessResponse(studentDtos));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving enrolled students for course {CourseId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error retrieving students", 500));
        }
    }

    /// <summary>
    /// Get course assignments
    /// </summary>
    [HttpGet("{id}/assignments")]
    [ProducesResponseType(typeof(ApiResponse<List<AssignmentDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCourseAssignments(int id)
    {
        try
        {
            var assignments = await _unitOfWork.Assignments.FindAsync(
                a => a.CourseId == id && a.IsPublished
            );

            var assignmentDtos = assignments.Select(a => new AssignmentDto
            {
                Id = a.Id,
                CourseId = a.CourseId,
                Title = a.Title,
                Description = a.Description,
                DueDate = a.DueDate,
                MaxScore = a.MaxScore,
                Weight = a.Weight,
                Type = a.Type,
                AllowLateSubmission = a.AllowLateSubmission,
                LatePenaltyPercentage = a.LatePenaltyPercentage,
                IsPublished = a.IsPublished,
                TotalSubmissions = a.TotalSubmissions,
                GradedSubmissions = a.GradedSubmissions
            }).ToList();

            return Ok(ApiResponse<List<AssignmentDto>>.SuccessResponse(assignmentDtos));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving assignments for course {CourseId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error retrieving assignments", 500));
        }
    }

    /// <summary>
    /// Assign teacher to course
    /// </summary>
    [HttpPost("{id}/assign-teacher")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> AssignTeacher(int id, [FromBody] int teacherId)
    {
        try
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(id);
            if (course == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Course not found", 404));
            }

            var teacher = await _unitOfWork.Teachers.GetByIdAsync(teacherId);
            if (teacher == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Teacher not found", 404));
            }

            course.TeacherId = teacherId;
            _unitOfWork.Courses.Update(course);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Teacher {TeacherId} assigned to course {CourseId}", teacherId, id);

            return Ok(ApiResponse<object>.SuccessResponse(null, "Teacher assigned successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning teacher to course {CourseId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error assigning teacher", 500));
        }
    }

    /// <summary>
    /// Get course schedule
    /// </summary>
    [HttpGet("{id}/schedule")]
    [ProducesResponseType(typeof(ApiResponse<List<ScheduleDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCourseSchedule(int id)
    {
        try
        {
            var schedules = await _unitOfWork.Schedules.FindAsync(s => s.CourseId == id);

            var scheduleDtos = schedules.Select(s => new ScheduleDto
            {
                Id = s.Id,
                CourseId = s.CourseId,
                DayOfWeek = s.DayOfWeek,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                Room = s.Room,
                Building = s.Building,
                Type = s.Type
            }).ToList();

            return Ok(ApiResponse<List<ScheduleDto>>.SuccessResponse(scheduleDtos));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving schedule for course {CourseId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error retrieving schedule", 500));
        }
    }
}
