using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Core.DTOs;
using SchoolManagementSystem.Core.Interfaces;

namespace SchoolManagementSystem.API.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
public class StudentsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<StudentsController> _logger;

    public StudentsController(IUnitOfWork unitOfWork, ILogger<StudentsController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Get all students with pagination and filtering
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<StudentListDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStudents([FromQuery] PagedRequest request)
    {
        try
        {
            var (students, totalCount) = await _unitOfWork.Students.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                filter: s => string.IsNullOrEmpty(request.SearchTerm) ||
                            s.User.FirstName.Contains(request.SearchTerm) ||
                            s.User.LastName.Contains(request.SearchTerm) ||
                            s.StudentNumber.Contains(request.SearchTerm),
                orderBy: query => request.SortOrder?.ToLower() == "desc"
                    ? query.OrderByDescending(s => s.GPA)
                    : query.OrderBy(s => s.GPA),
                includes: s => s.User
            );

            var studentDtos = students.Select(s => new StudentListDto
            {
                Id = s.Id,
                StudentNumber = s.StudentNumber,
                FullName = s.User.FullName,
                Email = s.User.Email!,
                GPA = s.GPA,
                CurrentSemester = s.CurrentSemester,
                Status = s.Status,
                Major = s.Major
            }).ToList();

            var pagedResult = new PagedResult<StudentListDto>
            {
                Items = studentDtos,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };

            return Ok(ApiResponse<PagedResult<StudentListDto>>.SuccessResponse(pagedResult));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving students");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error retrieving students", 500));
        }
    }

    /// <summary>
    /// Get student by ID with full details
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<StudentDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudent(int id)
    {
        try
        {
            var student = await _unitOfWork.Students.GetByIdWithIncludesAsync(
                id,
                s => s.User,
                s => s.Advisor!,
                s => s.Enrollments,
                s => s.Grades
            );

            if (student == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Student not found", 404));
            }

            var studentDto = new StudentDetailDto
            {
                Id = student.Id,
                UserId = student.UserId,
                StudentNumber = student.StudentNumber,
                FirstName = student.User.FirstName,
                LastName = student.User.LastName,
                FullName = student.User.FullName,
                Email = student.User.Email!,
                DateOfBirth = student.User.DateOfBirth,
                Age = student.User.Age,
                EnrollmentDate = student.EnrollmentDate,
                GPA = student.GPA,
                CurrentSemester = student.CurrentSemester,
                Major = student.Major,
                Minor = student.Minor,
                Status = student.Status,
                TotalCreditsEarned = student.TotalCreditsEarned,
                TotalCreditsRequired = student.TotalCreditsRequired,
                Address = student.Address,
                City = student.City,
                State = student.State,
                Country = student.Country,
                PostalCode = student.PostalCode,
                EmergencyContactName = student.EmergencyContactName,
                EmergencyContactPhone = student.EmergencyContactPhone,
                ProfilePictureUrl = student.User.ProfilePictureUrl
            };

            return Ok(ApiResponse<StudentDetailDto>.SuccessResponse(studentDto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving student {StudentId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error retrieving student", 500));
        }
    }

    /// <summary>
    /// Create a new student
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin")]
    [ProducesResponseType(typeof(ApiResponse<StudentDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateStudent([FromBody] CreateStudentRequestDto request)
    {
        try
        {
            // Check if email already exists
            var existingUser = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existingUser != null)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Email already exists", 400));
            }

            // Create user
            var user = new Core.Entities.User
            {
                Email = request.Email,
                UserName = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth,
                IsActive = true,
                EmailConfirmed = false
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // Generate student number
            var studentNumber = $"STU{DateTime.UtcNow.Year}{new Random().Next(1000, 9999)}";

            // Create student
            var student = new Core.Entities.Student
            {
                UserId = user.Id,
                StudentNumber = studentNumber,
                EnrollmentDate = DateTime.UtcNow,
                Major = request.Major,
                Minor = request.Minor,
                AdvisorId = request.AdvisorId,
                Status = "Active",
                Address = request.Address,
                City = request.City,
                State = request.State,
                Country = request.Country,
                PostalCode = request.PostalCode,
                EmergencyContactName = request.EmergencyContactName,
                EmergencyContactPhone = request.EmergencyContactPhone
            };

            await _unitOfWork.Students.AddAsync(student);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Student created: {StudentNumber}", studentNumber);

            var studentDto = new StudentDto
            {
                Id = student.Id,
                StudentNumber = studentNumber,
                FullName = user.FullName,
                Email = user.Email!,
                Status = "Active"
            };

            return CreatedAtAction(nameof(GetStudent), new { id = student.Id },
                ApiResponse<StudentDto>.SuccessResponse(studentDto, "Student created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating student");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error creating student", 500));
        }
    }

    /// <summary>
    /// Update student information
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
    [ProducesResponseType(typeof(ApiResponse<StudentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStudent(int id, [FromBody] UpdateStudentRequestDto request)
    {
        try
        {
            var student = await _unitOfWork.Students.GetByIdWithIncludesAsync(id, s => s.User);
            if (student == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Student not found", 404));
            }

            // Update user info
            if (!string.IsNullOrEmpty(request.FirstName))
                student.User.FirstName = request.FirstName;
            if (!string.IsNullOrEmpty(request.LastName))
                student.User.LastName = request.LastName;
            if (request.DateOfBirth.HasValue)
                student.User.DateOfBirth = request.DateOfBirth;

            // Update student info
            if (!string.IsNullOrEmpty(request.Major))
                student.Major = request.Major;
            if (!string.IsNullOrEmpty(request.Minor))
                student.Minor = request.Minor;
            if (request.AdvisorId.HasValue)
                student.AdvisorId = request.AdvisorId;
            if (!string.IsNullOrEmpty(request.Address))
                student.Address = request.Address;
            if (!string.IsNullOrEmpty(request.City))
                student.City = request.City;
            if (!string.IsNullOrEmpty(request.State))
                student.State = request.State;
            if (!string.IsNullOrEmpty(request.Country))
                student.Country = request.Country;
            if (!string.IsNullOrEmpty(request.PostalCode))
                student.PostalCode = request.PostalCode;
            if (!string.IsNullOrEmpty(request.Status))
                student.Status = request.Status;

            _unitOfWork.Students.Update(student);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Student updated: {StudentId}", id);

            var studentDto = new StudentDto
            {
                Id = student.Id,
                StudentNumber = student.StudentNumber,
                FullName = student.User.FullName,
                Email = student.User.Email!,
                Status = student.Status
            };

            return Ok(ApiResponse<StudentDto>.SuccessResponse(studentDto, "Student updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating student {StudentId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error updating student", 500));
        }
    }

    /// <summary>
    /// Delete student (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteStudent(int id)
    {
        try
        {
            var student = await _unitOfWork.Students.GetByIdAsync(id);
            if (student == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Student not found", 404));
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            await _unitOfWork.Students.SoftDeleteAsync(id, userId ?? "System");
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Student deleted: {StudentId}", id);

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Student deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting student {StudentId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error deleting student", 500));
        }
    }

    /// <summary>
    /// Enroll student in a course
    /// </summary>
    [HttpPost("{id}/enroll")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> EnrollInCourse(int id, [FromBody] EnrollStudentRequestDto request)
    {
        try
        {
            var student = await _unitOfWork.Students.GetByIdAsync(id);
            if (student == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Student not found", 404));
            }

            var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);
            if (course == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Course not found", 404));
            }

            // Check if already enrolled
            var existingEnrollment = await _unitOfWork.Enrollments.FirstOrDefaultAsync(
                e => e.StudentId == id && e.CourseId == request.CourseId && e.Status == "Active"
            );

            if (existingEnrollment != null)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Student already enrolled in this course", 400));
            }

            // Check course capacity
            if (course.CurrentEnrollment >= course.MaxStudents)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Course is full", 400));
            }

            var enrollment = new Core.Entities.Enrollment
            {
                StudentId = id,
                CourseId = request.CourseId,
                SemesterId = request.SemesterId,
                EnrollmentDate = DateTime.UtcNow,
                Status = "Active"
            };

            await _unitOfWork.Enrollments.AddAsync(enrollment);

            // Update course enrollment count
            course.CurrentEnrollment++;
            _unitOfWork.Courses.Update(course);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Student {StudentId} enrolled in course {CourseId}", id, request.CourseId);

            return Ok(ApiResponse<object>.SuccessResponse(new { enrollmentId = enrollment.Id },
                "Student enrolled successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enrolling student {StudentId} in course", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error enrolling student", 500));
        }
    }

    /// <summary>
    /// Unenroll student from a course
    /// </summary>
    [HttpDelete("{id}/unenroll/{courseId}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UnenrollFromCourse(int id, int courseId)
    {
        try
        {
            var enrollment = await _unitOfWork.Enrollments.FirstOrDefaultAsync(
                e => e.StudentId == id && e.CourseId == courseId && e.Status == "Active"
            );

            if (enrollment == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Enrollment not found", 404));
            }

            enrollment.Status = "Dropped";
            _unitOfWork.Enrollments.Update(enrollment);

            // Update course enrollment count
            var course = await _unitOfWork.Courses.GetByIdAsync(courseId);
            if (course != null)
            {
                course.CurrentEnrollment--;
                _unitOfWork.Courses.Update(course);
            }

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Student {StudentId} unenrolled from course {CourseId}", id, courseId);

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Student unenrolled successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unenrolling student {StudentId} from course {CourseId}", id, courseId);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error unenrolling student", 500));
        }
    }

    /// <summary>
    /// Get student transcript
    /// </summary>
    [HttpGet("{id}/transcript")]
    [ProducesResponseType(typeof(ApiResponse<StudentTranscriptDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTranscript(int id)
    {
        try
        {
            var student = await _unitOfWork.Students.GetByIdWithIncludesAsync(
                id,
                s => s.User,
                s => s.Grades,
                s => s.Enrollments
            );

            if (student == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Student not found", 404));
            }

            var studentDto = new StudentDto
            {
                Id = student.Id,
                StudentNumber = student.StudentNumber,
                FullName = student.User.FullName,
                Email = student.User.Email!,
                GPA = student.GPA
            };

            var transcript = new StudentTranscriptDto
            {
                Student = studentDto,
                OverallGPA = student.GPA,
                TotalCreditsEarned = student.TotalCreditsEarned,
                GeneratedAt = DateTime.UtcNow,
                SemesterGrades = new List<SemesterGradesDto>()
            };

            return Ok(ApiResponse<StudentTranscriptDto>.SuccessResponse(transcript));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating transcript for student {StudentId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error generating transcript", 500));
        }
    }

    /// <summary>
    /// Get student's enrolled courses
    /// </summary>
    [HttpGet("{id}/courses")]
    [ProducesResponseType(typeof(ApiResponse<List<CourseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEnrolledCourses(int id)
    {
        try
        {
            var enrollments = await _unitOfWork.Enrollments.FindAsync(
                e => e.StudentId == id && e.Status == "Active"
            );

            var courseIds = enrollments.Select(e => e.CourseId).ToList();
            var courses = new List<CourseDto>();

            foreach (var courseId in courseIds)
            {
                var course = await _unitOfWork.Courses.GetByIdAsync(courseId);
                if (course != null)
                {
                    courses.Add(new CourseDto
                    {
                        Id = course.Id,
                        CourseCode = course.CourseCode,
                        CourseName = course.CourseName,
                        Credits = course.Credits
                    });
                }
            }

            return Ok(ApiResponse<List<CourseDto>>.SuccessResponse(courses));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving enrolled courses for student {StudentId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error retrieving courses", 500));
        }
    }

    /// <summary>
    /// Get student's grades
    /// </summary>
    [HttpGet("{id}/grades")]
    [ProducesResponseType(typeof(ApiResponse<List<GradeDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetGrades(int id)
    {
        try
        {
            var grades = await _unitOfWork.Grades.FindAsync(g => g.StudentId == id && g.IsPublished);

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
                    Comments = grade.Comments,
                    IsPublished = grade.IsPublished
                });
            }

            return Ok(ApiResponse<List<GradeDto>>.SuccessResponse(gradeDtos));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving grades for student {StudentId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error retrieving grades", 500));
        }
    }
}
