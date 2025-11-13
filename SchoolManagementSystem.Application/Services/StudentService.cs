using AutoMapper;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Application.Interfaces;
using SchoolManagementSystem.Core.DTOs;
using SchoolManagementSystem.Core.Entities;
using SchoolManagementSystem.Core.Interfaces;

namespace SchoolManagementSystem.Application.Services;

public class StudentService : IStudentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<StudentService> _logger;
    private readonly INotificationService _notificationService;

    public StudentService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<StudentService> logger,
        INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _notificationService = notificationService;
    }

    public async Task<ApiResponse<PagedResult<StudentListDto>>> GetStudentsAsync(
        PagedRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var (students, totalCount) = await _unitOfWork.Students.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                filter: s => string.IsNullOrEmpty(request.SearchTerm) ||
                            s.User.FirstName.Contains(request.SearchTerm) ||
                            s.User.LastName.Contains(request.SearchTerm) ||
                            s.StudentNumber.Contains(request.SearchTerm),
                orderBy: query => request.SortOrder?.ToLower() == "desc"
                    ? query.OrderByDescending(s => s.GPA)
                    : query.OrderBy(s => s.GPA),
                includes: s => s.User
            );

            var studentDtos = _mapper.Map<List<StudentListDto>>(students);

            var pagedResult = new PagedResult<StudentListDto>
            {
                Items = studentDtos,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };

            return ApiResponse<PagedResult<StudentListDto>>.SuccessResponse(pagedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paginated students");
            return ApiResponse<PagedResult<StudentListDto>>.ErrorResponse(
                "Error retrieving students",
                500
            );
        }
    }

    public async Task<ApiResponse<StudentDetailDto>> GetStudentByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var student = await _unitOfWork.Students.GetByIdAsync(id, cancellationToken);
            if (student == null)
                return ApiResponse<StudentDetailDto>.ErrorResponse("Student not found", 404);

            var studentDto = _mapper.Map<StudentDetailDto>(student);
            return ApiResponse<StudentDetailDto>.SuccessResponse(studentDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting student {StudentId}", id);
            return ApiResponse<StudentDetailDto>.ErrorResponse(
                "Error retrieving student",
                500
            );
        }
    }

    public async Task<ApiResponse<StudentDetailDto>> CreateStudentAsync(
        CreateStudentRequestDto request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if email already exists
            var existingUser = await _unitOfWork.Users.FindAsync(
                u => u.Email == request.Email,
                cancellationToken
            );

            if (existingUser.Any())
                return ApiResponse<StudentDetailDto>.ErrorResponse("Email already exists", 400);

            // Create user account
            var user = new User
            {
                Email = request.Email,
                UserName = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth,
                PhoneNumber = request.PhoneNumber,
                ProfilePictureUrl = request.ProfilePictureUrl,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Users.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Generate student number
            var studentNumber = $"STU{DateTime.UtcNow.Year}{user.Id:D6}";

            // Create student profile
            var student = new Student
            {
                UserId = user.Id,
                StudentNumber = studentNumber,
                Major = request.Major,
                Minor = request.Minor,
                EnrollmentDate = request.EnrollmentDate ?? DateTime.UtcNow,
                ExpectedGraduationDate = request.ExpectedGraduationDate,
                AdvisorId = request.AdvisorId,
                Status = "Active",
                EmergencyContactName = request.EmergencyContactName,
                EmergencyContactPhone = request.EmergencyContactPhone,
                EmergencyContactRelationship = request.EmergencyContactRelationship,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Students.AddAsync(student, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created student {StudentNumber} for user {Email}", studentNumber, request.Email);

            var studentDto = _mapper.Map<StudentDetailDto>(student);
            return ApiResponse<StudentDetailDto>.SuccessResponse(
                studentDto,
                "Student created successfully"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating student");
            return ApiResponse<StudentDetailDto>.ErrorResponse(
                "Error creating student",
                500
            );
        }
    }

    public async Task<ApiResponse<StudentDetailDto>> UpdateStudentAsync(
        int id,
        UpdateStudentRequestDto request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var student = await _unitOfWork.Students.GetByIdAsync(id, cancellationToken);
            if (student == null)
                return ApiResponse<StudentDetailDto>.ErrorResponse("Student not found", 404);

            // Update user information
            if (!string.IsNullOrEmpty(request.FirstName))
                student.User.FirstName = request.FirstName;
            if (!string.IsNullOrEmpty(request.LastName))
                student.User.LastName = request.LastName;
            if (request.DateOfBirth.HasValue)
                student.User.DateOfBirth = request.DateOfBirth;
            if (!string.IsNullOrEmpty(request.PhoneNumber))
                student.User.PhoneNumber = request.PhoneNumber;
            if (!string.IsNullOrEmpty(request.ProfilePictureUrl))
                student.User.ProfilePictureUrl = request.ProfilePictureUrl;

            // Update student information
            if (!string.IsNullOrEmpty(request.Major))
                student.Major = request.Major;
            if (!string.IsNullOrEmpty(request.Minor))
                student.Minor = request.Minor;
            if (request.ExpectedGraduationDate.HasValue)
                student.ExpectedGraduationDate = request.ExpectedGraduationDate;
            if (request.AdvisorId.HasValue)
                student.AdvisorId = request.AdvisorId;
            if (!string.IsNullOrEmpty(request.Status))
                student.Status = request.Status;
            if (!string.IsNullOrEmpty(request.EmergencyContactName))
                student.EmergencyContactName = request.EmergencyContactName;
            if (!string.IsNullOrEmpty(request.EmergencyContactPhone))
                student.EmergencyContactPhone = request.EmergencyContactPhone;
            if (!string.IsNullOrEmpty(request.EmergencyContactRelationship))
                student.EmergencyContactRelationship = request.EmergencyContactRelationship;

            student.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Students.Update(student);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Updated student {StudentId}", id);

            var studentDto = _mapper.Map<StudentDetailDto>(student);
            return ApiResponse<StudentDetailDto>.SuccessResponse(
                studentDto,
                "Student updated successfully"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating student {StudentId}", id);
            return ApiResponse<StudentDetailDto>.ErrorResponse(
                "Error updating student",
                500
            );
        }
    }

    public async Task<ApiResponse<bool>> DeleteStudentAsync(
        int id,
        string deletedBy,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var student = await _unitOfWork.Students.GetByIdAsync(id, cancellationToken);
            if (student == null)
                return ApiResponse<bool>.ErrorResponse("Student not found", 404);

            await _unitOfWork.Students.SoftDeleteAsync(id, deletedBy, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Deleted student {StudentId} by {DeletedBy}", id, deletedBy);

            return ApiResponse<bool>.SuccessResponse(true, "Student deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting student {StudentId}", id);
            return ApiResponse<bool>.ErrorResponse("Error deleting student", 500);
        }
    }

    public async Task<ApiResponse<bool>> EnrollStudentAsync(
        EnrollStudentRequestDto request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var student = await _unitOfWork.Students.GetByIdAsync(request.StudentId, cancellationToken);
            if (student == null)
                return ApiResponse<bool>.ErrorResponse("Student not found", 404);

            var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId, cancellationToken);
            if (course == null)
                return ApiResponse<bool>.ErrorResponse("Course not found", 404);

            // Check if already enrolled
            var existingEnrollment = await _unitOfWork.Enrollments.FindAsync(
                e => e.StudentId == request.StudentId &&
                     e.CourseId == request.CourseId &&
                     e.Status == "Active",
                cancellationToken
            );

            if (existingEnrollment.Any())
                return ApiResponse<bool>.ErrorResponse("Student already enrolled in this course", 400);

            // Check course capacity
            var enrolledCount = await _unitOfWork.Enrollments.CountAsync(
                e => e.CourseId == request.CourseId && e.Status == "Active",
                cancellationToken
            );

            if (enrolledCount >= course.MaxStudents)
                return ApiResponse<bool>.ErrorResponse("Course is full", 400);

            // Check prerequisites
            if (!string.IsNullOrEmpty(course.Prerequisites))
            {
                var prerequisiteIds = course.Prerequisites.Split(',').Select(int.Parse).ToList();
                var completedCourses = await _unitOfWork.Enrollments.FindAsync(
                    e => e.StudentId == request.StudentId &&
                         prerequisiteIds.Contains(e.CourseId) &&
                         e.Status == "Completed",
                    cancellationToken
                );

                if (completedCourses.Count() < prerequisiteIds.Count)
                    return ApiResponse<bool>.ErrorResponse("Prerequisites not met", 400);
            }

            // Create enrollment
            var enrollment = new Enrollment
            {
                StudentId = request.StudentId,
                CourseId = request.CourseId,
                EnrollmentDate = DateTime.UtcNow,
                Status = "Active",
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Enrollments.AddAsync(enrollment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Send notification
            await _notificationService.NotifyEnrollmentConfirmationAsync(
                request.StudentId,
                request.CourseId,
                cancellationToken
            );

            _logger.LogInformation(
                "Student {StudentId} enrolled in course {CourseId}",
                request.StudentId,
                request.CourseId
            );

            return ApiResponse<bool>.SuccessResponse(true, "Student enrolled successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enrolling student");
            return ApiResponse<bool>.ErrorResponse("Error enrolling student", 500);
        }
    }

    public async Task<ApiResponse<bool>> UnenrollStudentAsync(
        int studentId,
        int courseId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var enrollment = await _unitOfWork.Enrollments.FindAsync(
                e => e.StudentId == studentId && e.CourseId == courseId && e.Status == "Active",
                cancellationToken
            );

            if (!enrollment.Any())
                return ApiResponse<bool>.ErrorResponse("Active enrollment not found", 404);

            var enrollmentToUpdate = enrollment.First();
            enrollmentToUpdate.Status = "Dropped";
            enrollmentToUpdate.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Enrollments.Update(enrollmentToUpdate);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Student {StudentId} unenrolled from course {CourseId}",
                studentId,
                courseId
            );

            return ApiResponse<bool>.SuccessResponse(true, "Student unenrolled successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unenrolling student");
            return ApiResponse<bool>.ErrorResponse("Error unenrolling student", 500);
        }
    }

    public async Task<ApiResponse<TranscriptDto>> GetStudentTranscriptAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var student = await _unitOfWork.Students.GetByIdAsync(id, cancellationToken);
            if (student == null)
                return ApiResponse<TranscriptDto>.ErrorResponse("Student not found", 404);

            var grades = await _unitOfWork.Grades.FindAsync(
                g => g.StudentId == id,
                cancellationToken
            );

            var enrollments = await _unitOfWork.Enrollments.FindAsync(
                e => e.StudentId == id,
                cancellationToken
            );

            var transcript = new TranscriptDto
            {
                StudentId = student.Id,
                StudentName = student.User.FullName,
                StudentNumber = student.StudentNumber,
                Major = student.Major ?? "",
                Minor = student.Minor ?? "",
                EnrollmentDate = student.EnrollmentDate,
                CumulativeGPA = student.GPA,
                TotalCreditsEarned = student.TotalCreditsEarned,
                Semesters = new List<SemesterTranscriptDto>() // Would group by semester
            };

            return ApiResponse<TranscriptDto>.SuccessResponse(transcript);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating transcript for student {StudentId}", id);
            return ApiResponse<TranscriptDto>.ErrorResponse("Error generating transcript", 500);
        }
    }

    public async Task<ApiResponse<IEnumerable<CourseDto>>> GetStudentCoursesAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var enrollments = await _unitOfWork.Enrollments.FindAsync(
                e => e.StudentId == id && e.Status == "Active",
                cancellationToken
            );

            var courseIds = enrollments.Select(e => e.CourseId).ToList();
            var courses = await _unitOfWork.Courses.FindAsync(
                c => courseIds.Contains(c.Id),
                cancellationToken
            );

            var courseDtos = _mapper.Map<IEnumerable<CourseDto>>(courses);
            return ApiResponse<IEnumerable<CourseDto>>.SuccessResponse(courseDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting courses for student {StudentId}", id);
            return ApiResponse<IEnumerable<CourseDto>>.ErrorResponse(
                "Error retrieving student courses",
                500
            );
        }
    }

    public async Task<ApiResponse<IEnumerable<GradeDto>>> GetStudentGradesAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var grades = await _unitOfWork.Grades.FindAsync(
                g => g.StudentId == id,
                cancellationToken
            );

            var gradeDtos = _mapper.Map<IEnumerable<GradeDto>>(grades);
            return ApiResponse<IEnumerable<GradeDto>>.SuccessResponse(gradeDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting grades for student {StudentId}", id);
            return ApiResponse<IEnumerable<GradeDto>>.ErrorResponse(
                "Error retrieving student grades",
                500
            );
        }
    }

    public async Task<ApiResponse<StudentStatisticsDto>> GetStudentStatisticsAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var student = await _unitOfWork.Students.GetByIdAsync(id, cancellationToken);
            if (student == null)
                return ApiResponse<StudentStatisticsDto>.ErrorResponse("Student not found", 404);

            var enrollments = await _unitOfWork.Enrollments.FindAsync(
                e => e.StudentId == id,
                cancellationToken
            );

            var grades = await _unitOfWork.Grades.FindAsync(
                g => g.StudentId == id,
                cancellationToken
            );

            var submissions = await _unitOfWork.AssignmentSubmissions.FindAsync(
                s => s.StudentId == id,
                cancellationToken
            );

            var attendances = await _unitOfWork.Attendances.FindAsync(
                a => a.StudentId == id,
                cancellationToken
            );

            var statistics = new StudentStatisticsDto
            {
                TotalCoursesEnrolled = enrollments.Count(),
                CompletedCourses = enrollments.Count(e => e.Status == "Completed"),
                InProgressCourses = enrollments.Count(e => e.Status == "Active"),
                OverallGPA = student.GPA,
                CurrentSemesterGPA = 0, // Calculate based on current semester
                AverageAttendanceRate = attendances.Any()
                    ? (decimal)attendances.Count(a => a.Status == "Present") / attendances.Count() * 100
                    : 0,
                TotalAssignmentsSubmitted = submissions.Count(),
                TotalAssignmentsGraded = submissions.Count(s => s.Grade.HasValue),
                AssignmentCompletionRate = 0 // Calculate based on total assignments available
            };

            return ApiResponse<StudentStatisticsDto>.SuccessResponse(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting statistics for student {StudentId}", id);
            return ApiResponse<StudentStatisticsDto>.ErrorResponse(
                "Error retrieving student statistics",
                500
            );
        }
    }
}
