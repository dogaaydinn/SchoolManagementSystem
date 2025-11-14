using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Core.DTOs;
using SchoolManagementSystem.Core.Interfaces;

namespace SchoolManagementSystem.API.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
public class AttendanceController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AttendanceController> _logger;

    public AttendanceController(IUnitOfWork unitOfWork, ILogger<AttendanceController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
    [ProducesResponseType(typeof(ApiResponse<AttendanceDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> MarkAttendance([FromBody] CreateAttendanceRequestDto request)
    {
        try
        {
            var student = await _unitOfWork.Students.GetByIdAsync(request.StudentId);
            if (student == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Student not found", 404));
            }

            var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);
            if (course == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Course not found", 404));
            }

            // Check if attendance already marked for this date
            var existingAttendance = await _unitOfWork.Attendances.FirstOrDefaultAsync(
                a => a.StudentId == request.StudentId &&
                     a.CourseId == request.CourseId &&
                     a.Date.Date == request.Date.Date
            );

            if (existingAttendance != null)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Attendance already marked for this date", 400));
            }

            var attendance = new Core.Entities.Attendance
            {
                StudentId = request.StudentId,
                CourseId = request.CourseId,
                ScheduleId = request.ScheduleId,
                Date = request.Date,
                Status = request.Status,
                Remarks = request.Remarks,
                IsExcused = request.IsExcused,
                ExcuseReason = request.ExcuseReason,
                MarkedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            };

            await _unitOfWork.Attendances.AddAsync(attendance);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Attendance marked for student {StudentId} in course {CourseId}", request.StudentId, request.CourseId);

            var attendanceDto = new AttendanceDto
            {
                Id = attendance.Id,
                StudentId = attendance.StudentId,
                StudentName = student.User.FullName,
                CourseId = attendance.CourseId,
                CourseName = course.CourseName,
                Date = attendance.Date,
                Status = attendance.Status,
                Remarks = attendance.Remarks,
                IsExcused = attendance.IsExcused
            };

            return CreatedAtAction(nameof(GetAttendance), new { id = attendance.Id },
                ApiResponse<AttendanceDto>.SuccessResponse(attendanceDto, "Attendance marked successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking attendance");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error marking attendance", 500));
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<AttendanceDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAttendance(int id)
    {
        try
        {
            var attendance = await _unitOfWork.Attendances.GetByIdAsync(id);
            if (attendance == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Attendance not found", 404));
            }

            var student = await _unitOfWork.Students.GetByIdWithIncludesAsync(attendance.StudentId, s => s.User);
            var course = await _unitOfWork.Courses.GetByIdAsync(attendance.CourseId);

            var attendanceDto = new AttendanceDto
            {
                Id = attendance.Id,
                StudentId = attendance.StudentId,
                StudentName = student?.User.FullName ?? "Unknown",
                CourseId = attendance.CourseId,
                CourseName = course?.CourseName ?? "Unknown",
                Date = attendance.Date,
                Status = attendance.Status,
                Remarks = attendance.Remarks,
                IsExcused = attendance.IsExcused
            };

            return Ok(ApiResponse<AttendanceDto>.SuccessResponse(attendanceDto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving attendance {AttendanceId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error retrieving attendance", 500));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
    [ProducesResponseType(typeof(ApiResponse<AttendanceDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateAttendance(int id, [FromBody] CreateAttendanceRequestDto request)
    {
        try
        {
            var attendance = await _unitOfWork.Attendances.GetByIdAsync(id);
            if (attendance == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Attendance not found", 404));
            }

            attendance.Status = request.Status;
            attendance.Remarks = request.Remarks;
            attendance.IsExcused = request.IsExcused;
            attendance.ExcuseReason = request.ExcuseReason;

            _unitOfWork.Attendances.Update(attendance);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Attendance updated: {AttendanceId}", id);

            return Ok(ApiResponse<AttendanceDto>.SuccessResponse(new AttendanceDto
            {
                Id = attendance.Id,
                Status = attendance.Status
            }, "Attendance updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating attendance {AttendanceId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error updating attendance", 500));
        }
    }

    [HttpPost("bulk")]
    [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status201Created)]
    public async Task<IActionResult> BulkMarkAttendance([FromBody] BulkAttendanceRequestDto request)
    {
        try
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);
            if (course == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Course not found", 404));
            }

            int successCount = 0;
            int failedCount = 0;
            int presentCount = 0;
            int absentCount = 0;
            int lateCount = 0;

            await _unitOfWork.BeginTransactionAsync();

            foreach (var studentAttendance in request.Attendances)
            {
                try
                {
                    // Check if already marked
                    var existing = await _unitOfWork.Attendances.FirstOrDefaultAsync(
                        a => a.StudentId == studentAttendance.StudentId &&
                             a.CourseId == request.CourseId &&
                             a.Date.Date == request.Date.Date
                    );

                    if (existing != null)
                    {
                        failedCount++;
                        continue;
                    }

                    var attendance = new Core.Entities.Attendance
                    {
                        StudentId = studentAttendance.StudentId,
                        CourseId = request.CourseId,
                        ScheduleId = request.ScheduleId,
                        Date = request.Date,
                        Status = studentAttendance.Status,
                        Remarks = studentAttendance.Remarks,
                        MarkedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                    };

                    await _unitOfWork.Attendances.AddAsync(attendance);
                    successCount++;

                    // Count by status
                    switch (studentAttendance.Status.ToLower())
                    {
                        case "present": presentCount++; break;
                        case "absent": absentCount++; break;
                        case "late": lateCount++; break;
                    }
                }
                catch
                {
                    failedCount++;
                }
            }

            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation("Bulk attendance marked: {SuccessCount} successful, {FailedCount} failed", successCount, failedCount);

            return Ok(ApiResponse<object>.SuccessResponse(new
            {
                totalRecords = request.Attendances.Count,
                successful = successCount,
                failed = failedCount,
                present = presentCount,
                absent = absentCount,
                late = lateCount
            }, $"Attendance marked for {successCount} students"));
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error in bulk attendance marking");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error marking attendance", 500));
        }
    }

    [HttpGet("report")]
    [ProducesResponseType(typeof(ApiResponse<AttendanceReportDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAttendanceReport([FromQuery] int studentId, [FromQuery] int courseId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        try
        {
            var student = await _unitOfWork.Students.GetByIdWithIncludesAsync(studentId, s => s.User);
            if (student == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Student not found", 404));
            }

            var course = await _unitOfWork.Courses.GetByIdAsync(courseId);
            if (course == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Course not found", 404));
            }

            var attendances = await _unitOfWork.Attendances.FindAsync(
                a => a.StudentId == studentId &&
                     a.CourseId == courseId &&
                     (!startDate.HasValue || a.Date >= startDate.Value) &&
                     (!endDate.HasValue || a.Date <= endDate.Value)
            );

            var attendanceList = attendances.ToList();
            int totalClasses = attendanceList.Count;
            int present = attendanceList.Count(a => a.Status.ToLower() == "present");
            int absent = attendanceList.Count(a => a.Status.ToLower() == "absent" && !a.IsExcused);
            int late = attendanceList.Count(a => a.Status.ToLower() == "late");
            int excused = attendanceList.Count(a => a.IsExcused);

            decimal attendancePercentage = totalClasses > 0
                ? (decimal)(present + late) / totalClasses * 100
                : 0;

            var report = new AttendanceReportDto
            {
                Student = new StudentDto
                {
                    Id = student.Id,
                    StudentNumber = student.StudentNumber,
                    FullName = student.User.FullName,
                    Email = student.User.Email!
                },
                Course = new CourseDto
                {
                    Id = course.Id,
                    CourseCode = course.CourseCode,
                    CourseName = course.CourseName
                },
                TotalClasses = totalClasses,
                Present = present,
                Absent = absent,
                Late = late,
                Excused = excused,
                AttendancePercentage = Math.Round(attendancePercentage, 2),
                AttendanceRecords = attendanceList.Select(a => new AttendanceDto
                {
                    Id = a.Id,
                    StudentId = a.StudentId,
                    CourseId = a.CourseId,
                    Date = a.Date,
                    Status = a.Status,
                    Remarks = a.Remarks,
                    IsExcused = a.IsExcused
                }).ToList()
            };

            return Ok(ApiResponse<AttendanceReportDto>.SuccessResponse(report));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating attendance report");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error generating report", 500));
        }
    }

    [HttpGet("course/{courseId}")]
    [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
    [ProducesResponseType(typeof(ApiResponse<List<AttendanceDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCourseAttendance(int courseId, [FromQuery] DateTime? date)
    {
        try
        {
            var attendances = date.HasValue
                ? await _unitOfWork.Attendances.FindAsync(a => a.CourseId == courseId && a.Date.Date == date.Value.Date)
                : await _unitOfWork.Attendances.FindAsync(a => a.CourseId == courseId);

            var attendanceDtos = new List<AttendanceDto>();
            foreach (var attendance in attendances)
            {
                var student = await _unitOfWork.Students.GetByIdWithIncludesAsync(attendance.StudentId, s => s.User);
                attendanceDtos.Add(new AttendanceDto
                {
                    Id = attendance.Id,
                    StudentId = attendance.StudentId,
                    StudentName = student?.User.FullName ?? "Unknown",
                    CourseId = attendance.CourseId,
                    Date = attendance.Date,
                    Status = attendance.Status,
                    Remarks = attendance.Remarks,
                    IsExcused = attendance.IsExcused
                });
            }

            return Ok(ApiResponse<List<AttendanceDto>>.SuccessResponse(attendanceDtos));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving course attendance");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error retrieving attendance", 500));
        }
    }

    [HttpGet("student/{studentId}")]
    [ProducesResponseType(typeof(ApiResponse<List<AttendanceDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStudentAttendance(int studentId)
    {
        try
        {
            var attendances = await _unitOfWork.Attendances.FindAsync(a => a.StudentId == studentId);

            var attendanceDtos = new List<AttendanceDto>();
            foreach (var attendance in attendances)
            {
                var course = await _unitOfWork.Courses.GetByIdAsync(attendance.CourseId);
                attendanceDtos.Add(new AttendanceDto
                {
                    Id = attendance.Id,
                    StudentId = attendance.StudentId,
                    CourseId = attendance.CourseId,
                    CourseName = course?.CourseName ?? "Unknown",
                    Date = attendance.Date,
                    Status = attendance.Status,
                    Remarks = attendance.Remarks,
                    IsExcused = attendance.IsExcused
                });
            }

            return Ok(ApiResponse<List<AttendanceDto>>.SuccessResponse(attendanceDtos));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving student attendance");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error retrieving attendance", 500));
        }
    }
}
