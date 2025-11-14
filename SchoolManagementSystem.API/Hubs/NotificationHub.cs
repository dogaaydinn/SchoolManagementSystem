using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace SchoolManagementSystem.API.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly ILogger<NotificationHub> _logger;

        public NotificationHub(ILogger<NotificationHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                // Add user to their personal group
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");

                // Add user to role-based groups
                var roles = Context.User?.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
                if (roles != null)
                {
                    foreach (var role in roles)
                    {
                        await Groups.AddToGroupAsync(Context.ConnectionId, $"role_{role}");
                    }
                }

                _logger.LogInformation("User {UserName} (ID: {UserId}) connected to NotificationHub. ConnectionId: {ConnectionId}",
                    userName, userId, Context.ConnectionId);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value;

            if (exception != null)
            {
                _logger.LogError(exception, "User {UserName} (ID: {UserId}) disconnected from NotificationHub with error",
                    userName, userId);
            }
            else
            {
                _logger.LogInformation("User {UserName} (ID: {UserId}) disconnected from NotificationHub",
                    userName, userId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        // Client can call this to mark a notification as read
        public async Task MarkAsRead(int notificationId)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            _logger.LogInformation("User {UserId} marked notification {NotificationId} as read",
                userId, notificationId);

            // Acknowledge to the caller
            await Clients.Caller.SendAsync("NotificationMarkedAsRead", notificationId);
        }

        // Client can call this to join a specific course or class room
        public async Task JoinCourse(int courseId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"course_{courseId}");

            _logger.LogInformation("User joined course {CourseId} notifications. ConnectionId: {ConnectionId}",
                courseId, Context.ConnectionId);
        }

        // Client can call this to leave a specific course
        public async Task LeaveCourse(int courseId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"course_{courseId}");

            _logger.LogInformation("User left course {CourseId} notifications. ConnectionId: {ConnectionId}",
                courseId, Context.ConnectionId);
        }

        // Typing indicator for chat/messaging (future feature)
        public async Task SendTypingIndicator(string recipientUserId)
        {
            var senderUserId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var senderName = Context.User?.FindFirst(ClaimTypes.Name)?.Value;

            await Clients.Group($"user_{recipientUserId}").SendAsync("UserTyping", new
            {
                UserId = senderUserId,
                UserName = senderName,
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
