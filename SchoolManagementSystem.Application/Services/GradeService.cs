using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Core.DTOs;
using SchoolManagementSystem.Core.Entities;
using SchoolManagementSystem.Core.Interfaces;

namespace SchoolManagementSystem.Application.Services;

/// <summary>
/// Service implementation for grade management operations
/// </summary>
public class GradeService : IGradeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GradeService> _logger;

    public GradeService(IUnitOfWork unitOfWork, ILogger<GradeService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<PagedResult<GradeDto>> GetAllGradesAsync(
        int pageNumber = 1,
        int pageSize = 10,
        int? studentId = null,
        int? courseId = null,
        int? assignmentId = null,
        bool? isPublished = null)
    {
        try
        {
            var query = _unitOfWork.Grades.GetAllQueryable()
                .Include(g => g.Student)
                    .ThenInclude(s => s.User)
                .Include(g => g.Course)
                .Where(g => !g.IsDeleted);

            // Apply student filter
            if (studentId.HasValue)
            {
                query = query.Where(g => g.StudentId == studentId.Value);
            }

            // Apply course filter
            if (courseId.HasValue)
            {
                query = query.Where(g => g.CourseId == courseId.Value);
            }

            // Apply assignment filter
            if (assignmentId.HasValue)
            {
                query = query.Where(g => g.AssignmentId == assignmentId.Value);
            }

            // Apply published status filter
            if (isPublished.HasValue)
            {
                query = query.Where(g => g.IsPublished == isPublished.Value);
            }

            var totalCount = await query.CountAsync();

            var grades = await query
                .OrderByDescending(g => g.GradeDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(g => new GradeDto
                {
                    Id = g.Id,
                    StudentId = g.StudentId,
                    StudentName = g.Student.User!.FullName,
                    CourseId = g.CourseId,
                    CourseName = g.Course.CourseName,
                    CourseCode = g.Course.CourseCode,
                    GradeType = g.GradeType,
                    Value = g.Value,
                    MaxValue = g.MaxValue,
                    LetterGrade = g.LetterGrade,
                    Weight = g.Weight,
                    GradeDate = g.GradeDate,
                    Comments = g.Comments,
                    IsPublished = g.IsPublished
                })
                .ToListAsync();

            return new PagedResult<GradeDto>
            {
                Items = grades,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving grades list");
            throw;
        }
    }

    public async Task<GradeDto?> GetGradeByIdAsync(int id)
    {
        try
        {
            var grade = await _unitOfWork.Grades.GetByIdWithIncludesAsync(
                id,
                g => g.Student!,
                g => g.Course!);

            if (grade == null || grade.IsDeleted)
            {
                return null;
            }

            // Load user for student
            if (grade.Student != null)
            {
                await _unitOfWork.Context.Entry(grade.Student)
                    .Reference(s => s.User)
                    .LoadAsync();
            }

            return MapToDto(grade);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving grade with ID {GradeId}", id);
            throw;
        }
    }

    public async Task<List<GradeDto>> GetGradesByStudentIdAsync(int studentId)
    {
        try
        {
            var grades = await _unitOfWork.Grades.FindAsync(g => g.StudentId == studentId && !g.IsDeleted);

            var gradeDtos = new List<GradeDto>();

            foreach (var grade in grades)
            {
                await _unitOfWork.Context.Entry(grade)
                    .Reference(g => g.Student)
                    .LoadAsync();

                if (grade.Student != null)
                {
                    await _unitOfWork.Context.Entry(grade.Student)
                        .Reference(s => s.User)
                        .LoadAsync();
                }

                await _unitOfWork.Context.Entry(grade)
                    .Reference(g => g.Course)
                    .LoadAsync();

                gradeDtos.Add(MapToDto(grade));
            }

            _logger.LogInformation("Retrieved {Count} grades for student {StudentId}", gradeDtos.Count, studentId);

            return gradeDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving grades for student {StudentId}", studentId);
            throw;
        }
    }

    public async Task<List<GradeDto>> GetGradesByCourseIdAsync(int courseId)
    {
        try
        {
            var grades = await _unitOfWork.Grades.FindAsync(g => g.CourseId == courseId && !g.IsDeleted);

            var gradeDtos = new List<GradeDto>();

            foreach (var grade in grades)
            {
                await _unitOfWork.Context.Entry(grade)
                    .Reference(g => g.Student)
                    .LoadAsync();

                if (grade.Student != null)
                {
                    await _unitOfWork.Context.Entry(grade.Student)
                        .Reference(s => s.User)
                        .LoadAsync();
                }

                await _unitOfWork.Context.Entry(grade)
                    .Reference(g => g.Course)
                    .LoadAsync();

                gradeDtos.Add(MapToDto(grade));
            }

            _logger.LogInformation("Retrieved {Count} grades for course {CourseId}", gradeDtos.Count, courseId);

            return gradeDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving grades for course {CourseId}", courseId);
            throw;
        }
    }

    public async Task<List<GradeDto>> GetGradesByAssignmentIdAsync(int assignmentId)
    {
        try
        {
            var grades = await _unitOfWork.Grades.FindAsync(g => g.AssignmentId == assignmentId && !g.IsDeleted);

            var gradeDtos = new List<GradeDto>();

            foreach (var grade in grades)
            {
                await _unitOfWork.Context.Entry(grade)
                    .Reference(g => g.Student)
                    .LoadAsync();

                if (grade.Student != null)
                {
                    await _unitOfWork.Context.Entry(grade.Student)
                        .Reference(s => s.User)
                        .LoadAsync();
                }

                await _unitOfWork.Context.Entry(grade)
                    .Reference(g => g.Course)
                    .LoadAsync();

                gradeDtos.Add(MapToDto(grade));
            }

            _logger.LogInformation("Retrieved {Count} grades for assignment {AssignmentId}", gradeDtos.Count, assignmentId);

            return gradeDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving grades for assignment {AssignmentId}", assignmentId);
            throw;
        }
    }

    public async Task<GradeDto> CreateGradeAsync(CreateGradeRequestDto request)
    {
        try
        {
            // Validate student exists
            var student = await _unitOfWork.Students.GetByIdAsync(request.StudentId);
            if (student == null || student.IsDeleted)
            {
                throw new InvalidOperationException("Student not found");
            }

            // Validate course exists
            var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);
            if (course == null || course.IsDeleted)
            {
                throw new InvalidOperationException("Course not found");
            }

            // Validate enrollment if provided
            if (request.EnrollmentId.HasValue)
            {
                var enrollment = await _unitOfWork.Enrollments.GetByIdAsync(request.EnrollmentId.Value);
                if (enrollment == null || enrollment.StudentId != request.StudentId || enrollment.CourseId != request.CourseId)
                {
                    throw new InvalidOperationException("Invalid enrollment for this student and course");
                }
            }

            // Calculate percentage and letter grade
            var percentage = (request.Value / request.MaxValue) * 100;
            var letterGrade = CalculateLetterGrade(percentage);

            // Create grade
            var grade = new Grade
            {
                StudentId = request.StudentId,
                CourseId = request.CourseId,
                EnrollmentId = request.EnrollmentId,
                AssignmentId = request.AssignmentId,
                GradeType = request.GradeType,
                Value = request.Value,
                MaxValue = request.MaxValue,
                LetterGrade = letterGrade,
                Weight = request.Weight,
                GradeDate = DateTime.UtcNow,
                Comments = request.Comments,
                IsPublished = request.IsPublished,
                PublishedDate = request.IsPublished ? DateTime.UtcNow : null,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Grades.AddAsync(grade);
            await _unitOfWork.SaveChangesAsync();

            // Load related entities
            await _unitOfWork.Context.Entry(grade)
                .Reference(g => g.Student)
                .LoadAsync();

            if (grade.Student != null)
            {
                await _unitOfWork.Context.Entry(grade.Student)
                    .Reference(s => s.User)
                    .LoadAsync();
            }

            await _unitOfWork.Context.Entry(grade)
                .Reference(g => g.Course)
                .LoadAsync();

            _logger.LogInformation("Created grade for student {StudentId} in course {CourseId}", request.StudentId, request.CourseId);

            return MapToDto(grade);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating grade for student {StudentId} in course {CourseId}", request.StudentId, request.CourseId);
            throw;
        }
    }

    public async Task<List<GradeDto>> CreateBulkGradesAsync(BulkGradeRequestDto request)
    {
        try
        {
            // Validate course exists
            var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);
            if (course == null || course.IsDeleted)
            {
                throw new InvalidOperationException("Course not found");
            }

            var createdGrades = new List<GradeDto>();

            foreach (var studentGrade in request.StudentGrades)
            {
                // Validate student exists
                var student = await _unitOfWork.Students.GetByIdAsync(studentGrade.StudentId);
                if (student == null || student.IsDeleted)
                {
                    _logger.LogWarning("Skipping grade for student {StudentId} - student not found", studentGrade.StudentId);
                    continue;
                }

                // Calculate percentage and letter grade
                var percentage = (studentGrade.Value / request.MaxValue) * 100;
                var letterGrade = CalculateLetterGrade(percentage);

                // Create grade
                var grade = new Grade
                {
                    StudentId = studentGrade.StudentId,
                    CourseId = request.CourseId,
                    AssignmentId = request.AssignmentId,
                    GradeType = request.GradeType,
                    Value = studentGrade.Value,
                    MaxValue = request.MaxValue,
                    LetterGrade = letterGrade,
                    Weight = request.Weight,
                    GradeDate = DateTime.UtcNow,
                    Comments = studentGrade.Comments,
                    IsPublished = false, // Bulk grades are unpublished by default
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Grades.AddAsync(grade);
            }

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Created {Count} bulk grades for course {CourseId}", request.StudentGrades.Count, request.CourseId);

            // Fetch and return created grades
            var createdGradeEntities = await _unitOfWork.Grades.FindAsync(
                g => g.CourseId == request.CourseId &&
                     g.AssignmentId == request.AssignmentId &&
                     !g.IsDeleted &&
                     g.GradeType == request.GradeType);

            foreach (var grade in createdGradeEntities.OrderByDescending(g => g.CreatedAt).Take(request.StudentGrades.Count))
            {
                await _unitOfWork.Context.Entry(grade)
                    .Reference(g => g.Student)
                    .LoadAsync();

                if (grade.Student != null)
                {
                    await _unitOfWork.Context.Entry(grade.Student)
                        .Reference(s => s.User)
                        .LoadAsync();
                }

                await _unitOfWork.Context.Entry(grade)
                    .Reference(g => g.Course)
                    .LoadAsync();

                createdGrades.Add(MapToDto(grade));
            }

            return createdGrades;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating bulk grades for course {CourseId}", request.CourseId);
            throw;
        }
    }

    public async Task<GradeDto?> UpdateGradeAsync(int id, UpdateGradeRequestDto request)
    {
        try
        {
            var grade = await _unitOfWork.Grades.GetByIdWithIncludesAsync(
                id,
                g => g.Student!,
                g => g.Course!);

            if (grade == null || grade.IsDeleted)
            {
                return null;
            }

            // Update grade fields
            var recalculateLetterGrade = false;

            if (request.Value.HasValue)
            {
                grade.Value = request.Value.Value;
                recalculateLetterGrade = true;
            }

            if (request.MaxValue.HasValue)
            {
                grade.MaxValue = request.MaxValue.Value;
                recalculateLetterGrade = true;
            }

            if (request.Weight.HasValue)
            {
                grade.Weight = request.Weight.Value;
            }

            if (request.Comments != null)
            {
                grade.Comments = request.Comments;
            }

            if (request.IsPublished.HasValue && request.IsPublished.Value && !grade.IsPublished)
            {
                grade.IsPublished = true;
                grade.PublishedDate = DateTime.UtcNow;
            }
            else if (request.IsPublished.HasValue)
            {
                grade.IsPublished = request.IsPublished.Value;
            }

            // Recalculate letter grade if value or max value changed
            if (recalculateLetterGrade)
            {
                var percentage = (grade.Value / grade.MaxValue) * 100;
                grade.LetterGrade = CalculateLetterGrade(percentage);
            }

            grade.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Grades.Update(grade);
            await _unitOfWork.SaveChangesAsync();

            // Load user for student
            if (grade.Student != null)
            {
                await _unitOfWork.Context.Entry(grade.Student)
                    .Reference(s => s.User)
                    .LoadAsync();
            }

            _logger.LogInformation("Updated grade {GradeId}", id);

            return MapToDto(grade);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating grade {GradeId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteGradeAsync(int id)
    {
        try
        {
            var grade = await _unitOfWork.Grades.GetByIdAsync(id);

            if (grade == null || grade.IsDeleted)
            {
                return false;
            }

            // Soft delete
            grade.IsDeleted = true;
            grade.DeletedAt = DateTime.UtcNow;

            _unitOfWork.Grades.Update(grade);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Deleted grade {GradeId}", id);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting grade {GradeId}", id);
            throw;
        }
    }

    public async Task<bool> PublishGradeAsync(int id)
    {
        try
        {
            var grade = await _unitOfWork.Grades.GetByIdAsync(id);

            if (grade == null || grade.IsDeleted)
            {
                return false;
            }

            if (grade.IsPublished)
            {
                _logger.LogWarning("Grade {GradeId} is already published", id);
                return true;
            }

            grade.IsPublished = true;
            grade.PublishedDate = DateTime.UtcNow;
            grade.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Grades.Update(grade);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Published grade {GradeId}", id);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing grade {GradeId}", id);
            throw;
        }
    }

    public string CalculateLetterGrade(decimal percentage)
    {
        return percentage switch
        {
            >= 93 => "A",
            >= 90 => "A-",
            >= 87 => "B+",
            >= 83 => "B",
            >= 80 => "B-",
            >= 77 => "C+",
            >= 73 => "C",
            >= 70 => "C-",
            >= 67 => "D+",
            >= 63 => "D",
            >= 60 => "D-",
            _ => "F"
        };
    }

    #region Private Helper Methods

    private GradeDto MapToDto(Grade grade)
    {
        return new GradeDto
        {
            Id = grade.Id,
            StudentId = grade.StudentId,
            StudentName = grade.Student?.User?.FullName ?? string.Empty,
            CourseId = grade.CourseId,
            CourseName = grade.Course?.CourseName ?? string.Empty,
            CourseCode = grade.Course?.CourseCode ?? string.Empty,
            GradeType = grade.GradeType,
            Value = grade.Value,
            MaxValue = grade.MaxValue,
            LetterGrade = grade.LetterGrade,
            Weight = grade.Weight,
            GradeDate = grade.GradeDate,
            Comments = grade.Comments,
            IsPublished = grade.IsPublished
        };
    }

    #endregion
}
