using SchoolManagementSystem.Core.DTOs;

namespace SchoolManagementSystem.Core.Interfaces;

/// <summary>
/// Service interface for teacher management operations
/// </summary>
public interface ITeacherService
{
    /// <summary>
    /// Gets a paginated list of teachers
    /// </summary>
    Task<PagedResult<TeacherDto>> GetAllTeachersAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null, int? departmentId = null, bool? isActive = null);

    /// <summary>
    /// Gets a teacher by ID with full details
    /// </summary>
    Task<TeacherDetailDto?> GetTeacherByIdAsync(int id);

    /// <summary>
    /// Gets a teacher by employee number
    /// </summary>
    Task<TeacherDetailDto?> GetTeacherByEmployeeNumberAsync(string employeeNumber);

    /// <summary>
    /// Creates a new teacher
    /// </summary>
    Task<TeacherDto> CreateTeacherAsync(CreateTeacherRequestDto request);

    /// <summary>
    /// Updates an existing teacher
    /// </summary>
    Task<TeacherDto?> UpdateTeacherAsync(int id, UpdateTeacherRequestDto request);

    /// <summary>
    /// Deletes a teacher (soft delete)
    /// </summary>
    Task<bool> DeleteTeacherAsync(int id);

    /// <summary>
    /// Gets all courses taught by a teacher
    /// </summary>
    Task<List<CourseDto>> GetTeacherCoursesAsync(int teacherId);

    /// <summary>
    /// Gets all students advised by a teacher
    /// </summary>
    Task<List<StudentListDto>> GetTeacherAdviseesAsync(int teacherId);

    /// <summary>
    /// Assigns a course to a teacher
    /// </summary>
    Task<bool> AssignCourseAsync(int teacherId, int courseId);

    /// <summary>
    /// Unassigns a course from a teacher
    /// </summary>
    Task<bool> UnassignCourseAsync(int teacherId, int courseId);
}
