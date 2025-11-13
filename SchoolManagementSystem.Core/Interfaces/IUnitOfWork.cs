namespace SchoolManagementSystem.Core.Interfaces;

/// <summary>
/// Unit of Work interface for managing transactions
/// </summary>
public interface IUnitOfWork : IDisposable
{
    // Repositories
    IRepository<Entities.User> Users { get; }
    IRepository<Entities.Student> Students { get; }
    IRepository<Entities.Teacher> Teachers { get; }
    IRepository<Entities.Admin> Admins { get; }
    IRepository<Entities.Course> Courses { get; }
    IRepository<Entities.Enrollment> Enrollments { get; }
    IRepository<Entities.Grade> Grades { get; }
    IRepository<Entities.Assignment> Assignments { get; }
    IRepository<Entities.AssignmentSubmission> AssignmentSubmissions { get; }
    IRepository<Entities.Attendance> Attendances { get; }
    IRepository<Entities.Schedule> Schedules { get; }
    IRepository<Entities.Department> Departments { get; }
    IRepository<Entities.Semester> Semesters { get; }
    IRepository<Entities.Notification> Notifications { get; }
    IRepository<Entities.Document> Documents { get; }
    IRepository<Entities.AuditLog> AuditLogs { get; }
    IRepository<Entities.SystemSetting> SystemSettings { get; }

    // Transaction management
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
