using SchoolManagementSystem.Core.DTOs;

namespace SchoolManagementSystem.Core.Interfaces;

/// <summary>
/// Service interface for course management operations
/// </summary>
public interface ICourseService
{
    /// <summary>
    /// Gets a paginated list of courses
    /// </summary>
    Task<PagedResult<CourseDto>> GetAllCoursesAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null, int? departmentId = null, int? semesterId = null, bool? isActive = null);

    /// <summary>
    /// Gets a course by ID with full details
    /// </summary>
    Task<CourseDetailDto?> GetCourseByIdAsync(int id);

    /// <summary>
    /// Gets a course by course code
    /// </summary>
    Task<CourseDetailDto?> GetCourseByCourseCodeAsync(string courseCode);

    /// <summary>
    /// Creates a new course
    /// </summary>
    Task<CourseDto> CreateCourseAsync(CreateCourseRequestDto request);

    /// <summary>
    /// Updates an existing course
    /// </summary>
    Task<CourseDto?> UpdateCourseAsync(int id, UpdateCourseRequestDto request);

    /// <summary>
    /// Deletes a course (soft delete)
    /// </summary>
    Task<bool> DeleteCourseAsync(int id);

    /// <summary>
    /// Gets all students enrolled in a course
    /// </summary>
    Task<List<StudentListDto>> GetCourseEnrolledStudentsAsync(int courseId);

    /// <summary>
    /// Checks if a student can enroll in a course (prerequisites, capacity)
    /// </summary>
    Task<(bool CanEnroll, string? Reason)> CanEnrollInCourseAsync(int studentId, int courseId);

    /// <summary>
    /// Gets available courses for a student (not enrolled, prerequisites met)
    /// </summary>
    Task<List<CourseDto>> GetAvailableCoursesForStudentAsync(int studentId);
}
