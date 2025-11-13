using SchoolManagementSystem.Core.DTOs;

namespace SchoolManagementSystem.Application.Interfaces;

public interface IReportingService
{
    Task<ApiResponse<StudentPerformanceReportDto>> GetStudentPerformanceReportAsync(int studentId, int? semesterId = null, CancellationToken cancellationToken = default);
    Task<ApiResponse<CoursePerformanceReportDto>> GetCoursePerformanceReportAsync(int courseId, CancellationToken cancellationToken = default);
    Task<ApiResponse<TeacherPerformanceReportDto>> GetTeacherPerformanceReportAsync(int teacherId, int? semesterId = null, CancellationToken cancellationToken = default);
    Task<ApiResponse<DepartmentReportDto>> GetDepartmentReportAsync(int departmentId, CancellationToken cancellationToken = default);
    Task<ApiResponse<AttendanceReportDto>> GetAttendanceReportAsync(int studentId, int? courseId = null, CancellationToken cancellationToken = default);
    Task<ApiResponse<EnrollmentReportDto>> GetEnrollmentReportAsync(int? departmentId = null, int? semesterId = null, CancellationToken cancellationToken = default);
    Task<ApiResponse<byte[]>> ExportReportToPdfAsync(string reportType, int entityId, CancellationToken cancellationToken = default);
    Task<ApiResponse<byte[]>> ExportReportToExcelAsync(string reportType, int entityId, CancellationToken cancellationToken = default);
}
