using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Core.Entities;

namespace SchoolManagementSystem.Infrastructure.Data;

/// <summary>
/// Main database context for the School Management System
/// </summary>
public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<Admin> Admins => Set<Admin>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<Grade> Grades => Set<Grade>();
    public DbSet<Assignment> Assignments => Set<Assignment>();
    public DbSet<AssignmentSubmission> AssignmentSubmissions => Set<AssignmentSubmission>();
    public DbSet<Attendance> Attendances => Set<Attendance>();
    public DbSet<Schedule> Schedules => Set<Schedule>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Semester> Semesters => Set<Semester>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from the assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Configure Identity tables
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<IdentityRole<int>>().ToTable("Roles");
        modelBuilder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
        modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
        modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
        modelBuilder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");
        modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");

        // Student Configuration
        modelBuilder.Entity<Student>(entity =>
        {
            entity.ToTable("Students");
            entity.HasIndex(e => e.StudentNumber).IsUnique();
            entity.Property(e => e.StudentNumber).HasMaxLength(20).IsRequired();
            entity.Property(e => e.GPA).HasPrecision(3, 2);
            entity.Property(e => e.Status).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Major).HasMaxLength(100);
            entity.Property(e => e.Minor).HasMaxLength(100);

            entity.HasOne(e => e.User)
                .WithOne(u => u.Student)
                .HasForeignKey<Student>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Advisor)
                .WithMany(t => t.Advisees)
                .HasForeignKey(e => e.AdvisorId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Teacher Configuration
        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.ToTable("Teachers");
            entity.HasIndex(e => e.EmployeeNumber).IsUnique();
            entity.Property(e => e.EmployeeNumber).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Specialization).HasMaxLength(200);
            entity.Property(e => e.Salary).HasPrecision(18, 2);
            entity.Property(e => e.EmploymentType).HasMaxLength(50).IsRequired();

            entity.HasOne(e => e.User)
                .WithOne(u => u.Teacher)
                .HasForeignKey<Teacher>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Department)
                .WithMany(d => d.Teachers)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Admin Configuration
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.ToTable("Admins");
            entity.HasIndex(e => e.EmployeeNumber).IsUnique();
            entity.Property(e => e.EmployeeNumber).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Role).HasMaxLength(50).IsRequired();

            entity.HasOne(e => e.User)
                .WithOne(u => u.Admin)
                .HasForeignKey<Admin>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Course Configuration
        modelBuilder.Entity<Course>(entity =>
        {
            entity.ToTable("Courses");
            entity.HasIndex(e => e.CourseCode).IsUnique();
            entity.Property(e => e.CourseCode).HasMaxLength(20).IsRequired();
            entity.Property(e => e.CourseName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Level).HasMaxLength(50).IsRequired();
            entity.Property(e => e.CourseFee).HasPrecision(18, 2);

            entity.HasOne(e => e.Teacher)
                .WithMany(t => t.Courses)
                .HasForeignKey(e => e.TeacherId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Department)
                .WithMany(d => d.Courses)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Semester)
                .WithMany(s => s.Courses)
                .HasForeignKey(e => e.SemesterId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Enrollment Configuration
        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.ToTable("Enrollments");
            entity.HasIndex(e => new { e.StudentId, e.CourseId, e.SemesterId });
            entity.Property(e => e.Status).HasMaxLength(50).IsRequired();
            entity.Property(e => e.FinalGrade).HasPrecision(5, 2);
            entity.Property(e => e.LetterGrade).HasMaxLength(5);

            entity.HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Semester)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.SemesterId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Grade Configuration
        modelBuilder.Entity<Grade>(entity =>
        {
            entity.ToTable("Grades");
            entity.HasIndex(e => new { e.StudentId, e.CourseId, e.GradeType });
            entity.Property(e => e.Value).HasPrecision(5, 2).IsRequired();
            entity.Property(e => e.MaxValue).HasPrecision(5, 2).IsRequired();
            entity.Property(e => e.Weight).HasPrecision(5, 2).IsRequired();
            entity.Property(e => e.LetterGrade).HasMaxLength(5);
            entity.Property(e => e.GradeType).HasMaxLength(50).IsRequired();

            entity.HasOne(e => e.Student)
                .WithMany(s => s.Grades)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Course)
                .WithMany(c => c.Grades)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Enrollment)
                .WithMany(en => en.Grades)
                .HasForeignKey(e => e.EnrollmentId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Assignment)
                .WithMany(a => a.Grades)
                .HasForeignKey(e => e.AssignmentId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Assignment Configuration
        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.ToTable("Assignments");
            entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
            entity.Property(e => e.MaxScore).HasPrecision(5, 2).IsRequired();
            entity.Property(e => e.Weight).HasPrecision(5, 2).IsRequired();
            entity.Property(e => e.Type).HasMaxLength(50).IsRequired();
            entity.Property(e => e.LatePenaltyPercentage).HasPrecision(5, 2);

            entity.HasOne(e => e.Course)
                .WithMany(c => c.Assignments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Teacher)
                .WithMany(t => t.Assignments)
                .HasForeignKey(e => e.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Assignment Submission Configuration
        modelBuilder.Entity<AssignmentSubmission>(entity =>
        {
            entity.ToTable("AssignmentSubmissions");
            entity.HasIndex(e => new { e.AssignmentId, e.StudentId }).IsUnique();
            entity.Property(e => e.Status).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Score).HasPrecision(5, 2);

            entity.HasOne(e => e.Assignment)
                .WithMany(a => a.Submissions)
                .HasForeignKey(e => e.AssignmentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Student)
                .WithMany(s => s.AssignmentSubmissions)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Attendance Configuration
        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.ToTable("Attendances");
            entity.HasIndex(e => new { e.StudentId, e.CourseId, e.Date });
            entity.Property(e => e.Status).HasMaxLength(50).IsRequired();

            entity.HasOne(e => e.Student)
                .WithMany(s => s.Attendances)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Course)
                .WithMany(c => c.Attendances)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Schedule)
                .WithMany(sch => sch.Attendances)
                .HasForeignKey(e => e.ScheduleId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Schedule Configuration
        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.ToTable("Schedules");
            entity.Property(e => e.DayOfWeek).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Type).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Room).HasMaxLength(50);
            entity.Property(e => e.Building).HasMaxLength(100);

            entity.HasOne(e => e.Course)
                .WithMany(c => c.Schedules)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Teacher)
                .WithMany(t => t.Schedules)
                .HasForeignKey(e => e.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Semester)
                .WithMany(s => s.Schedules)
                .HasForeignKey(e => e.SemesterId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Department Configuration
        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("Departments");
            entity.HasIndex(e => e.DepartmentCode).IsUnique();
            entity.Property(e => e.DepartmentCode).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(100);

            entity.HasOne(e => e.Head)
                .WithMany()
                .HasForeignKey(e => e.HeadId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Semester Configuration
        modelBuilder.Entity<Semester>(entity =>
        {
            entity.ToTable("Semesters");
            entity.HasIndex(e => e.Code).IsUnique();
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Code).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Term).HasMaxLength(50).IsRequired();

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Notification Configuration
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.ToTable("Notifications");
            entity.HasIndex(e => new { e.UserId, e.IsRead, e.CreatedAt });
            entity.Property(e => e.Type).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Category).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Priority).HasMaxLength(50).IsRequired();

            entity.HasOne(e => e.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Document Configuration
        modelBuilder.Entity<Document>(entity =>
        {
            entity.ToTable("Documents");
            entity.Property(e => e.FileName).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Category).HasMaxLength(50).IsRequired();

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // AuditLog Configuration
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.ToTable("AuditLogs");
            entity.HasIndex(e => new { e.EntityType, e.EntityId, e.Timestamp });
            entity.Property(e => e.Action).HasMaxLength(50).IsRequired();
            entity.Property(e => e.EntityType).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Severity).HasMaxLength(50).IsRequired();

            entity.HasOne(e => e.User)
                .WithMany(u => u.AuditLogs)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // SystemSetting Configuration
        modelBuilder.Entity<SystemSetting>(entity =>
        {
            entity.ToTable("SystemSettings");
            entity.HasIndex(e => e.Key).IsUnique();
            entity.Property(e => e.Key).HasMaxLength(100).IsRequired();
            entity.Property(e => e.DataType).HasMaxLength(50).IsRequired();

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Seed initial data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Roles
        modelBuilder.Entity<IdentityRole<int>>().HasData(
            new IdentityRole<int> { Id = 1, Name = "SuperAdmin", NormalizedName = "SUPERADMIN" },
            new IdentityRole<int> { Id = 2, Name = "Admin", NormalizedName = "ADMIN" },
            new IdentityRole<int> { Id = 3, Name = "Teacher", NormalizedName = "TEACHER" },
            new IdentityRole<int> { Id = 4, Name = "Student", NormalizedName = "STUDENT" }
        );

        // Seed System Settings
        modelBuilder.Entity<SystemSetting>().HasData(
            new SystemSetting
            {
                Id = 1,
                Key = "MaxStudentsPerCourse",
                Value = "30",
                Category = "Course",
                Description = "Maximum number of students allowed per course",
                DataType = "Int",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow
            },
            new SystemSetting
            {
                Id = 2,
                Key = "MinAttendancePercentage",
                Value = "75",
                Category = "Attendance",
                Description = "Minimum attendance percentage required",
                DataType = "Int",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow
            },
            new SystemSetting
            {
                Id = 3,
                Key = "GradePassingScore",
                Value = "60",
                Category = "Grade",
                Description = "Minimum score to pass a course",
                DataType = "Int",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow
            }
        );
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }
    }
}
