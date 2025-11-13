using SchoolManagementSystem.Core.DTOs;

namespace SchoolManagementSystem.Application.Interfaces;

public interface IPdfService
{
    Task<byte[]> GenerateTranscriptPdfAsync(TranscriptDto transcript, CancellationToken cancellationToken = default);
    Task<byte[]> GenerateStudentReportPdfAsync(StudentPerformanceReportDto report, CancellationToken cancellationToken = default);
    Task<byte[]> GenerateCourseReportPdfAsync(CoursePerformanceReportDto report, CancellationToken cancellationToken = default);
    Task<byte[]> GenerateGradeSheetPdfAsync(int courseId, CancellationToken cancellationToken = default);
    Task<byte[]> GenerateAttendanceReportPdfAsync(AttendanceReportDto report, CancellationToken cancellationToken = default);
    Task<byte[]> GenerateCertificatePdfAsync(int studentId, string certificateType, CancellationToken cancellationToken = default);
}
