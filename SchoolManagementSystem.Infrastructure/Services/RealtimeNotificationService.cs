using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Application.Interfaces;

namespace SchoolManagementSystem.Infrastructure.Services
{
    public class RealtimeNotificationService : IRealtimeNotificationService
    {
        private readonly IHubContext<dynamic> _hubContext;
        private readonly ILogger<RealtimeNotificationService> _logger;

        public RealtimeNotificationService(
            IHubContext<dynamic> hubContext,
            ILogger<RealtimeNotificationService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task SendToUserAsync(string userId, string type, object data, CancellationToken cancellationToken = default)
        {
            try
            {
                var notification = CreateNotification(type, data);
                await _hubContext.Clients.Group($"user_{userId}").SendAsync("ReceiveNotification", notification, cancellationToken);

                _logger.LogInformation("Sent {Type} notification to user {UserId}", type, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification to user {UserId}", userId);
            }
        }

        public async Task SendToUsersAsync(IEnumerable<string> userIds, string type, object data, CancellationToken cancellationToken = default)
        {
            try
            {
                var notification = CreateNotification(type, data);
                var groups = userIds.Select(id => $"user_{id}").ToList();

                foreach (var group in groups)
                {
                    await _hubContext.Clients.Group(group).SendAsync("ReceiveNotification", notification, cancellationToken);
                }

                _logger.LogInformation("Sent {Type} notification to {Count} users", type, userIds.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification to multiple users");
            }
        }

        public async Task SendToRoleAsync(string role, string type, object data, CancellationToken cancellationToken = default)
        {
            try
            {
                var notification = CreateNotification(type, data);
                await _hubContext.Clients.Group($"role_{role}").SendAsync("ReceiveNotification", notification, cancellationToken);

                _logger.LogInformation("Sent {Type} notification to role {Role}", type, role);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification to role {Role}", role);
            }
        }

        public async Task SendToCourseAsync(int courseId, string type, object data, CancellationToken cancellationToken = default)
        {
            try
            {
                var notification = CreateNotification(type, data);
                await _hubContext.Clients.Group($"course_{courseId}").SendAsync("ReceiveNotification", notification, cancellationToken);

                _logger.LogInformation("Sent {Type} notification to course {CourseId}", type, courseId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification to course {CourseId}", courseId);
            }
        }

        public async Task SendToAllAsync(string type, object data, CancellationToken cancellationToken = default)
        {
            try
            {
                var notification = CreateNotification(type, data);
                await _hubContext.Clients.All.SendAsync("ReceiveNotification", notification, cancellationToken);

                _logger.LogInformation("Sent {Type} notification to all users", type);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification to all users");
            }
        }

        public async Task NotifyGradePostedAsync(int studentId, string studentName, string courseName, string letterGrade, CancellationToken cancellationToken = default)
        {
            var data = new
            {
                StudentId = studentId,
                StudentName = studentName,
                CourseName = courseName,
                LetterGrade = letterGrade,
                Message = $"New grade posted for {courseName}: {letterGrade}",
                Timestamp = DateTime.UtcNow
            };

            await SendToUserAsync(studentId.ToString(), "GradePosted", data, cancellationToken);
        }

        public async Task NotifyEnrollmentAsync(int studentId, string studentName, string courseName, bool isEnrolled, CancellationToken cancellationToken = default)
        {
            var action = isEnrolled ? "enrolled in" : "dropped from";
            var data = new
            {
                StudentId = studentId,
                StudentName = studentName,
                CourseName = courseName,
                IsEnrolled = isEnrolled,
                Message = $"{studentName} {action} {courseName}",
                Timestamp = DateTime.UtcNow
            };

            await SendToUserAsync(studentId.ToString(), "Enrollment", data, cancellationToken);
        }

        public async Task NotifyAssignmentAsync(int courseId, string courseName, string assignmentTitle, DateTime dueDate, CancellationToken cancellationToken = default)
        {
            var data = new
            {
                CourseId = courseId,
                CourseName = courseName,
                AssignmentTitle = assignmentTitle,
                DueDate = dueDate,
                Message = $"New assignment in {courseName}: {assignmentTitle} (Due: {dueDate:MMM dd, yyyy})",
                Timestamp = DateTime.UtcNow
            };

            await SendToCourseAsync(courseId, "Assignment", data, cancellationToken);
        }

        public async Task NotifySystemMessageAsync(string message, string severity, CancellationToken cancellationToken = default)
        {
            var data = new
            {
                Message = message,
                Severity = severity, // Info, Warning, Error, Success
                Timestamp = DateTime.UtcNow
            };

            await SendToAllAsync("SystemMessage", data, cancellationToken);
        }

        public async Task NotifyAttendanceAsync(int studentId, string studentName, string courseName, string status, CancellationToken cancellationToken = default)
        {
            var data = new
            {
                StudentId = studentId,
                StudentName = studentName,
                CourseName = courseName,
                Status = status,
                Message = $"Attendance recorded for {courseName}: {status}",
                Timestamp = DateTime.UtcNow
            };

            await SendToUserAsync(studentId.ToString(), "Attendance", data, cancellationToken);
        }

        private object CreateNotification(string type, object data)
        {
            return new
            {
                Id = Guid.NewGuid(),
                Type = type,
                Data = data,
                Timestamp = DateTime.UtcNow
            };
        }
    }
}
