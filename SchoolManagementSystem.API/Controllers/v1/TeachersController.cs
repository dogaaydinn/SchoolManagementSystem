using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Core.DTOs;
using SchoolManagementSystem.Core.Interfaces;

namespace SchoolManagementSystem.API.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
public class TeachersController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TeachersController> _logger;

    public TeachersController(IUnitOfWork unitOfWork, ILogger<TeachersController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<TeacherDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTeachers([FromQuery] PagedRequest request)
    {
        try
        {
            var (teachers, totalCount) = await _unitOfWork.Teachers.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                filter: t => (string.IsNullOrEmpty(request.SearchTerm) ||
                             t.User.FirstName.Contains(request.SearchTerm) ||
                             t.User.LastName.Contains(request.SearchTerm)) &&
                             t.IsActive,
                orderBy: query => query.OrderBy(t => t.User.LastName),
                includes: t => t.User, t => t.Department!
            );

            var teacherDtos = teachers.Select(t => new TeacherDto
            {
                Id = t.Id,
                UserId = t.UserId,
                EmployeeNumber = t.EmployeeNumber,
                FirstName = t.User.FirstName,
                LastName = t.User.LastName,
                FullName = t.User.FullName,
                Email = t.User.Email!,
                Specialization = t.Specialization,
                Qualification = t.Qualification,
                DepartmentName = t.Department?.Name,
                HireDate = t.HireDate,
                EmploymentType = t.EmploymentType,
                IsActive = t.IsActive
            }).ToList();

            var pagedResult = new PagedResult<TeacherDto>
            {
                Items = teacherDtos,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };

            return Ok(ApiResponse<PagedResult<TeacherDto>>.SuccessResponse(pagedResult));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving teachers");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error retrieving teachers", 500));
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<TeacherDetailDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTeacher(int id)
    {
        try
        {
            var teacher = await _unitOfWork.Teachers.GetByIdWithIncludesAsync(
                id,
                t => t.User,
                t => t.Department!,
                t => t.Courses,
                t => t.Advisees
            );

            if (teacher == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Teacher not found", 404));
            }

            var teacherDto = new TeacherDetailDto
            {
                Id = teacher.Id,
                UserId = teacher.UserId,
                EmployeeNumber = teacher.EmployeeNumber,
                FirstName = teacher.User.FirstName,
                LastName = teacher.User.LastName,
                FullName = teacher.User.FullName,
                Email = teacher.User.Email!,
                Specialization = teacher.Specialization,
                Qualification = teacher.Qualification,
                DepartmentName = teacher.Department?.Name,
                HireDate = teacher.HireDate,
                EmploymentType = teacher.EmploymentType,
                IsActive = teacher.IsActive,
                OfficeLocation = teacher.OfficeLocation,
                OfficeHours = teacher.OfficeHours,
                Biography = teacher.Biography,
                ResearchInterests = teacher.ResearchInterests,
                Courses = teacher.Courses.Select(c => new CourseDto
                {
                    Id = c.Id,
                    CourseCode = c.CourseCode,
                    CourseName = c.CourseName,
                    Credits = c.Credits
                }).ToList()
            };

            return Ok(ApiResponse<TeacherDetailDto>.SuccessResponse(teacherDto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving teacher {TeacherId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error retrieving teacher", 500));
        }
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin")]
    [ProducesResponseType(typeof(ApiResponse<TeacherDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateTeacher([FromBody] CreateTeacherRequestDto request)
    {
        try
        {
            var existingUser = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existingUser != null)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Email already exists", 400));
            }

            var user = new Core.Entities.User
            {
                Email = request.Email,
                UserName = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth,
                IsActive = true
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var teacher = new Core.Entities.Teacher
            {
                UserId = user.Id,
                EmployeeNumber = $"EMP{DateTime.UtcNow.Year}{new Random().Next(1000, 9999)}",
                Specialization = request.Specialization,
                Qualification = request.Qualification,
                DepartmentId = request.DepartmentId,
                Salary = request.Salary,
                EmploymentType = request.EmploymentType,
                OfficeLocation = request.OfficeLocation,
                OfficeHours = request.OfficeHours,
                HireDate = request.HireDate,
                IsActive = true
            };

            await _unitOfWork.Teachers.AddAsync(teacher);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Teacher created: {EmployeeNumber}", teacher.EmployeeNumber);

            var teacherDto = new TeacherDto
            {
                Id = teacher.Id,
                EmployeeNumber = teacher.EmployeeNumber,
                FullName = user.FullName,
                Email = user.Email!
            };

            return CreatedAtAction(nameof(GetTeacher), new { id = teacher.Id },
                ApiResponse<TeacherDto>.SuccessResponse(teacherDto, "Teacher created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating teacher");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error creating teacher", 500));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    [ProducesResponseType(typeof(ApiResponse<TeacherDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateTeacher(int id, [FromBody] UpdateTeacherRequestDto request)
    {
        try
        {
            var teacher = await _unitOfWork.Teachers.GetByIdWithIncludesAsync(id, t => t.User);
            if (teacher == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Teacher not found", 404));
            }

            if (!string.IsNullOrEmpty(request.FirstName))
                teacher.User.FirstName = request.FirstName;
            if (!string.IsNullOrEmpty(request.LastName))
                teacher.User.LastName = request.LastName;
            if (!string.IsNullOrEmpty(request.Specialization))
                teacher.Specialization = request.Specialization;
            if (!string.IsNullOrEmpty(request.Qualification))
                teacher.Qualification = request.Qualification;
            if (request.DepartmentId.HasValue)
                teacher.DepartmentId = request.DepartmentId;
            if (request.Salary.HasValue)
                teacher.Salary = request.Salary.Value;
            if (!string.IsNullOrEmpty(request.OfficeLocation))
                teacher.OfficeLocation = request.OfficeLocation;
            if (!string.IsNullOrEmpty(request.OfficeHours))
                teacher.OfficeHours = request.OfficeHours;
            if (!string.IsNullOrEmpty(request.Biography))
                teacher.Biography = request.Biography;
            if (request.IsActive.HasValue)
                teacher.IsActive = request.IsActive.Value;

            _unitOfWork.Teachers.Update(teacher);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Teacher updated: {TeacherId}", id);

            return Ok(ApiResponse<TeacherDto>.SuccessResponse(new TeacherDto
            {
                Id = teacher.Id,
                FullName = teacher.User.FullName,
                Email = teacher.User.Email!
            }, "Teacher updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating teacher {TeacherId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error updating teacher", 500));
        }
    }

    [HttpGet("{id}/courses")]
    [ProducesResponseType(typeof(ApiResponse<List<CourseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTeacherCourses(int id)
    {
        try
        {
            var courses = await _unitOfWork.Courses.FindAsync(c => c.TeacherId == id && c.IsActive);

            var courseDtos = courses.Select(c => new CourseDto
            {
                Id = c.Id,
                CourseCode = c.CourseCode,
                CourseName = c.CourseName,
                Credits = c.Credits,
                CurrentEnrollment = c.CurrentEnrollment,
                MaxStudents = c.MaxStudents
            }).ToList();

            return Ok(ApiResponse<List<CourseDto>>.SuccessResponse(courseDtos));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving courses for teacher {TeacherId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error retrieving courses", 500));
        }
    }
}
