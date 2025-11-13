using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Core.DTOs;
using SchoolManagementSystem.Core.Interfaces;

namespace SchoolManagementSystem.API.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
public class SchedulesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SchedulesController> _logger;

    public SchedulesController(IUnitOfWork unitOfWork, ILogger<SchedulesController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<ScheduleDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSchedules([FromQuery] int? courseId, [FromQuery] int? teacherId)
    {
        try
        {
            var schedules = await (courseId.HasValue
                ? _unitOfWork.Schedules.FindAsync(s => s.CourseId == courseId.Value)
                : teacherId.HasValue
                    ? _unitOfWork.Schedules.FindAsync(s => s.TeacherId == teacherId.Value)
                    : _unitOfWork.Schedules.GetAllAsync());

            var scheduleDtos = new List<ScheduleDto>();
            foreach (var schedule in schedules)
            {
                var course = await _unitOfWork.Courses.GetByIdAsync(schedule.CourseId);
                var teacher = await _unitOfWork.Teachers.GetByIdWithIncludesAsync(schedule.TeacherId, t => t.User);

                scheduleDtos.Add(new ScheduleDto
                {
                    Id = schedule.Id,
                    CourseId = schedule.CourseId,
                    CourseName = course?.CourseName ?? "Unknown",
                    CourseCode = course?.CourseCode ?? "Unknown",
                    TeacherId = schedule.TeacherId,
                    TeacherName = teacher?.User.FullName ?? "Unknown",
                    DayOfWeek = schedule.DayOfWeek,
                    StartTime = schedule.StartTime,
                    EndTime = schedule.EndTime,
                    Room = schedule.Room,
                    Building = schedule.Building,
                    Type = schedule.Type
                });
            }

            return Ok(ApiResponse<List<ScheduleDto>>.SuccessResponse(scheduleDtos));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving schedules");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error retrieving schedules", 500));
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ScheduleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSchedule(int id)
    {
        try
        {
            var schedule = await _unitOfWork.Schedules.GetByIdAsync(id);
            if (schedule == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Schedule not found", 404));
            }

            var course = await _unitOfWork.Courses.GetByIdAsync(schedule.CourseId);
            var teacher = await _unitOfWork.Teachers.GetByIdWithIncludesAsync(schedule.TeacherId, t => t.User);

            var scheduleDto = new ScheduleDto
            {
                Id = schedule.Id,
                CourseId = schedule.CourseId,
                CourseName = course?.CourseName ?? "Unknown",
                CourseCode = course?.CourseCode ?? "Unknown",
                TeacherId = schedule.TeacherId,
                TeacherName = teacher?.User.FullName ?? "Unknown",
                DayOfWeek = schedule.DayOfWeek,
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime,
                Room = schedule.Room,
                Building = schedule.Building,
                Type = schedule.Type
            };

            return Ok(ApiResponse<ScheduleDto>.SuccessResponse(scheduleDto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving schedule {ScheduleId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error retrieving schedule", 500));
        }
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin")]
    [ProducesResponseType(typeof(ApiResponse<ScheduleDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateSchedule([FromBody] CreateScheduleRequestDto request)
    {
        try
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);
            if (course == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Course not found", 404));
            }

            var teacher = await _unitOfWork.Teachers.GetByIdAsync(request.TeacherId);
            if (teacher == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Teacher not found", 404));
            }

            // Check for schedule conflicts
            var conflicts = await _unitOfWork.Schedules.FindAsync(
                s => s.TeacherId == request.TeacherId &&
                     s.DayOfWeek == request.DayOfWeek &&
                     ((s.StartTime <= request.StartTime && s.EndTime > request.StartTime) ||
                      (s.StartTime < request.EndTime && s.EndTime >= request.EndTime) ||
                      (s.StartTime >= request.StartTime && s.EndTime <= request.EndTime))
            );

            if (conflicts.Any())
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Schedule conflict detected for this teacher", 400));
            }

            var schedule = new Core.Entities.Schedule
            {
                CourseId = request.CourseId,
                TeacherId = request.TeacherId,
                SemesterId = request.SemesterId,
                DayOfWeek = request.DayOfWeek,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Room = request.Room,
                Building = request.Building,
                Type = request.Type,
                IsRecurring = request.IsRecurring
            };

            await _unitOfWork.Schedules.AddAsync(schedule);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Schedule created for course {CourseId}", request.CourseId);

            var scheduleDto = new ScheduleDto
            {
                Id = schedule.Id,
                CourseId = schedule.CourseId,
                CourseName = course.CourseName,
                CourseCode = course.CourseCode,
                DayOfWeek = schedule.DayOfWeek,
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime,
                Room = schedule.Room,
                Building = schedule.Building
            };

            return CreatedAtAction(nameof(GetSchedule), new { id = schedule.Id },
                ApiResponse<ScheduleDto>.SuccessResponse(scheduleDto, "Schedule created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating schedule");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error creating schedule", 500));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    [ProducesResponseType(typeof(ApiResponse<ScheduleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateSchedule(int id, [FromBody] CreateScheduleRequestDto request)
    {
        try
        {
            var schedule = await _unitOfWork.Schedules.GetByIdAsync(id);
            if (schedule == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Schedule not found", 404));
            }

            schedule.DayOfWeek = request.DayOfWeek;
            schedule.StartTime = request.StartTime;
            schedule.EndTime = request.EndTime;
            schedule.Room = request.Room;
            schedule.Building = request.Building;
            schedule.Type = request.Type;

            _unitOfWork.Schedules.Update(schedule);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Schedule updated: {ScheduleId}", id);

            return Ok(ApiResponse<ScheduleDto>.SuccessResponse(new ScheduleDto
            {
                Id = schedule.Id,
                DayOfWeek = schedule.DayOfWeek,
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime
            }, "Schedule updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating schedule {ScheduleId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error updating schedule", 500));
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteSchedule(int id)
    {
        try
        {
            var schedule = await _unitOfWork.Schedules.GetByIdAsync(id);
            if (schedule == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Schedule not found", 404));
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            await _unitOfWork.Schedules.SoftDeleteAsync(id, userId ?? "System");
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Schedule deleted: {ScheduleId}", id);

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Schedule deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting schedule {ScheduleId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error deleting schedule", 500));
        }
    }

    [HttpGet("student/{studentId}/timetable")]
    [ProducesResponseType(typeof(ApiResponse<TimetableDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStudentTimetable(int studentId)
    {
        try
        {
            var student = await _unitOfWork.Students.GetByIdWithIncludesAsync(studentId, s => s.User);
            if (student == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Student not found", 404));
            }

            // Get student's active enrollments
            var enrollments = await _unitOfWork.Enrollments.FindAsync(
                e => e.StudentId == studentId && e.Status == "Active"
            );

            var courseIds = enrollments.Select(e => e.CourseId).ToList();
            var schedules = new List<Core.Entities.Schedule>();

            foreach (var courseId in courseIds)
            {
                var courseSchedules = await _unitOfWork.Schedules.FindAsync(s => s.CourseId == courseId);
                schedules.AddRange(courseSchedules);
            }

            // Group schedules by day of week
            var dailySchedules = schedules
                .GroupBy(s => s.DayOfWeek)
                .Select(g => new DayScheduleDto
                {
                    DayOfWeek = g.Key,
                    Classes = g.Select(s => new ScheduleDto
                    {
                        Id = s.Id,
                        CourseId = s.CourseId,
                        DayOfWeek = s.DayOfWeek,
                        StartTime = s.StartTime,
                        EndTime = s.EndTime,
                        Room = s.Room,
                        Building = s.Building,
                        Type = s.Type
                    }).OrderBy(s => s.StartTime).ToList()
                }).ToList();

            var timetable = new TimetableDto
            {
                EntityType = "Student",
                EntityId = studentId,
                EntityName = student.User.FullName,
                DailySchedules = dailySchedules
            };

            return Ok(ApiResponse<TimetableDto>.SuccessResponse(timetable));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving student timetable");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error retrieving timetable", 500));
        }
    }

    [HttpGet("teacher/{teacherId}/timetable")]
    [ProducesResponseType(typeof(ApiResponse<TimetableDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTeacherTimetable(int teacherId)
    {
        try
        {
            var teacher = await _unitOfWork.Teachers.GetByIdWithIncludesAsync(teacherId, t => t.User);
            if (teacher == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Teacher not found", 404));
            }

            var schedules = await _unitOfWork.Schedules.FindAsync(s => s.TeacherId == teacherId);

            var dailySchedules = schedules
                .GroupBy(s => s.DayOfWeek)
                .Select(g => new DayScheduleDto
                {
                    DayOfWeek = g.Key,
                    Classes = g.Select(s => new ScheduleDto
                    {
                        Id = s.Id,
                        CourseId = s.CourseId,
                        DayOfWeek = s.DayOfWeek,
                        StartTime = s.StartTime,
                        EndTime = s.EndTime,
                        Room = s.Room,
                        Building = s.Building,
                        Type = s.Type
                    }).OrderBy(s => s.StartTime).ToList()
                }).ToList();

            var timetable = new TimetableDto
            {
                EntityType = "Teacher",
                EntityId = teacherId,
                EntityName = teacher.User.FullName,
                DailySchedules = dailySchedules
            };

            return Ok(ApiResponse<TimetableDto>.SuccessResponse(timetable));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving teacher timetable");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error retrieving timetable", 500));
        }
    }
}
