namespace SchoolManagementSystem.Core.DTOs;

public class EmailNotificationDto
{
    public string ToEmail { get; set; } = string.Empty;
    public string ToName { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsHtml { get; set; } = true;
    public List<EmailAttachment>? Attachments { get; set; }
    public string? CcEmails { get; set; }
    public string? BccEmails { get; set; }
    public Dictionary<string, string>? TemplateData { get; set; }
}

public class BulkEmailNotificationDto
{
    public List<string> ToEmails { get; set; } = new();
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsHtml { get; set; } = true;
    public string? TemplateName { get; set; }
    public Dictionary<string, string>? TemplateData { get; set; }
}

public class SmsNotificationDto
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? SenderName { get; set; }
}

public class EmailAttachment
{
    public string FileName { get; set; } = string.Empty;
    public byte[] Content { get; set; } = Array.Empty<byte>();
    public string ContentType { get; set; } = "application/octet-stream";
}

public class NotificationTemplateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // Email, SMS
    public Dictionary<string, string> PlaceholderDescriptions { get; set; } = new();
}

public class NotificationHistoryDto
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty; // Email, SMS
    public string Recipient { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // Sent, Failed, Pending
    public DateTime SentAt { get; set; }
    public string? ErrorMessage { get; set; }
}
