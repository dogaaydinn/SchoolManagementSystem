using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using SchoolManagementSystem.Application.Interfaces;
using SchoolManagementSystem.Core.DTOs;
using SchoolManagementSystem.Core.Interfaces;
using System.Net;
using System.Net.Mail;

namespace SchoolManagementSystem.Application.Services;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<NotificationService> _logger;
    private readonly IConfiguration _configuration;
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _smtpUsername;
    private readonly string _smtpPassword;
    private readonly string _fromEmail;
    private readonly string _fromName;

    public NotificationService(
        IUnitOfWork unitOfWork,
        ILogger<NotificationService> logger,
        IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _configuration = configuration;

        _smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
        _smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
        _smtpUsername = _configuration["Email:Username"] ?? "";
        _smtpPassword = _configuration["Email:Password"] ?? "";
        _fromEmail = _configuration["Email:FromEmail"] ?? "noreply@school.edu";
        _fromName = _configuration["Email:FromName"] ?? "School Management System";
    }

    public async Task<bool> SendEmailAsync(
        EmailNotificationDto notification,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var client = new SmtpClient(_smtpHost, _smtpPort)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_smtpUsername, _smtpPassword)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromEmail, _fromName),
                Subject = notification.Subject,
                Body = notification.Body,
                IsBodyHtml = notification.IsHtml
            };

            mailMessage.To.Add(new MailAddress(notification.ToEmail, notification.ToName));

            if (!string.IsNullOrEmpty(notification.CcEmails))
            {
                foreach (var cc in notification.CcEmails.Split(';'))
                {
                    if (!string.IsNullOrWhiteSpace(cc))
                        mailMessage.CC.Add(cc.Trim());
                }
            }

            if (!string.IsNullOrEmpty(notification.BccEmails))
            {
                foreach (var bcc in notification.BccEmails.Split(';'))
                {
                    if (!string.IsNullOrWhiteSpace(bcc))
                        mailMessage.Bcc.Add(bcc.Trim());
                }
            }

            if (notification.Attachments != null)
            {
                foreach (var attachment in notification.Attachments)
                {
                    var stream = new MemoryStream(attachment.Content);
                    mailMessage.Attachments.Add(new Attachment(stream, attachment.FileName, attachment.ContentType));
                }
            }

            await client.SendMailAsync(mailMessage, cancellationToken);

            _logger.LogInformation("Email sent successfully to {Email}", notification.ToEmail);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to {Email}", notification.ToEmail);
            return false;
        }
    }

    public async Task<bool> SendSmsAsync(
        SmsNotificationDto notification,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // This would integrate with Twilio, AWS SNS, or another SMS service
            // For now, logging the SMS attempt
            _logger.LogInformation(
                "SMS would be sent to {PhoneNumber}: {Message}",
                notification.PhoneNumber,
                notification.Message
            );

            // Placeholder for actual SMS implementation
            return await Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending SMS to {PhoneNumber}", notification.PhoneNumber);
            return false;
        }
    }

    public async Task<bool> SendBulkEmailAsync(
        BulkEmailNotificationDto notification,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var tasks = notification.ToEmails.Select(email =>
                SendEmailAsync(new EmailNotificationDto
                {
                    ToEmail = email,
                    ToName = "",
                    Subject = notification.Subject,
                    Body = notification.Body,
                    IsHtml = notification.IsHtml
                }, cancellationToken)
            );

            var results = await Task.WhenAll(tasks);
            var successCount = results.Count(r => r);

            _logger.LogInformation(
                "Bulk email sent: {SuccessCount}/{TotalCount} successful",
                successCount,
                notification.ToEmails.Count
            );

            return successCount > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending bulk email");
            return false;
        }
    }

    public async Task<bool> NotifyGradePostedAsync(
        int studentId,
        int gradeId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var student = await _unitOfWork.Students.GetByIdAsync(studentId, cancellationToken);
            if (student == null)
            {
                _logger.LogWarning("Student {StudentId} not found", studentId);
                return false;
            }

            var grade = await _unitOfWork.Grades.GetByIdAsync(gradeId, cancellationToken);
            if (grade == null)
            {
                _logger.LogWarning("Grade {GradeId} not found", gradeId);
                return false;
            }

            var course = await _unitOfWork.Courses.GetByIdAsync(grade.CourseId, cancellationToken);
            if (course == null)
            {
                _logger.LogWarning("Course {CourseId} not found", grade.CourseId);
                return false;
            }

            var emailBody = $@"
                <html>
                <body>
                    <h2>New Grade Posted</h2>
                    <p>Dear {student.User.FirstName},</p>
                    <p>A new grade has been posted for your course:</p>
                    <ul>
                        <li><strong>Course:</strong> {course.CourseName} ({course.CourseCode})</li>
                        <li><strong>Assignment Type:</strong> {grade.GradeType}</li>
                        <li><strong>Grade:</strong> {grade.Percentage:F2}% ({grade.LetterGrade})</li>
                        <li><strong>Score:</strong> {grade.Value}/{grade.MaxValue}</li>
                    </ul>
                    {(!string.IsNullOrEmpty(grade.Comments) ? $"<p><strong>Comments:</strong> {grade.Comments}</p>" : "")}
                    <p>Best regards,<br/>School Management System</p>
                </body>
                </html>";

            var notification = new EmailNotificationDto
            {
                ToEmail = student.User.Email,
                ToName = student.User.FullName,
                Subject = $"New Grade Posted - {course.CourseCode}",
                Body = emailBody,
                IsHtml = true
            };

            return await SendEmailAsync(notification, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending grade notification");
            return false;
        }
    }

    public async Task<bool> NotifyAssignmentDueAsync(
        int studentId,
        int assignmentId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var student = await _unitOfWork.Students.GetByIdAsync(studentId, cancellationToken);
            if (student == null) return false;

            var assignment = await _unitOfWork.Assignments.GetByIdAsync(assignmentId, cancellationToken);
            if (assignment == null) return false;

            var course = await _unitOfWork.Courses.GetByIdAsync(assignment.CourseId, cancellationToken);
            if (course == null) return false;

            var hoursUntilDue = (assignment.DueDate - DateTime.UtcNow).TotalHours;

            var emailBody = $@"
                <html>
                <body>
                    <h2>Assignment Due Reminder</h2>
                    <p>Dear {student.User.FirstName},</p>
                    <p>This is a reminder that the following assignment is due soon:</p>
                    <ul>
                        <li><strong>Course:</strong> {course.CourseName} ({course.CourseCode})</li>
                        <li><strong>Assignment:</strong> {assignment.Title}</li>
                        <li><strong>Due Date:</strong> {assignment.DueDate:f}</li>
                        <li><strong>Time Remaining:</strong> {(int)hoursUntilDue} hours</li>
                    </ul>
                    {(!string.IsNullOrEmpty(assignment.Description) ? $"<p><strong>Description:</strong> {assignment.Description}</p>" : "")}
                    <p>Please ensure you submit your work before the deadline.</p>
                    <p>Best regards,<br/>School Management System</p>
                </body>
                </html>";

            var notification = new EmailNotificationDto
            {
                ToEmail = student.User.Email,
                ToName = student.User.FullName,
                Subject = $"Assignment Due Soon - {assignment.Title}",
                Body = emailBody,
                IsHtml = true
            };

            return await SendEmailAsync(notification, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending assignment due notification");
            return false;
        }
    }

    public async Task<bool> NotifyEnrollmentConfirmationAsync(
        int studentId,
        int courseId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var student = await _unitOfWork.Students.GetByIdAsync(studentId, cancellationToken);
            if (student == null) return false;

            var course = await _unitOfWork.Courses.GetByIdAsync(courseId, cancellationToken);
            if (course == null) return false;

            var emailBody = $@"
                <html>
                <body>
                    <h2>Enrollment Confirmation</h2>
                    <p>Dear {student.User.FirstName},</p>
                    <p>You have been successfully enrolled in the following course:</p>
                    <ul>
                        <li><strong>Course:</strong> {course.CourseName}</li>
                        <li><strong>Course Code:</strong> {course.CourseCode}</li>
                        <li><strong>Credits:</strong> {course.Credits}</li>
                        <li><strong>Instructor:</strong> {course.Teacher?.User.FullName ?? "TBA"}</li>
                    </ul>
                    {(!string.IsNullOrEmpty(course.Description) ? $"<p><strong>Description:</strong> {course.Description}</p>" : "")}
                    <p>Please check your schedule for class times and locations.</p>
                    <p>Best regards,<br/>School Management System</p>
                </body>
                </html>";

            var notification = new EmailNotificationDto
            {
                ToEmail = student.User.Email,
                ToName = student.User.FullName,
                Subject = $"Enrollment Confirmation - {course.CourseCode}",
                Body = emailBody,
                IsHtml = true
            };

            return await SendEmailAsync(notification, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending enrollment confirmation");
            return false;
        }
    }

    public async Task<bool> NotifyAttendanceWarningAsync(
        int studentId,
        int courseId,
        decimal attendancePercentage,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var student = await _unitOfWork.Students.GetByIdAsync(studentId, cancellationToken);
            if (student == null) return false;

            var course = await _unitOfWork.Courses.GetByIdAsync(courseId, cancellationToken);
            if (course == null) return false;

            var emailBody = $@"
                <html>
                <body>
                    <h2>Attendance Warning</h2>
                    <p>Dear {student.User.FirstName},</p>
                    <p>Your attendance in the following course is below the required threshold:</p>
                    <ul>
                        <li><strong>Course:</strong> {course.CourseName} ({course.CourseCode})</li>
                        <li><strong>Current Attendance:</strong> {attendancePercentage:F1}%</li>
                        <li><strong>Required Minimum:</strong> 75%</li>
                    </ul>
                    <p>Please note that maintaining adequate attendance is crucial for your academic success.
                    If your attendance continues to be below the required threshold, it may affect your grade
                    and eligibility to continue in this course.</p>
                    <p>If you are experiencing difficulties attending class, please contact your advisor
                    or the instructor as soon as possible.</p>
                    <p>Best regards,<br/>School Management System</p>
                </body>
                </html>";

            var notification = new EmailNotificationDto
            {
                ToEmail = student.User.Email,
                ToName = student.User.FullName,
                Subject = $"Attendance Warning - {course.CourseCode}",
                Body = emailBody,
                IsHtml = true
            };

            return await SendEmailAsync(notification, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending attendance warning");
            return false;
        }
    }
}
