using SchoolManagementSystem.Core.DTOs;

namespace SchoolManagementSystem.Application.Interfaces;

public interface INotificationService
{
    Task<bool> SendEmailAsync(EmailNotificationDto notification, CancellationToken cancellationToken = default);
    Task<bool> SendSmsAsync(SmsNotificationDto notification, CancellationToken cancellationToken = default);
    Task<bool> SendBulkEmailAsync(BulkEmailNotificationDto notification, CancellationToken cancellationToken = default);
    Task<bool> NotifyGradePostedAsync(int studentId, int gradeId, CancellationToken cancellationToken = default);
    Task<bool> NotifyAssignmentDueAsync(int studentId, int assignmentId, CancellationToken cancellationToken = default);
    Task<bool> NotifyEnrollmentConfirmationAsync(int studentId, int courseId, CancellationToken cancellationToken = default);
    Task<bool> NotifyAttendanceWarningAsync(int studentId, int courseId, decimal attendancePercentage, CancellationToken cancellationToken = default);
}
