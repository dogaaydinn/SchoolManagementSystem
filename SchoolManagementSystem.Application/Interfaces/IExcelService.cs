using SchoolManagementSystem.Core.DTOs;

namespace SchoolManagementSystem.Application.Interfaces;

public interface IExcelService
{
    Task<byte[]> ExportStudentsToExcelAsync(IEnumerable<StudentListDto> students, CancellationToken cancellationToken = default);
    Task<byte[]> ExportGradesToExcelAsync(IEnumerable<GradeDto> grades, CancellationToken cancellationToken = default);
    Task<byte[]> ExportAttendanceToExcelAsync(IEnumerable<AttendanceDto> attendances, CancellationToken cancellationToken = default);
    Task<byte[]> ExportCourseRosterToExcelAsync(int courseId, CancellationToken cancellationToken = default);
    Task<byte[]> ExportTranscriptToExcelAsync(TranscriptDto transcript, CancellationToken cancellationToken = default);
    Task<ApiResponse<BulkImportResultDto>> ImportStudentsFromExcelAsync(Stream excelStream, CancellationToken cancellationToken = default);
    Task<ApiResponse<BulkImportResultDto>> ImportGradesFromExcelAsync(Stream excelStream, CancellationToken cancellationToken = default);
}
