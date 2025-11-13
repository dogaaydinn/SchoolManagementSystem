using SchoolManagementSystem.Core.DTOs;

namespace SchoolManagementSystem.Application.Interfaces;

public interface IGradeService
{
    Task<ApiResponse<GradeDto>> CreateGradeAsync(CreateGradeRequestDto request, string gradedBy, CancellationToken cancellationToken = default);
    Task<ApiResponse<IEnumerable<GradeDto>>> BulkCreateGradesAsync(BulkGradeRequestDto request, string gradedBy, CancellationToken cancellationToken = default);
    Task<ApiResponse<GradeDto>> UpdateGradeAsync(int id, UpdateGradeRequestDto request, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> DeleteGradeAsync(int id, CancellationToken cancellationToken = default);
    Task<ApiResponse<IEnumerable<GradeDto>>> GetStudentGradesAsync(int studentId, CancellationToken cancellationToken = default);
    Task<ApiResponse<IEnumerable<GradeDto>>> GetCourseGradesAsync(int courseId, CancellationToken cancellationToken = default);
    Task<ApiResponse<decimal>> CalculateStudentGPAAsync(int studentId, CancellationToken cancellationToken = default);
    Task<ApiResponse<GradeDistributionDto>> GetGradeDistributionAsync(int courseId, CancellationToken cancellationToken = default);
}
