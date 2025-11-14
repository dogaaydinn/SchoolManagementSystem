using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Core.DTOs;
using SchoolManagementSystem.Core.Interfaces;

namespace SchoolManagementSystem.API.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
public class AssignmentsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AssignmentsController> _logger;

    public AssignmentsController(IUnitOfWork unitOfWork, ILogger<AssignmentsController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<AssignmentDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAssignments([FromQuery] int? courseId)
    {
        try
        {
            var assignments = courseId.HasValue
                ? await _unitOfWork.Assignments.FindAsync(a => a.CourseId == courseId.Value && a.IsPublished)
                : await _unitOfWork.Assignments.FindAsync(a => a.IsPublished);

            var assignmentDtos = assignments.Select(a => new AssignmentDto
            {
                Id = a.Id,
                CourseId = a.CourseId,
                Title = a.Title,
                Description = a.Description,
                DueDate = a.DueDate,
                MaxScore = a.MaxScore,
                Weight = a.Weight,
                Type = a.Type,
                AllowLateSubmission = a.AllowLateSubmission,
                LatePenaltyPercentage = a.LatePenaltyPercentage,
                IsPublished = a.IsPublished,
                TotalSubmissions = a.TotalSubmissions,
                GradedSubmissions = a.GradedSubmissions
            }).ToList();

            return Ok(ApiResponse<List<AssignmentDto>>.SuccessResponse(assignmentDtos));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving assignments");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error retrieving assignments", 500));
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<AssignmentDetailDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAssignment(int id)
    {
        try
        {
            var assignment = await _unitOfWork.Assignments.GetByIdWithIncludesAsync(
                id,
                a => a.Course,
                a => a.Teacher,
                a => a.Submissions
            );

            if (assignment == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Assignment not found", 404));
            }

            var assignmentDto = new AssignmentDetailDto
            {
                Id = assignment.Id,
                CourseId = assignment.CourseId,
                CourseName = assignment.Course.CourseName,
                Title = assignment.Title,
                Description = assignment.Description,
                Instructions = assignment.Instructions,
                DueDate = assignment.DueDate,
                MaxScore = assignment.MaxScore,
                Weight = assignment.Weight,
                Type = assignment.Type,
                AllowLateSubmission = assignment.AllowLateSubmission,
                LatePenaltyPercentage = assignment.LatePenaltyPercentage,
                AttachmentUrl = assignment.AttachmentUrl,
                IsPublished = assignment.IsPublished,
                PublishedDate = assignment.PublishedDate,
                TotalSubmissions = assignment.TotalSubmissions,
                GradedSubmissions = assignment.GradedSubmissions,
                TeacherName = assignment.Teacher.User.FullName,
                Submissions = assignment.Submissions.Select(s => new AssignmentSubmissionDto
                {
                    Id = s.Id,
                    AssignmentId = s.AssignmentId,
                    StudentId = s.StudentId,
                    SubmittedAt = s.SubmittedAt,
                    FileUrl = s.FileUrl,
                    FileName = s.FileName,
                    FileSize = s.FileSize,
                    Score = s.Score,
                    Feedback = s.Feedback,
                    Status = s.Status,
                    IsLate = s.IsLate,
                    PlagiarismScore = s.PlagiarismScore
                }).ToList()
            };

            return Ok(ApiResponse<AssignmentDetailDto>.SuccessResponse(assignmentDto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving assignment {AssignmentId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error retrieving assignment", 500));
        }
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
    [ProducesResponseType(typeof(ApiResponse<AssignmentDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAssignment([FromBody] CreateAssignmentRequestDto request)
    {
        try
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);
            if (course == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Course not found", 404));
            }

            var teacherId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

            var assignment = new Core.Entities.Assignment
            {
                CourseId = request.CourseId,
                TeacherId = teacherId,
                Title = request.Title,
                Description = request.Description,
                Instructions = request.Instructions,
                DueDate = request.DueDate,
                MaxScore = request.MaxScore,
                Weight = request.Weight,
                Type = request.Type,
                AllowLateSubmission = request.AllowLateSubmission,
                LatePenaltyPercentage = request.LatePenaltyPercentage,
                IsPublished = request.IsPublished,
                PublishedDate = request.IsPublished ? DateTime.UtcNow : null
            };

            await _unitOfWork.Assignments.AddAsync(assignment);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Assignment created: {AssignmentId} for course {CourseId}", assignment.Id, request.CourseId);

            var assignmentDto = new AssignmentDto
            {
                Id = assignment.Id,
                CourseId = assignment.CourseId,
                Title = assignment.Title,
                DueDate = assignment.DueDate,
                MaxScore = assignment.MaxScore,
                IsPublished = assignment.IsPublished
            };

            return CreatedAtAction(nameof(GetAssignment), new { id = assignment.Id },
                ApiResponse<AssignmentDto>.SuccessResponse(assignmentDto, "Assignment created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating assignment");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error creating assignment", 500));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
    [ProducesResponseType(typeof(ApiResponse<AssignmentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateAssignment(int id, [FromBody] UpdateAssignmentRequestDto request)
    {
        try
        {
            var assignment = await _unitOfWork.Assignments.GetByIdAsync(id);
            if (assignment == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Assignment not found", 404));
            }

            if (!string.IsNullOrEmpty(request.Title))
                assignment.Title = request.Title;
            if (!string.IsNullOrEmpty(request.Description))
                assignment.Description = request.Description;
            if (!string.IsNullOrEmpty(request.Instructions))
                assignment.Instructions = request.Instructions;
            if (request.DueDate.HasValue)
                assignment.DueDate = request.DueDate.Value;
            if (request.MaxScore.HasValue)
                assignment.MaxScore = request.MaxScore.Value;
            if (request.Weight.HasValue)
                assignment.Weight = request.Weight.Value;
            if (request.AllowLateSubmission.HasValue)
                assignment.AllowLateSubmission = request.AllowLateSubmission.Value;
            if (request.LatePenaltyPercentage.HasValue)
                assignment.LatePenaltyPercentage = request.LatePenaltyPercentage;
            if (request.IsPublished.HasValue && request.IsPublished.Value && !assignment.IsPublished)
            {
                assignment.IsPublished = true;
                assignment.PublishedDate = DateTime.UtcNow;
            }

            _unitOfWork.Assignments.Update(assignment);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Assignment updated: {AssignmentId}", id);

            return Ok(ApiResponse<AssignmentDto>.SuccessResponse(new AssignmentDto
            {
                Id = assignment.Id,
                Title = assignment.Title,
                IsPublished = assignment.IsPublished
            }, "Assignment updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating assignment {AssignmentId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error updating assignment", 500));
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteAssignment(int id)
    {
        try
        {
            var assignment = await _unitOfWork.Assignments.GetByIdAsync(id);
            if (assignment == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Assignment not found", 404));
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            await _unitOfWork.Assignments.SoftDeleteAsync(id, userId ?? "System");
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Assignment deleted: {AssignmentId}", id);

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Assignment deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting assignment {AssignmentId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error deleting assignment", 500));
        }
    }

    [HttpPost("{id}/submit")]
    [Authorize(Roles = "Student")]
    [ProducesResponseType(typeof(ApiResponse<AssignmentSubmissionDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> SubmitAssignment(int id, [FromBody] SubmitAssignmentRequestDto request)
    {
        try
        {
            var assignment = await _unitOfWork.Assignments.GetByIdAsync(id);
            if (assignment == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Assignment not found", 404));
            }

            var studentId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

            // Check if already submitted
            var existingSubmission = await _unitOfWork.AssignmentSubmissions.FirstOrDefaultAsync(
                s => s.AssignmentId == id && s.StudentId == studentId
            );

            if (existingSubmission != null)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Assignment already submitted", 400));
            }

            var isLate = DateTime.UtcNow > assignment.DueDate;

            if (isLate && !assignment.AllowLateSubmission)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Late submissions not allowed", 400));
            }

            var submission = new Core.Entities.AssignmentSubmission
            {
                AssignmentId = id,
                StudentId = studentId,
                SubmittedAt = DateTime.UtcNow,
                FileUrl = request.FileUrl,
                FileName = request.FileName,
                FileSize = request.FileSize,
                SubmissionText = request.SubmissionText,
                Status = "Submitted",
                IsLate = isLate
            };

            await _unitOfWork.AssignmentSubmissions.AddAsync(submission);

            // Update assignment submission count
            assignment.TotalSubmissions++;
            _unitOfWork.Assignments.Update(assignment);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Assignment {AssignmentId} submitted by student {StudentId}", id, studentId);

            var submissionDto = new AssignmentSubmissionDto
            {
                Id = submission.Id,
                AssignmentId = submission.AssignmentId,
                StudentId = submission.StudentId,
                SubmittedAt = submission.SubmittedAt,
                FileUrl = submission.FileUrl,
                FileName = submission.FileName,
                Status = submission.Status,
                IsLate = submission.IsLate
            };

            return CreatedAtAction(nameof(GetSubmission), new { id = submission.Id },
                ApiResponse<AssignmentSubmissionDto>.SuccessResponse(submissionDto, "Assignment submitted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting assignment {AssignmentId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error submitting assignment", 500));
        }
    }

    [HttpGet("submissions/{id}")]
    [ProducesResponseType(typeof(ApiResponse<AssignmentSubmissionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSubmission(int id)
    {
        try
        {
            var submission = await _unitOfWork.AssignmentSubmissions.GetByIdAsync(id);
            if (submission == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Submission not found", 404));
            }

            var submissionDto = new AssignmentSubmissionDto
            {
                Id = submission.Id,
                AssignmentId = submission.AssignmentId,
                StudentId = submission.StudentId,
                SubmittedAt = submission.SubmittedAt,
                FileUrl = submission.FileUrl,
                FileName = submission.FileName,
                FileSize = submission.FileSize,
                Score = submission.Score,
                Feedback = submission.Feedback,
                Status = submission.Status,
                IsLate = submission.IsLate,
                PlagiarismScore = submission.PlagiarismScore
            };

            return Ok(ApiResponse<AssignmentSubmissionDto>.SuccessResponse(submissionDto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving submission {SubmissionId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error retrieving submission", 500));
        }
    }

    [HttpGet("{id}/submissions")]
    [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
    [ProducesResponseType(typeof(ApiResponse<List<AssignmentSubmissionDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAssignmentSubmissions(int id)
    {
        try
        {
            var submissions = await _unitOfWork.AssignmentSubmissions.FindAsync(s => s.AssignmentId == id);

            var submissionDtos = new List<AssignmentSubmissionDto>();
            foreach (var submission in submissions)
            {
                var student = await _unitOfWork.Students.GetByIdWithIncludesAsync(submission.StudentId, s => s.User);
                submissionDtos.Add(new AssignmentSubmissionDto
                {
                    Id = submission.Id,
                    AssignmentId = submission.AssignmentId,
                    StudentId = submission.StudentId,
                    StudentName = student?.User.FullName ?? "Unknown",
                    SubmittedAt = submission.SubmittedAt,
                    FileUrl = submission.FileUrl,
                    FileName = submission.FileName,
                    Score = submission.Score,
                    Status = submission.Status,
                    IsLate = submission.IsLate
                });
            }

            return Ok(ApiResponse<List<AssignmentSubmissionDto>>.SuccessResponse(submissionDtos));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving submissions for assignment {AssignmentId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error retrieving submissions", 500));
        }
    }

    [HttpPost("submissions/{id}/grade")]
    [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GradeSubmission(int id, [FromBody] GradeSubmissionRequestDto request)
    {
        try
        {
            var submission = await _unitOfWork.AssignmentSubmissions.GetByIdAsync(id);
            if (submission == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Submission not found", 404));
            }

            submission.Score = request.Score;
            submission.Feedback = request.Feedback;
            submission.Status = "Graded";
            submission.GradedAt = DateTime.UtcNow;
            submission.GradedBy = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

            _unitOfWork.AssignmentSubmissions.Update(submission);

            // Update assignment graded count
            var assignment = await _unitOfWork.Assignments.GetByIdAsync(submission.AssignmentId);
            if (assignment != null)
            {
                assignment.GradedSubmissions++;
                _unitOfWork.Assignments.Update(assignment);
            }

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Submission {SubmissionId} graded", id);

            return Ok(ApiResponse<object>.SuccessResponse(null, "Submission graded successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error grading submission {SubmissionId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error grading submission", 500));
        }
    }
}
