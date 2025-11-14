namespace SchoolManagementSystem.Application.Interfaces
{
    public interface IRealtimeNotificationService
    {
        // Send notification to a specific user
        Task SendToUserAsync(string userId, string type, object data, CancellationToken cancellationToken = default);

        // Send notification to multiple users
        Task SendToUsersAsync(IEnumerable<string> userIds, string type, object data, CancellationToken cancellationToken = default);

        // Send notification to all users with a specific role
        Task SendToRoleAsync(string role, string type, object data, CancellationToken cancellationToken = default);

        // Send notification to all users in a course
        Task SendToCourseAsync(int courseId, string type, object data, CancellationToken cancellationToken = default);

        // Send notification to all connected users
        Task SendToAllAsync(string type, object data, CancellationToken cancellationToken = default);

        // Grade posted notification
        Task NotifyGradePostedAsync(int studentId, string studentName, string courseName, string letterGrade, CancellationToken cancellationToken = default);

        // Enrollment notification
        Task NotifyEnrollmentAsync(int studentId, string studentName, string courseName, bool isEnrolled, CancellationToken cancellationToken = default);

        // Assignment notification
        Task NotifyAssignmentAsync(int courseId, string courseName, string assignmentTitle, DateTime dueDate, CancellationToken cancellationToken = default);

        // System notification (maintenance, announcements, etc.)
        Task NotifySystemMessageAsync(string message, string severity, CancellationToken cancellationToken = default);

        // Attendance notification
        Task NotifyAttendanceAsync(int studentId, string studentName, string courseName, string status, CancellationToken cancellationToken = default);
    }
}
