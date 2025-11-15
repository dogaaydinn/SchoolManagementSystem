using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Core.DTOs;
using SchoolManagementSystem.Core.Entities;
using SchoolManagementSystem.Core.Interfaces;

namespace SchoolManagementSystem.Application.Services;

/// <summary>
/// Service implementation for teacher management operations
/// </summary>
public class TeacherService : ITeacherService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TeacherService> _logger;

    public TeacherService(IUnitOfWork unitOfWork, ILogger<TeacherService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<PagedResult<TeacherDto>> GetAllTeachersAsync(
        int pageNumber = 1,
        int pageSize = 10,
        string? searchTerm = null,
        int? departmentId = null,
        bool? isActive = null)
    {
        try
        {
            var query = _unitOfWork.Teachers.GetAllQueryable()
                .Include(t => t.User)
                .Include(t => t.Department)
                .Where(t => !t.IsDeleted);

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(t =>
                    t.User!.FirstName.Contains(searchTerm) ||
                    t.User.LastName.Contains(searchTerm) ||
                    t.User.Email!.Contains(searchTerm) ||
                    t.EmployeeNumber.Contains(searchTerm) ||
                    (t.Specialization != null && t.Specialization.Contains(searchTerm)));
            }

            // Apply department filter
            if (departmentId.HasValue)
            {
                query = query.Where(t => t.DepartmentId == departmentId.Value);
            }

            // Apply active status filter
            if (isActive.HasValue)
            {
                query = query.Where(t => t.IsActive == isActive.Value);
            }

            var totalCount = await query.CountAsync();

            var teachers = await query
                .OrderBy(t => t.EmployeeNumber)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TeacherDto
                {
                    Id = t.Id,
                    UserId = t.UserId,
                    EmployeeNumber = t.EmployeeNumber,
                    FirstName = t.User!.FirstName,
                    LastName = t.User.LastName,
                    FullName = t.User.FullName,
                    Email = t.User.Email!,
                    Specialization = t.Specialization,
                    Qualification = t.Qualification,
                    DepartmentName = t.Department != null ? t.Department.Name : null,
                    HireDate = t.HireDate,
                    EmploymentType = t.EmploymentType,
                    IsActive = t.IsActive
                })
                .ToListAsync();

            return new PagedResult<TeacherDto>
            {
                Items = teachers,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving teachers list");
            throw;
        }
    }

    public async Task<TeacherDetailDto?> GetTeacherByIdAsync(int id)
    {
        try
        {
            var teacher = await _unitOfWork.Teachers.GetByIdWithIncludesAsync(
                id,
                t => t.User!,
                t => t.Department!,
                t => t.Courses!,
                t => t.Advisees!);

            if (teacher == null || teacher.IsDeleted)
            {
                return null;
            }

            return await MapToDetailDtoAsync(teacher);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving teacher with ID {TeacherId}", id);
            throw;
        }
    }

    public async Task<TeacherDetailDto?> GetTeacherByEmployeeNumberAsync(string employeeNumber)
    {
        try
        {
            var teachers = await _unitOfWork.Teachers.FindAsync(t => t.EmployeeNumber == employeeNumber && !t.IsDeleted);
            var teacher = teachers.FirstOrDefault();

            if (teacher == null)
            {
                return null;
            }

            // Load related data
            await _unitOfWork.Context.Entry(teacher)
                .Reference(t => t.User)
                .LoadAsync();

            await _unitOfWork.Context.Entry(teacher)
                .Reference(t => t.Department)
                .LoadAsync();

            await _unitOfWork.Context.Entry(teacher)
                .Collection(t => t.Courses!)
                .LoadAsync();

            await _unitOfWork.Context.Entry(teacher)
                .Collection(t => t.Advisees!)
                .LoadAsync();

            return await MapToDetailDtoAsync(teacher);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving teacher with employee number {EmployeeNumber}", employeeNumber);
            throw;
        }
    }

    public async Task<TeacherDto> CreateTeacherAsync(CreateTeacherRequestDto request)
    {
        try
        {
            // Check if email already exists
            var existingUser = await _unitOfWork.Users.FindAsync(u => u.Email == request.Email);
            if (existingUser.Any())
            {
                throw new InvalidOperationException("A user with this email already exists");
            }

            // Create user first
            var user = new User
            {
                Email = request.Email,
                UserName = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth,
                IsActive = true,
                EmailConfirmed = false,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // Generate employee number
            var employeeNumber = await GenerateEmployeeNumberAsync();

            // Create teacher
            var teacher = new Teacher
            {
                UserId = user.Id,
                EmployeeNumber = employeeNumber,
                HireDate = request.HireDate != default ? request.HireDate : DateTime.UtcNow,
                Specialization = request.Specialization,
                Qualification = request.Qualification,
                DepartmentId = request.DepartmentId,
                Salary = request.Salary,
                EmploymentType = request.EmploymentType,
                OfficeLocation = request.OfficeLocation,
                OfficeHours = request.OfficeHours,
                IsActive = true,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Teachers.AddAsync(teacher);
            await _unitOfWork.SaveChangesAsync();

            // Load department for DTO mapping
            if (teacher.DepartmentId.HasValue)
            {
                await _unitOfWork.Context.Entry(teacher)
                    .Reference(t => t.Department)
                    .LoadAsync();
            }

            _logger.LogInformation("Created teacher {EmployeeNumber} for user {Email}", employeeNumber, request.Email);

            return MapToDto(teacher, user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating teacher for email {Email}", request.Email);
            throw;
        }
    }

    public async Task<TeacherDto?> UpdateTeacherAsync(int id, UpdateTeacherRequestDto request)
    {
        try
        {
            var teacher = await _unitOfWork.Teachers.GetByIdWithIncludesAsync(id, t => t.User!, t => t.Department!);

            if (teacher == null || teacher.IsDeleted)
            {
                return null;
            }

            // Update user fields
            if (request.FirstName != null)
                teacher.User!.FirstName = request.FirstName;

            if (request.LastName != null)
                teacher.User!.LastName = request.LastName;

            // Update teacher fields
            if (request.Specialization != null)
                teacher.Specialization = request.Specialization;

            if (request.Qualification != null)
                teacher.Qualification = request.Qualification;

            if (request.DepartmentId.HasValue)
                teacher.DepartmentId = request.DepartmentId;

            if (request.Salary.HasValue)
                teacher.Salary = request.Salary.Value;

            if (request.EmploymentType != null)
                teacher.EmploymentType = request.EmploymentType;

            if (request.OfficeLocation != null)
                teacher.OfficeLocation = request.OfficeLocation;

            if (request.OfficeHours != null)
                teacher.OfficeHours = request.OfficeHours;

            if (request.Biography != null)
                teacher.Biography = request.Biography;

            if (request.ResearchInterests != null)
                teacher.ResearchInterests = request.ResearchInterests;

            if (request.IsActive.HasValue)
                teacher.IsActive = request.IsActive.Value;

            teacher.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Teachers.Update(teacher);
            await _unitOfWork.SaveChangesAsync();

            // Reload department if it was updated
            if (request.DepartmentId.HasValue)
            {
                await _unitOfWork.Context.Entry(teacher)
                    .Reference(t => t.Department)
                    .LoadAsync();
            }

            _logger.LogInformation("Updated teacher {TeacherId}", id);

            return MapToDto(teacher, teacher.User!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating teacher {TeacherId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteTeacherAsync(int id)
    {
        try
        {
            var teacher = await _unitOfWork.Teachers.GetByIdAsync(id);

            if (teacher == null || teacher.IsDeleted)
            {
                return false;
            }

            // Soft delete
            teacher.IsDeleted = true;
            teacher.IsActive = false;
            teacher.DeletedAt = DateTime.UtcNow;

            _unitOfWork.Teachers.Update(teacher);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Deleted teacher {TeacherId}", id);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting teacher {TeacherId}", id);
            throw;
        }
    }

    public async Task<List<CourseDto>> GetTeacherCoursesAsync(int teacherId)
    {
        try
        {
            var teacher = await _unitOfWork.Teachers.GetByIdWithIncludesAsync(teacherId, t => t.Courses!);

            if (teacher == null || teacher.IsDeleted)
            {
                return new List<CourseDto>();
            }

            var courses = teacher.Courses?
                .Where(c => !c.IsDeleted)
                .Select(c => new CourseDto
                {
                    Id = c.Id,
                    CourseCode = c.CourseCode,
                    Name = c.Name,
                    Description = c.Description,
                    Credits = c.Credits,
                    Department = c.Department != null ? c.Department.Name : null,
                    Semester = c.Semester
                })
                .ToList() ?? new List<CourseDto>();

            _logger.LogInformation("Retrieved {Count} courses for teacher {TeacherId}", courses.Count, teacherId);

            return courses;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving courses for teacher {TeacherId}", teacherId);
            throw;
        }
    }

    public async Task<List<StudentListDto>> GetTeacherAdviseesAsync(int teacherId)
    {
        try
        {
            var teacher = await _unitOfWork.Teachers.GetByIdWithIncludesAsync(teacherId, t => t.Advisees!);

            if (teacher == null || teacher.IsDeleted)
            {
                return new List<StudentListDto>();
            }

            // Load user data for each advisee
            var advisees = new List<StudentListDto>();

            foreach (var student in teacher.Advisees ?? Enumerable.Empty<Student>())
            {
                if (student.IsDeleted)
                    continue;

                await _unitOfWork.Context.Entry(student)
                    .Reference(s => s.User)
                    .LoadAsync();

                advisees.Add(new StudentListDto
                {
                    Id = student.Id,
                    StudentNumber = student.StudentNumber,
                    FullName = student.User!.FullName,
                    Email = student.User.Email!,
                    GPA = student.GPA,
                    CurrentSemester = student.CurrentSemester,
                    Status = student.Status,
                    Major = student.Major
                });
            }

            _logger.LogInformation("Retrieved {Count} advisees for teacher {TeacherId}", advisees.Count, teacherId);

            return advisees;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving advisees for teacher {TeacherId}", teacherId);
            throw;
        }
    }

    public async Task<bool> AssignCourseAsync(int teacherId, int courseId)
    {
        try
        {
            var teacher = await _unitOfWork.Teachers.GetByIdAsync(teacherId);
            if (teacher == null || teacher.IsDeleted)
            {
                return false;
            }

            var course = await _unitOfWork.Courses.GetByIdAsync(courseId);
            if (course == null || course.IsDeleted)
            {
                return false;
            }

            // Update course teacher
            course.TeacherId = teacherId;
            course.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Courses.Update(course);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Assigned course {CourseId} to teacher {TeacherId}", courseId, teacherId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning course {CourseId} to teacher {TeacherId}", courseId, teacherId);
            throw;
        }
    }

    public async Task<bool> UnassignCourseAsync(int teacherId, int courseId)
    {
        try
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(courseId);

            if (course == null || course.TeacherId != teacherId)
            {
                return false;
            }

            // Remove teacher assignment
            course.TeacherId = null;
            course.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Courses.Update(course);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Unassigned course {CourseId} from teacher {TeacherId}", courseId, teacherId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unassigning course {CourseId} from teacher {TeacherId}", courseId, teacherId);
            throw;
        }
    }

    #region Private Helper Methods

    private async Task<string> GenerateEmployeeNumberAsync()
    {
        var year = DateTime.UtcNow.Year;
        var random = System.Security.Cryptography.RandomNumberGenerator.GetInt32(1000, 10000);
        var employeeNumber = $"EMP{year}{random}";

        // Ensure uniqueness
        var existing = await _unitOfWork.Teachers.FindAsync(t => t.EmployeeNumber == employeeNumber);
        if (existing.Any())
        {
            // Recursively try again
            return await GenerateEmployeeNumberAsync();
        }

        return employeeNumber;
    }

    private TeacherDto MapToDto(Teacher teacher, User user)
    {
        return new TeacherDto
        {
            Id = teacher.Id,
            UserId = teacher.UserId,
            EmployeeNumber = teacher.EmployeeNumber,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = user.FullName,
            Email = user.Email!,
            Specialization = teacher.Specialization,
            Qualification = teacher.Qualification,
            DepartmentName = teacher.Department?.Name,
            HireDate = teacher.HireDate,
            EmploymentType = teacher.EmploymentType,
            IsActive = teacher.IsActive
        };
    }

    private async Task<TeacherDetailDto> MapToDetailDtoAsync(Teacher teacher)
    {
        var dto = new TeacherDetailDto
        {
            Id = teacher.Id,
            UserId = teacher.UserId,
            EmployeeNumber = teacher.EmployeeNumber,
            FirstName = teacher.User!.FirstName,
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
            Courses = new List<CourseDto>(),
            Advisees = new List<StudentListDto>()
        };

        // Map courses
        if (teacher.Courses != null)
        {
            foreach (var course in teacher.Courses.Where(c => !c.IsDeleted))
            {
                // Load department for each course if needed
                if (course.DepartmentId.HasValue && course.Department == null)
                {
                    await _unitOfWork.Context.Entry(course)
                        .Reference(c => c.Department)
                        .LoadAsync();
                }

                dto.Courses.Add(new CourseDto
                {
                    Id = course.Id,
                    CourseCode = course.CourseCode,
                    Name = course.Name,
                    Description = course.Description,
                    Credits = course.Credits,
                    Department = course.Department?.Name,
                    Semester = course.Semester
                });
            }
        }

        // Map advisees
        if (teacher.Advisees != null)
        {
            foreach (var student in teacher.Advisees.Where(s => !s.IsDeleted))
            {
                // Load user data if not already loaded
                if (student.User == null)
                {
                    await _unitOfWork.Context.Entry(student)
                        .Reference(s => s.User)
                        .LoadAsync();
                }

                dto.Advisees.Add(new StudentListDto
                {
                    Id = student.Id,
                    StudentNumber = student.StudentNumber,
                    FullName = student.User!.FullName,
                    Email = student.User.Email!,
                    GPA = student.GPA,
                    CurrentSemester = student.CurrentSemester,
                    Status = student.Status,
                    Major = student.Major
                });
            }
        }

        return dto;
    }

    #endregion
}
