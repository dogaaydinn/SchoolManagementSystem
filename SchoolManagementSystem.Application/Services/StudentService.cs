using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Core.DTOs;
using SchoolManagementSystem.Core.Entities;
using SchoolManagementSystem.Core.Interfaces;

namespace SchoolManagementSystem.Application.Services;

/// <summary>
/// Service implementation for student management operations
/// </summary>
public class StudentService : IStudentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<StudentService> _logger;

    public StudentService(IUnitOfWork unitOfWork, ILogger<StudentService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<PagedResult<StudentListDto>> GetAllStudentsAsync(
        int pageNumber = 1,
        int pageSize = 10,
        string? searchTerm = null,
        string? status = null)
    {
        try
        {
            var query = _unitOfWork.Students.GetAllQueryable()
                .Include(s => s.User)
                .Where(s => !s.IsDeleted);

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(s =>
                    s.User!.FirstName.Contains(searchTerm) ||
                    s.User.LastName.Contains(searchTerm) ||
                    s.User.Email!.Contains(searchTerm) ||
                    s.StudentNumber.Contains(searchTerm));
            }

            // Apply status filter
            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(s => s.Status == status);
            }

            var totalCount = await query.CountAsync();

            var students = await query
                .OrderBy(s => s.StudentNumber)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new StudentListDto
                {
                    Id = s.Id,
                    StudentNumber = s.StudentNumber,
                    FullName = s.User!.FullName,
                    Email = s.User.Email!,
                    GPA = s.GPA,
                    CurrentSemester = s.CurrentSemester,
                    Status = s.Status,
                    Major = s.Major
                })
                .ToListAsync();

            return new PagedResult<StudentListDto>
            {
                Items = students,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving students list");
            throw;
        }
    }

    public async Task<StudentDetailDto?> GetStudentByIdAsync(int id)
    {
        try
        {
            var student = await _unitOfWork.Students.GetByIdWithIncludesAsync(
                id,
                s => s.User!,
                s => s.Enrollments!,
                s => s.Grades!);

            if (student == null || student.IsDeleted)
            {
                return null;
            }

            return MapToDetailDto(student);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving student with ID {StudentId}", id);
            throw;
        }
    }

    public async Task<StudentDetailDto?> GetStudentByNumberAsync(string studentNumber)
    {
        try
        {
            var students = await _unitOfWork.Students.FindAsync(s => s.StudentNumber == studentNumber && !s.IsDeleted);
            var student = students.FirstOrDefault();

            if (student == null)
            {
                return null;
            }

            // Load related data
            await _unitOfWork.Context.Entry(student)
                .Reference(s => s.User)
                .LoadAsync();

            await _unitOfWork.Context.Entry(student)
                .Collection(s => s.Enrollments!)
                .LoadAsync();

            await _unitOfWork.Context.Entry(student)
                .Collection(s => s.Grades!)
                .LoadAsync();

            return MapToDetailDto(student);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving student with number {StudentNumber}", studentNumber);
            throw;
        }
    }

    public async Task<StudentDto> CreateStudentAsync(CreateStudentRequestDto request)
    {
        try
        {
            // Check if email already exists
            var existingUser = await _unitOfWork.Users.FindAsync(u => u.Email == request.Email);
            if (existingUser.Any())
            {
                throw new InvalidOperationException("A user with this email already exists");
            }

            // Create user first
            var user = new User
            {
                Email = request.Email,
                UserName = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth,
                IsActive = true,
                EmailConfirmed = false,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // Generate student number
            var studentNumber = await GenerateStudentNumberAsync();

            // Create student
            var student = new Student
            {
                UserId = user.Id,
                StudentNumber = studentNumber,
                EnrollmentDate = DateTime.UtcNow,
                Status = "Active",
                Major = request.Major,
                Minor = request.Minor,
                GPA = 0.0m,
                CurrentSemester = 1,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Students.AddAsync(student);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Created student {StudentNumber} for user {Email}", studentNumber, request.Email);

            return MapToDto(student, user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating student for email {Email}", request.Email);
            throw;
        }
    }

    public async Task<StudentDto?> UpdateStudentAsync(int id, UpdateStudentRequestDto request)
    {
        try
        {
            var student = await _unitOfWork.Students.GetByIdWithIncludesAsync(id, s => s.User!);

            if (student == null || student.IsDeleted)
            {
                return null;
            }

            // Update user fields
            if (request.FirstName != null)
                student.User!.FirstName = request.FirstName;

            if (request.LastName != null)
                student.User!.LastName = request.LastName;

            if (request.DateOfBirth.HasValue)
                student.User!.DateOfBirth = request.DateOfBirth;

            // Update student fields
            if (request.Status != null)
                student.Status = request.Status;

            if (request.Major != null)
                student.Major = request.Major;

            if (request.Minor != null)
                student.Minor = request.Minor;

            student.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Students.Update(student);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Updated student {StudentId}", id);

            return MapToDto(student, student.User!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating student {StudentId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteStudentAsync(int id)
    {
        try
        {
            var student = await _unitOfWork.Students.GetByIdAsync(id);

            if (student == null || student.IsDeleted)
            {
                return false;
            }

            // Soft delete
            student.IsDeleted = true;
            student.DeletedAt = DateTime.UtcNow;

            _unitOfWork.Students.Update(student);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Deleted student {StudentId}", id);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting student {StudentId}", id);
            throw;
        }
    }

    public async Task<StudentTranscriptDto?> GetStudentTranscriptAsync(int id)
    {
        try
        {
            var student = await _unitOfWork.Students.GetByIdWithIncludesAsync(
                id,
                s => s.User!,
                s => s.Grades!);

            if (student == null || student.IsDeleted)
            {
                return null;
            }

            // Load courses for each grade
            foreach (var grade in student.Grades ?? Enumerable.Empty<Grade>())
            {
                await _unitOfWork.Context.Entry(grade)
                    .Reference(g => g.Course)
                    .LoadAsync();
            }

            var transcript = new StudentTranscriptDto
            {
                Student = MapToDto(student, student.User!),
                SemesterGrades = new List<SemesterGradesDto>(),
                OverallGPA = student.GPA,
                TotalCreditsEarned = student.TotalCreditsEarned,
                GeneratedAt = DateTime.UtcNow
            };

            // Group grades by semester
            var gradesBySemester = student.Grades?
                .Where(g => g.Course != null)
                .GroupBy(g => new { g.Course!.Semester })
                .ToList() ?? new List<IGrouping<dynamic, Grade>>();

            foreach (var semesterGroup in gradesBySemester)
            {
                var semesterGrades = new SemesterGradesDto
                {
                    SemesterName = semesterGroup.Key.Semester ?? "Unknown",
                    Grades = semesterGroup.Select(g => new GradeDto
                    {
                        Id = g.Id,
                        StudentId = g.StudentId,
                        CourseId = g.CourseId,
                        CourseCode = g.Course!.CourseCode,
                        CourseName = g.Course.Name,
                        Value = g.Value,
                        GradeLetterValue = g.GradeLetterValue,
                        AssignedDate = g.AssignedDate,
                        Comments = g.Comments
                    }).ToList(),
                    SemesterGPA = 0, // Calculate based on semester grades
                    CreditsEarned = semesterGroup.Sum(g => g.Course!.Credits)
                };

                transcript.SemesterGrades.Add(semesterGrades);
            }

            return transcript;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transcript for student {StudentId}", id);
            throw;
        }
    }

    public async Task<bool> EnrollInCourseAsync(int studentId, int courseId)
    {
        try
        {
            // Check if student exists
            var student = await _unitOfWork.Students.GetByIdAsync(studentId);
            if (student == null || student.IsDeleted)
            {
                return false;
            }

            // Check if course exists
            var course = await _unitOfWork.Courses.GetByIdAsync(courseId);
            if (course == null || course.IsDeleted)
            {
                return false;
            }

            // Check if already enrolled
            var existingEnrollments = await _unitOfWork.Enrollments.FindAsync(
                e => e.StudentId == studentId && e.CourseId == courseId);

            if (existingEnrollments.Any())
            {
                return false;
            }

            // Create enrollment
            var enrollment = new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId,
                EnrollmentDate = DateTime.UtcNow,
                Status = "Enrolled",
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Enrollments.AddAsync(enrollment);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Student {StudentId} enrolled in course {CourseId}", studentId, courseId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enrolling student {StudentId} in course {CourseId}", studentId, courseId);
            throw;
        }
    }

    public async Task<bool> UnenrollFromCourseAsync(int studentId, int courseId)
    {
        try
        {
            var enrollments = await _unitOfWork.Enrollments.FindAsync(
                e => e.StudentId == studentId && e.CourseId == courseId);

            var enrollment = enrollments.FirstOrDefault();
            if (enrollment == null)
            {
                return false;
            }

            _unitOfWork.Enrollments.Remove(enrollment);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Student {StudentId} unenrolled from course {CourseId}", studentId, courseId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unenrolling student {StudentId} from course {CourseId}", studentId, courseId);
            throw;
        }
    }

    public async Task<List<CourseDto>> GetEnrolledCoursesAsync(int studentId)
    {
        try
        {
            var enrollments = await _unitOfWork.Enrollments.FindAsync(e => e.StudentId == studentId);

            var courses = new List<CourseDto>();

            foreach (var enrollment in enrollments)
            {
                await _unitOfWork.Context.Entry(enrollment)
                    .Reference(e => e.Course)
                    .LoadAsync();

                if (enrollment.Course != null && !enrollment.Course.IsDeleted)
                {
                    courses.Add(new CourseDto
                    {
                        Id = enrollment.Course.Id,
                        CourseCode = enrollment.Course.CourseCode,
                        Name = enrollment.Course.Name,
                        Description = enrollment.Course.Description,
                        Credits = enrollment.Course.Credits,
                        Department = enrollment.Course.Department,
                        Semester = enrollment.Course.Semester
                    });
                }
            }

            return courses;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving enrolled courses for student {StudentId}", studentId);
            throw;
        }
    }

    public async Task<decimal> CalculateGPAAsync(int studentId)
    {
        try
        {
            var grades = await _unitOfWork.Grades.FindAsync(g => g.StudentId == studentId);

            if (!grades.Any())
            {
                return 0.0m;
            }

            // Load courses for credit hours
            foreach (var grade in grades)
            {
                await _unitOfWork.Context.Entry(grade)
                    .Reference(g => g.Course)
                    .LoadAsync();
            }

            decimal totalPoints = 0;
            int totalCredits = 0;

            foreach (var grade in grades.Where(g => g.Course != null && g.Value.HasValue))
            {
                totalPoints += grade.Value!.Value * grade.Course!.Credits;
                totalCredits += grade.Course.Credits;
            }

            var gpa = totalCredits > 0 ? totalPoints / totalCredits : 0.0m;

            // Update student GPA
            var student = await _unitOfWork.Students.GetByIdAsync(studentId);
            if (student != null)
            {
                student.GPA = gpa;
                student.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Students.Update(student);
                await _unitOfWork.SaveChangesAsync();
            }

            return gpa;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating GPA for student {StudentId}", studentId);
            throw;
        }
    }

    #region Private Helper Methods

    private async Task<string> GenerateStudentNumberAsync()
    {
        var year = DateTime.UtcNow.Year;
        var random = System.Security.Cryptography.RandomNumberGenerator.GetInt32(1000, 10000);
        var studentNumber = $"STU{year}{random}";

        // Ensure uniqueness
        var existing = await _unitOfWork.Students.FindAsync(s => s.StudentNumber == studentNumber);
        if (existing.Any())
        {
            // Recursively try again
            return await GenerateStudentNumberAsync();
        }

        return studentNumber;
    }

    private StudentDto MapToDto(Student student, User user)
    {
        return new StudentDto
        {
            Id = student.Id,
            UserId = student.UserId,
            StudentNumber = student.StudentNumber,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = user.FullName,
            Email = user.Email!,
            DateOfBirth = user.DateOfBirth,
            Age = user.Age,
            EnrollmentDate = student.EnrollmentDate,
            GPA = student.GPA,
            CurrentSemester = student.CurrentSemester,
            Major = student.Major,
            Minor = student.Minor,
            Status = student.Status,
            TotalCreditsEarned = student.TotalCreditsEarned,
            TotalCreditsRequired = 120, // Default - should come from program requirements
            ProfilePictureUrl = user.ProfilePictureUrl
        };
    }

    private StudentDetailDto MapToDetailDto(Student student)
    {
        var dto = new StudentDetailDto
        {
            Id = student.Id,
            UserId = student.UserId,
            StudentNumber = student.StudentNumber,
            FirstName = student.User!.FirstName,
            LastName = student.User.LastName,
            FullName = student.User.FullName,
            Email = student.User.Email!,
            DateOfBirth = student.User.DateOfBirth,
            Age = student.User.Age,
            EnrollmentDate = student.EnrollmentDate,
            GPA = student.GPA,
            CurrentSemester = student.CurrentSemester,
            Major = student.Major,
            Minor = student.Minor,
            Status = student.Status,
            TotalCreditsEarned = student.TotalCreditsEarned,
            TotalCreditsRequired = 120,
            ProfilePictureUrl = student.User.ProfilePictureUrl,
            EnrolledCourses = new List<CourseDto>(),
            Grades = new List<GradeDto>()
        };

        // Map enrolled courses
        if (student.Enrollments != null)
        {
            foreach (var enrollment in student.Enrollments.Where(e => e.Course != null))
            {
                dto.EnrolledCourses.Add(new CourseDto
                {
                    Id = enrollment.Course!.Id,
                    CourseCode = enrollment.Course.CourseCode,
                    Name = enrollment.Course.Name,
                    Description = enrollment.Course.Description,
                    Credits = enrollment.Course.Credits,
                    Department = enrollment.Course.Department,
                    Semester = enrollment.Course.Semester
                });
            }
        }

        // Map grades
        if (student.Grades != null)
        {
            foreach (var grade in student.Grades.Where(g => g.Course != null))
            {
                dto.Grades.Add(new GradeDto
                {
                    Id = grade.Id,
                    StudentId = grade.StudentId,
                    CourseId = grade.CourseId,
                    CourseCode = grade.Course!.CourseCode,
                    CourseName = grade.Course.Name,
                    Value = grade.Value,
                    GradeLetterValue = grade.GradeLetterValue,
                    AssignedDate = grade.AssignedDate,
                    Comments = grade.Comments
                });
            }
        }

        return dto;
    }

    #endregion
}
