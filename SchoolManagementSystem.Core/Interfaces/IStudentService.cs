using SchoolManagementSystem.Core.DTOs;

namespace SchoolManagementSystem.Core.Interfaces;

/// <summary>
/// Service interface for student management operations
/// </summary>
public interface IStudentService
{
    /// <summary>
    /// Gets a paginated list of students
    /// </summary>
    Task<PagedResult<StudentListDto>> GetAllStudentsAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null, string? status = null);

    /// <summary>
    /// Gets a student by ID with full details
    /// </summary>
    Task<StudentDetailDto?> GetStudentByIdAsync(int id);

    /// <summary>
    /// Gets a student by student number
    /// </summary>
    Task<StudentDetailDto?> GetStudentByNumberAsync(string studentNumber);

    /// <summary>
    /// Creates a new student
    /// </summary>
    Task<StudentDto> CreateStudentAsync(CreateStudentRequestDto request);

    /// <summary>
    /// Updates an existing student
    /// </summary>
    Task<StudentDto?> UpdateStudentAsync(int id, UpdateStudentRequestDto request);

    /// <summary>
    /// Deletes a student (soft delete)
    /// </summary>
    Task<bool> DeleteStudentAsync(int id);

    /// <summary>
    /// Gets student transcript with all grades
    /// </summary>
    Task<StudentTranscriptDto?> GetStudentTranscriptAsync(int id);

    /// <summary>
    /// Enrolls a student in a course
    /// </summary>
    Task<bool> EnrollInCourseAsync(int studentId, int courseId);

    /// <summary>
    /// Unenrolls a student from a course
    /// </summary>
    Task<bool> UnenrollFromCourseAsync(int studentId, int courseId);

    /// <summary>
    /// Gets all courses a student is enrolled in
    /// </summary>
    Task<List<CourseDto>> GetEnrolledCoursesAsync(int studentId);

    /// <summary>
    /// Calculates and updates student GPA
    /// </summary>
    Task<decimal> CalculateGPAAsync(int studentId);
}
