using Microsoft.EntityFrameworkCore.Storage;
using SchoolManagementSystem.Core.Entities;
using SchoolManagementSystem.Core.Interfaces;
using SchoolManagementSystem.Infrastructure.Data;

namespace SchoolManagementSystem.Infrastructure.Repositories;

/// <summary>
/// Unit of Work implementation for managing transactions
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    // Repository instances
    private IRepository<User>? _users;
    private IRepository<Student>? _students;
    private IRepository<Teacher>? _teachers;
    private IRepository<Admin>? _admins;
    private IRepository<Course>? _courses;
    private IRepository<Enrollment>? _enrollments;
    private IRepository<Grade>? _grades;
    private IRepository<Assignment>? _assignments;
    private IRepository<AssignmentSubmission>? _assignmentSubmissions;
    private IRepository<Attendance>? _attendances;
    private IRepository<Schedule>? _schedules;
    private IRepository<Department>? _departments;
    private IRepository<Semester>? _semesters;
    private IRepository<Notification>? _notifications;
    private IRepository<Document>? _documents;
    private IRepository<AuditLog>? _auditLogs;
    private IRepository<SystemSetting>? _systemSettings;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    // Repository properties with lazy initialization
    public IRepository<User> Users => _users ??= new Repository<User>(_context);
    public IRepository<Student> Students => _students ??= new Repository<Student>(_context);
    public IRepository<Teacher> Teachers => _teachers ??= new Repository<Teacher>(_context);
    public IRepository<Admin> Admins => _admins ??= new Repository<Admin>(_context);
    public IRepository<Course> Courses => _courses ??= new Repository<Course>(_context);
    public IRepository<Enrollment> Enrollments => _enrollments ??= new Repository<Enrollment>(_context);
    public IRepository<Grade> Grades => _grades ??= new Repository<Grade>(_context);
    public IRepository<Assignment> Assignments => _assignments ??= new Repository<Assignment>(_context);
    public IRepository<AssignmentSubmission> AssignmentSubmissions => _assignmentSubmissions ??= new Repository<AssignmentSubmission>(_context);
    public IRepository<Attendance> Attendances => _attendances ??= new Repository<Attendance>(_context);
    public IRepository<Schedule> Schedules => _schedules ??= new Repository<Schedule>(_context);
    public IRepository<Department> Departments => _departments ??= new Repository<Department>(_context);
    public IRepository<Semester> Semesters => _semesters ??= new Repository<Semester>(_context);
    public IRepository<Notification> Notifications => _notifications ??= new Repository<Notification>(_context);
    public IRepository<Document> Documents => _documents ??= new Repository<Document>(_context);
    public IRepository<AuditLog> AuditLogs => _auditLogs ??= new Repository<AuditLog>(_context);
    public IRepository<SystemSetting> SystemSettings => _systemSettings ??= new Repository<SystemSetting>(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
            }
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
