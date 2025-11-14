using SchoolManagementSystem.Application.Interfaces;
using System.Security.Claims;

namespace SchoolManagementSystem.API.Middleware;

public class AuditLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuditLoggingMiddleware> _logger;

    public AuditLoggingMiddleware(
        RequestDelegate next,
        ILogger<AuditLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IAuditService auditService)
    {
        await _next(context);

        // Only audit successful state-changing operations
        if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
        {
            var method = context.Request.Method.ToUpper();

            // Audit POST, PUT, DELETE operations
            if (method == "POST" || method == "PUT" || method == "DELETE")
            {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Anonymous";
                var endpoint = context.Request.Path.Value ?? "";
                var action = GetActionFromMethod(method);

                // Extract entity information from route
                var (entityType, entityId) = ExtractEntityInfo(endpoint);

                if (!string.IsNullOrEmpty(entityType))
                {
                    await auditService.LogActivityAsync(
                        entityType,
                        entityId,
                        action,
                        userId,
                        $"{method} {endpoint}"
                    );
                }
            }
        }
    }

    private string GetActionFromMethod(string method)
    {
        return method switch
        {
            "POST" => "Created",
            "PUT" => "Updated",
            "DELETE" => "Deleted",
            _ => "Modified"
        };
    }

    private (string EntityType, int EntityId) ExtractEntityInfo(string path)
    {
        // Parse route to extract entity type and ID
        // Example: /api/v1/students/123 -> (Students, 123)
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (segments.Length >= 3)
        {
            var entityType = segments[2]; // After "api/v1"

            // Try to get entity ID
            if (segments.Length >= 4 && int.TryParse(segments[3], out int entityId))
            {
                return (CapitalizeFirst(entityType), entityId);
            }

            return (CapitalizeFirst(entityType), 0);
        }

        return ("Unknown", 0);
    }

    private string CapitalizeFirst(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return char.ToUpper(input[0]) + input.Substring(1);
    }
}
