using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Core.DTOs;
using SchoolManagementSystem.Core.Entities;
using SchoolManagementSystem.Core.Interfaces;
using System.Text.Json;

namespace SchoolManagementSystem.Application.Services;

/// <summary>
/// Service implementation for course management operations
/// </summary>
public class CourseService : ICourseService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CourseService> _logger;

    public CourseService(IUnitOfWork unitOfWork, ILogger<CourseService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<PagedResult<CourseDto>> GetAllCoursesAsync(
        int pageNumber = 1,
        int pageSize = 10,
        string? searchTerm = null,
        int? departmentId = null,
        int? semesterId = null,
        bool? isActive = null)
    {
        try
        {
            var query = _unitOfWork.Courses.GetAllQueryable()
                .Include(c => c.Department)
                .Include(c => c.Teacher)
                    .ThenInclude(t => t!.User)
                .Include(c => c.Semester)
                .Where(c => !c.IsDeleted);

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(c =>
                    c.CourseCode.Contains(searchTerm) ||
                    c.CourseName.Contains(searchTerm) ||
                    (c.Description != null && c.Description.Contains(searchTerm)));
            }

            // Apply department filter
            if (departmentId.HasValue)
            {
                query = query.Where(c => c.DepartmentId == departmentId.Value);
            }

            // Apply semester filter
            if (semesterId.HasValue)
            {
                query = query.Where(c => c.SemesterId == semesterId.Value);
            }

            // Apply active status filter
            if (isActive.HasValue)
            {
                query = query.Where(c => c.IsActive == isActive.Value);
            }

            var totalCount = await query.CountAsync();

            var courses = await query
                .OrderBy(c => c.CourseCode)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CourseDto
                {
                    Id = c.Id,
                    CourseCode = c.CourseCode,
                    CourseName = c.CourseName,
                    Description = c.Description,
                    Credits = c.Credits,
                    DepartmentName = c.Department != null ? c.Department.Name : null,
                    TeacherName = c.Teacher != null ? c.Teacher.User!.FullName : null,
                    MaxStudents = c.MaxStudents,
                    CurrentEnrollment = c.CurrentEnrollment,
                    Level = c.Level,
                    IsActive = c.IsActive,
                    SemesterName = c.Semester != null ? c.Semester.Name : null
                })
                .ToListAsync();

            return new PagedResult<CourseDto>
            {
                Items = courses,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving courses list");
            throw;
        }
    }

    public async Task<CourseDetailDto?> GetCourseByIdAsync(int id)
    {
        try
        {
            var course = await _unitOfWork.Courses.GetByIdWithIncludesAsync(
                id,
                c => c.Department!,
                c => c.Teacher!,
                c => c.Semester!,
                c => c.Enrollments!,
                c => c.Assignments!,
                c => c.Schedules!);

            if (course == null || course.IsDeleted)
            {
                return null;
            }

            return await MapToDetailDtoAsync(course);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving course with ID {CourseId}", id);
            throw;
        }
    }

    public async Task<CourseDetailDto?> GetCourseByCourseCodeAsync(string courseCode)
    {
        try
        {
            var courses = await _unitOfWork.Courses.FindAsync(c => c.CourseCode == courseCode && !c.IsDeleted);
            var course = courses.FirstOrDefault();

            if (course == null)
            {
                return null;
            }

            // Load related data
            await _unitOfWork.Context.Entry(course)
                .Reference(c => c.Department)
                .LoadAsync();

            await _unitOfWork.Context.Entry(course)
                .Reference(c => c.Teacher)
                .LoadAsync();

            if (course.Teacher != null)
            {
                await _unitOfWork.Context.Entry(course.Teacher)
                    .Reference(t => t.User)
                    .LoadAsync();
            }

            await _unitOfWork.Context.Entry(course)
                .Reference(c => c.Semester)
                .LoadAsync();

            await _unitOfWork.Context.Entry(course)
                .Collection(c => c.Enrollments!)
                .LoadAsync();

            await _unitOfWork.Context.Entry(course)
                .Collection(c => c.Assignments!)
                .LoadAsync();

            await _unitOfWork.Context.Entry(course)
                .Collection(c => c.Schedules!)
                .LoadAsync();

            return await MapToDetailDtoAsync(course);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving course with code {CourseCode}", courseCode);
            throw;
        }
    }

    public async Task<CourseDto> CreateCourseAsync(CreateCourseRequestDto request)
    {
        try
        {
            // Check if course code already exists
            var existingCourse = await _unitOfWork.Courses.FindAsync(c => c.CourseCode == request.CourseCode);
            if (existingCourse.Any())
            {
                throw new InvalidOperationException("A course with this course code already exists");
            }

            // Validate teacher exists if provided
            if (request.TeacherId.HasValue)
            {
                var teacher = await _unitOfWork.Teachers.GetByIdAsync(request.TeacherId.Value);
                if (teacher == null || teacher.IsDeleted)
                {
                    throw new InvalidOperationException("Specified teacher does not exist");
                }
            }

            // Validate department exists if provided
            if (request.DepartmentId.HasValue)
            {
                var department = await _unitOfWork.Departments.GetByIdAsync(request.DepartmentId.Value);
                if (department == null || department.IsDeleted)
                {
                    throw new InvalidOperationException("Specified department does not exist");
                }
            }

            // Create course
            var course = new Course
            {
                CourseCode = request.CourseCode,
                CourseName = request.CourseName,
                Description = request.Description,
                Credits = request.Credits,
                DepartmentId = request.DepartmentId,
                TeacherId = request.TeacherId,
                MaxStudents = request.MaxStudents,
                CurrentEnrollment = 0,
                Level = request.Level,
                Prerequisites = request.PrerequisiteCourseIds != null && request.PrerequisiteCourseIds.Any()
                    ? JsonSerializer.Serialize(request.PrerequisiteCourseIds)
                    : null,
                IsActive = true,
                Syllabus = request.Syllabus,
                LearningOutcomes = request.LearningOutcomes,
                SemesterId = request.SemesterId,
                CourseFee = request.CourseFee,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Courses.AddAsync(course);
            await _unitOfWork.SaveChangesAsync();

            // Load related entities for DTO mapping
            if (course.DepartmentId.HasValue)
            {
                await _unitOfWork.Context.Entry(course)
                    .Reference(c => c.Department)
                    .LoadAsync();
            }

            if (course.TeacherId.HasValue)
            {
                await _unitOfWork.Context.Entry(course)
                    .Reference(c => c.Teacher)
                    .LoadAsync();

                if (course.Teacher != null)
                {
                    await _unitOfWork.Context.Entry(course.Teacher)
                        .Reference(t => t.User)
                        .LoadAsync();
                }
            }

            if (course.SemesterId.HasValue)
            {
                await _unitOfWork.Context.Entry(course)
                    .Reference(c => c.Semester)
                    .LoadAsync();
            }

            _logger.LogInformation("Created course {CourseCode}", request.CourseCode);

            return MapToDto(course);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating course {CourseCode}", request.CourseCode);
            throw;
        }
    }

    public async Task<CourseDto?> UpdateCourseAsync(int id, UpdateCourseRequestDto request)
    {
        try
        {
            var course = await _unitOfWork.Courses.GetByIdWithIncludesAsync(
                id,
                c => c.Department!,
                c => c.Teacher!,
                c => c.Semester!);

            if (course == null || course.IsDeleted)
            {
                return null;
            }

            // Update course fields
            if (request.CourseName != null)
                course.CourseName = request.CourseName;

            if (request.Description != null)
                course.Description = request.Description;

            if (request.Credits.HasValue)
                course.Credits = request.Credits.Value;

            if (request.TeacherId.HasValue)
            {
                // Validate teacher exists
                var teacher = await _unitOfWork.Teachers.GetByIdAsync(request.TeacherId.Value);
                if (teacher == null || teacher.IsDeleted)
                {
                    throw new InvalidOperationException("Specified teacher does not exist");
                }
                course.TeacherId = request.TeacherId;
            }

            if (request.MaxStudents.HasValue)
            {
                if (request.MaxStudents.Value < course.CurrentEnrollment)
                {
                    throw new InvalidOperationException(
                        $"Cannot set max students to {request.MaxStudents.Value} when {course.CurrentEnrollment} students are already enrolled");
                }
                course.MaxStudents = request.MaxStudents.Value;
            }

            if (request.PrerequisiteCourseIds != null)
            {
                course.Prerequisites = request.PrerequisiteCourseIds.Any()
                    ? JsonSerializer.Serialize(request.PrerequisiteCourseIds)
                    : null;
            }

            if (request.Syllabus != null)
                course.Syllabus = request.Syllabus;

            if (request.LearningOutcomes != null)
                course.LearningOutcomes = request.LearningOutcomes;

            if (request.IsActive.HasValue)
                course.IsActive = request.IsActive.Value;

            course.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Courses.Update(course);
            await _unitOfWork.SaveChangesAsync();

            // Reload related entities if they were updated
            if (request.TeacherId.HasValue)
            {
                await _unitOfWork.Context.Entry(course)
                    .Reference(c => c.Teacher)
                    .LoadAsync();

                if (course.Teacher != null)
                {
                    await _unitOfWork.Context.Entry(course.Teacher)
                        .Reference(t => t.User)
                        .LoadAsync();
                }
            }

            _logger.LogInformation("Updated course {CourseId}", id);

            return MapToDto(course);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating course {CourseId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteCourseAsync(int id)
    {
        try
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(id);

            if (course == null || course.IsDeleted)
            {
                return false;
            }

            // Check if course has active enrollments
            var enrollments = await _unitOfWork.Enrollments.FindAsync(e => e.CourseId == id);
            if (enrollments.Any())
            {
                throw new InvalidOperationException(
                    $"Cannot delete course with active enrollments. Please unenroll all students first.");
            }

            // Soft delete
            course.IsDeleted = true;
            course.IsActive = false;
            course.DeletedAt = DateTime.UtcNow;

            _unitOfWork.Courses.Update(course);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Deleted course {CourseId}", id);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting course {CourseId}", id);
            throw;
        }
    }

    public async Task<List<StudentListDto>> GetCourseEnrolledStudentsAsync(int courseId)
    {
        try
        {
            var enrollments = await _unitOfWork.Enrollments.FindAsync(e => e.CourseId == courseId);

            var students = new List<StudentListDto>();

            foreach (var enrollment in enrollments)
            {
                await _unitOfWork.Context.Entry(enrollment)
                    .Reference(e => e.Student)
                    .LoadAsync();

                if (enrollment.Student != null && !enrollment.Student.IsDeleted)
                {
                    await _unitOfWork.Context.Entry(enrollment.Student)
                        .Reference(s => s.User)
                        .LoadAsync();

                    students.Add(new StudentListDto
                    {
                        Id = enrollment.Student.Id,
                        StudentNumber = enrollment.Student.StudentNumber,
                        FullName = enrollment.Student.User!.FullName,
                        Email = enrollment.Student.User.Email!,
                        GPA = enrollment.Student.GPA,
                        CurrentSemester = enrollment.Student.CurrentSemester,
                        Status = enrollment.Student.Status,
                        Major = enrollment.Student.Major
                    });
                }
            }

            _logger.LogInformation("Retrieved {Count} enrolled students for course {CourseId}", students.Count, courseId);

            return students;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving enrolled students for course {CourseId}", courseId);
            throw;
        }
    }

    public async Task<(bool CanEnroll, string? Reason)> CanEnrollInCourseAsync(int studentId, int courseId)
    {
        try
        {
            var student = await _unitOfWork.Students.GetByIdAsync(studentId);
            if (student == null || student.IsDeleted)
            {
                return (false, "Student not found");
            }

            var course = await _unitOfWork.Courses.GetByIdAsync(courseId);
            if (course == null || course.IsDeleted)
            {
                return (false, "Course not found");
            }

            if (!course.IsActive)
            {
                return (false, "Course is not active");
            }

            // Check if already enrolled
            var existingEnrollments = await _unitOfWork.Enrollments.FindAsync(
                e => e.StudentId == studentId && e.CourseId == courseId);
            if (existingEnrollments.Any())
            {
                return (false, "Student is already enrolled in this course");
            }

            // Check capacity
            if (course.CurrentEnrollment >= course.MaxStudents)
            {
                return (false, "Course is full");
            }

            // Check prerequisites
            if (!string.IsNullOrEmpty(course.Prerequisites))
            {
                var prerequisiteIds = JsonSerializer.Deserialize<List<int>>(course.Prerequisites);
                if (prerequisiteIds != null && prerequisiteIds.Any())
                {
                    var studentGrades = await _unitOfWork.Grades.FindAsync(g => g.StudentId == studentId);
                    var completedCourseIds = studentGrades
                        .Where(g => g.Value >= 2.0m) // Passing grade
                        .Select(g => g.CourseId)
                        .ToHashSet();

                    var unmetPrerequisites = prerequisiteIds.Where(pid => !completedCourseIds.Contains(pid)).ToList();
                    if (unmetPrerequisites.Any())
                    {
                        return (false, $"Student has not met all prerequisites for this course");
                    }
                }
            }

            return (true, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking enrollment eligibility for student {StudentId} in course {CourseId}",
                studentId, courseId);
            throw;
        }
    }

    public async Task<List<CourseDto>> GetAvailableCoursesForStudentAsync(int studentId)
    {
        try
        {
            var student = await _unitOfWork.Students.GetByIdAsync(studentId);
            if (student == null || student.IsDeleted)
            {
                return new List<CourseDto>();
            }

            // Get all active courses
            var allCourses = await _unitOfWork.Courses.FindAsync(c => c.IsActive && !c.IsDeleted);

            // Get student's current enrollments
            var currentEnrollments = await _unitOfWork.Enrollments.FindAsync(e => e.StudentId == studentId);
            var enrolledCourseIds = currentEnrollments.Select(e => e.CourseId).ToHashSet();

            // Get student's completed courses (for prerequisite checking)
            var studentGrades = await _unitOfWork.Grades.FindAsync(g => g.StudentId == studentId);
            var completedCourseIds = studentGrades
                .Where(g => g.Value >= 2.0m) // Passing grade
                .Select(g => g.CourseId)
                .ToHashSet();

            var availableCourses = new List<CourseDto>();

            foreach (var course in allCourses)
            {
                // Skip if already enrolled
                if (enrolledCourseIds.Contains(course.Id))
                    continue;

                // Skip if course is full
                if (course.CurrentEnrollment >= course.MaxStudents)
                    continue;

                // Check prerequisites
                if (!string.IsNullOrEmpty(course.Prerequisites))
                {
                    var prerequisiteIds = JsonSerializer.Deserialize<List<int>>(course.Prerequisites);
                    if (prerequisiteIds != null && prerequisiteIds.Any())
                    {
                        var unmetPrerequisites = prerequisiteIds.Where(pid => !completedCourseIds.Contains(pid)).ToList();
                        if (unmetPrerequisites.Any())
                            continue; // Skip if prerequisites not met
                    }
                }

                // Load related entities
                await _unitOfWork.Context.Entry(course)
                    .Reference(c => c.Department)
                    .LoadAsync();

                await _unitOfWork.Context.Entry(course)
                    .Reference(c => c.Teacher)
                    .LoadAsync();

                if (course.Teacher != null)
                {
                    await _unitOfWork.Context.Entry(course.Teacher)
                        .Reference(t => t.User)
                        .LoadAsync();
                }

                await _unitOfWork.Context.Entry(course)
                    .Reference(c => c.Semester)
                    .LoadAsync();

                availableCourses.Add(MapToDto(course));
            }

            _logger.LogInformation("Retrieved {Count} available courses for student {StudentId}",
                availableCourses.Count, studentId);

            return availableCourses;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving available courses for student {StudentId}", studentId);
            throw;
        }
    }

    #region Private Helper Methods

    private CourseDto MapToDto(Course course)
    {
        return new CourseDto
        {
            Id = course.Id,
            CourseCode = course.CourseCode,
            CourseName = course.CourseName,
            Description = course.Description,
            Credits = course.Credits,
            DepartmentName = course.Department?.Name,
            TeacherName = course.Teacher?.User?.FullName,
            MaxStudents = course.MaxStudents,
            CurrentEnrollment = course.CurrentEnrollment,
            Level = course.Level,
            IsActive = course.IsActive,
            SemesterName = course.Semester?.Name
        };
    }

    private async Task<CourseDetailDto> MapToDetailDtoAsync(Course course)
    {
        var dto = new CourseDetailDto
        {
            Id = course.Id,
            CourseCode = course.CourseCode,
            CourseName = course.CourseName,
            Description = course.Description,
            Credits = course.Credits,
            DepartmentName = course.Department?.Name,
            TeacherName = course.Teacher?.User?.FullName,
            MaxStudents = course.MaxStudents,
            CurrentEnrollment = course.CurrentEnrollment,
            Level = course.Level,
            IsActive = course.IsActive,
            SemesterName = course.Semester?.Name,
            Syllabus = course.Syllabus,
            LearningOutcomes = course.LearningOutcomes,
            Prerequisites = !string.IsNullOrEmpty(course.Prerequisites)
                ? JsonSerializer.Deserialize<List<int>>(course.Prerequisites)?
                    .Select(id => id.ToString())
                    .ToList()
                : null,
            CourseFee = course.CourseFee,
            EnrolledStudents = new List<StudentListDto>(),
            Assignments = new List<AssignmentDto>(),
            Schedules = new List<ScheduleDto>()
        };

        // Map teacher
        if (course.Teacher != null)
        {
            dto.Teacher = new TeacherDto
            {
                Id = course.Teacher.Id,
                UserId = course.Teacher.UserId,
                EmployeeNumber = course.Teacher.EmployeeNumber,
                FirstName = course.Teacher.User?.FirstName ?? string.Empty,
                LastName = course.Teacher.User?.LastName ?? string.Empty,
                FullName = course.Teacher.User?.FullName ?? string.Empty,
                Email = course.Teacher.User?.Email ?? string.Empty,
                Specialization = course.Teacher.Specialization,
                Qualification = course.Teacher.Qualification,
                DepartmentName = course.Teacher.Department?.Name,
                HireDate = course.Teacher.HireDate,
                EmploymentType = course.Teacher.EmploymentType,
                IsActive = course.Teacher.IsActive
            };
        }

        // Map enrolled students
        if (course.Enrollments != null)
        {
            foreach (var enrollment in course.Enrollments)
            {
                if (enrollment.Student == null)
                {
                    await _unitOfWork.Context.Entry(enrollment)
                        .Reference(e => e.Student)
                        .LoadAsync();
                }

                if (enrollment.Student != null && !enrollment.Student.IsDeleted)
                {
                    if (enrollment.Student.User == null)
                    {
                        await _unitOfWork.Context.Entry(enrollment.Student)
                            .Reference(s => s.User)
                            .LoadAsync();
                    }

                    dto.EnrolledStudents.Add(new StudentListDto
                    {
                        Id = enrollment.Student.Id,
                        StudentNumber = enrollment.Student.StudentNumber,
                        FullName = enrollment.Student.User!.FullName,
                        Email = enrollment.Student.User.Email!,
                        GPA = enrollment.Student.GPA,
                        CurrentSemester = enrollment.Student.CurrentSemester,
                        Status = enrollment.Student.Status,
                        Major = enrollment.Student.Major
                    });
                }
            }
        }

        // Map assignments (placeholder - AssignmentDto structure might need to be defined)
        if (course.Assignments != null)
        {
            foreach (var assignment in course.Assignments.Where(a => !a.IsDeleted))
            {
                dto.Assignments.Add(new AssignmentDto
                {
                    Id = assignment.Id,
                    Title = assignment.Title,
                    Description = assignment.Description,
                    DueDate = assignment.DueDate,
                    MaxPoints = assignment.MaxPoints
                });
            }
        }

        // Map schedules (placeholder - ScheduleDto structure might need to be defined)
        if (course.Schedules != null)
        {
            foreach (var schedule in course.Schedules.Where(s => !s.IsDeleted))
            {
                dto.Schedules.Add(new ScheduleDto
                {
                    Id = schedule.Id,
                    DayOfWeek = schedule.DayOfWeek,
                    StartTime = schedule.StartTime,
                    EndTime = schedule.EndTime,
                    Location = schedule.Location
                });
            }
        }

        return dto;
    }

    #endregion
}
