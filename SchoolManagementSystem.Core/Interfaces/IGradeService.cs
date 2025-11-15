using SchoolManagementSystem.Core.DTOs;

namespace SchoolManagementSystem.Core.Interfaces;

/// <summary>
/// Service interface for grade management operations
/// </summary>
public interface IGradeService
{
    /// <summary>
    /// Gets a paginated list of grades
    /// </summary>
    Task<PagedResult<GradeDto>> GetAllGradesAsync(int pageNumber = 1, int pageSize = 10, int? studentId = null, int? courseId = null, int? assignmentId = null, bool? isPublished = null);

    /// <summary>
    /// Gets a grade by ID
    /// </summary>
    Task<GradeDto?> GetGradeByIdAsync(int id);

    /// <summary>
    /// Gets all grades for a specific student
    /// </summary>
    Task<List<GradeDto>> GetGradesByStudentIdAsync(int studentId);

    /// <summary>
    /// Gets all grades for a specific course
    /// </summary>
    Task<List<GradeDto>> GetGradesByCourseIdAsync(int courseId);

    /// <summary>
    /// Gets all grades for a specific assignment
    /// </summary>
    Task<List<GradeDto>> GetGradesByAssignmentIdAsync(int assignmentId);

    /// <summary>
    /// Creates a new grade
    /// </summary>
    Task<GradeDto> CreateGradeAsync(CreateGradeRequestDto request);

    /// <summary>
    /// Creates multiple grades in bulk (e.g., for an assignment)
    /// </summary>
    Task<List<GradeDto>> CreateBulkGradesAsync(BulkGradeRequestDto request);

    /// <summary>
    /// Updates an existing grade
    /// </summary>
    Task<GradeDto?> UpdateGradeAsync(int id, UpdateGradeRequestDto request);

    /// <summary>
    /// Deletes a grade (soft delete)
    /// </summary>
    Task<bool> DeleteGradeAsync(int id);

    /// <summary>
    /// Publishes a grade to make it visible to the student
    /// </summary>
    Task<bool> PublishGradeAsync(int id);

    /// <summary>
    /// Calculates letter grade based on percentage
    /// </summary>
    string CalculateLetterGrade(decimal percentage);
}
