using SchoolManagementSystem.Core.DTOs;

namespace SchoolManagementSystem.Application.Interfaces;

public interface ICourseService
{
    Task<ApiResponse<PagedResult<CourseDto>>> GetCoursesAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<CourseDetailDto>> GetCourseByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ApiResponse<CourseDetailDto>> CreateCourseAsync(CreateCourseRequestDto request, CancellationToken cancellationToken = default);
    Task<ApiResponse<CourseDetailDto>> UpdateCourseAsync(int id, UpdateCourseRequestDto request, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> DeleteCourseAsync(int id, string deletedBy, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> AssignTeacherAsync(int courseId, int teacherId, CancellationToken cancellationToken = default);
    Task<ApiResponse<IEnumerable<StudentListDto>>> GetCourseStudentsAsync(int id, CancellationToken cancellationToken = default);
    Task<ApiResponse<IEnumerable<AssignmentDto>>> GetCourseAssignmentsAsync(int id, CancellationToken cancellationToken = default);
    Task<ApiResponse<IEnumerable<ScheduleDto>>> GetCourseScheduleAsync(int id, CancellationToken cancellationToken = default);
    Task<ApiResponse<CourseStatisticsDto>> GetCourseStatisticsAsync(int id, CancellationToken cancellationToken = default);
}
