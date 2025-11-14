using SchoolManagementSystem.Core.DTOs;

namespace SchoolManagementSystem.Application.Interfaces;

public interface IStudentService
{
    Task<ApiResponse<PagedResult<StudentListDto>>> GetStudentsAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<StudentDetailDto>> GetStudentByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ApiResponse<StudentDetailDto>> CreateStudentAsync(CreateStudentRequestDto request, CancellationToken cancellationToken = default);
    Task<ApiResponse<StudentDetailDto>> UpdateStudentAsync(int id, UpdateStudentRequestDto request, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> DeleteStudentAsync(int id, string deletedBy, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> EnrollStudentAsync(EnrollStudentRequestDto request, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> UnenrollStudentAsync(int studentId, int courseId, CancellationToken cancellationToken = default);
    Task<ApiResponse<TranscriptDto>> GetStudentTranscriptAsync(int id, CancellationToken cancellationToken = default);
    Task<ApiResponse<IEnumerable<CourseDto>>> GetStudentCoursesAsync(int id, CancellationToken cancellationToken = default);
    Task<ApiResponse<IEnumerable<GradeDto>>> GetStudentGradesAsync(int id, CancellationToken cancellationToken = default);
    Task<ApiResponse<StudentStatisticsDto>> GetStudentStatisticsAsync(int id, CancellationToken cancellationToken = default);
}
