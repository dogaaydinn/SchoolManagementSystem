using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Application.Interfaces;
using SchoolManagementSystem.Core.DTOs;

namespace SchoolManagementSystem.API.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IReportingService _reportingService;
    private readonly IPdfService _pdfService;
    private readonly IExcelService _excelService;
    private readonly ILogger<ReportsController> _logger;

    public ReportsController(
        IReportingService reportingService,
        IPdfService pdfService,
        IExcelService excelService,
        ILogger<ReportsController> logger)
    {
        _reportingService = reportingService;
        _pdfService = pdfService;
        _excelService = excelService;
        _logger = logger;
    }

    /// <summary>
    /// Get student performance report
    /// </summary>
    [HttpGet("student/{studentId}/performance")]
    [Authorize(Roles = "SuperAdmin,Admin,Teacher,Student")]
    public async Task<IActionResult> GetStudentPerformanceReport(int studentId, [FromQuery] int? semesterId)
    {
        var result = await _reportingService.GetStudentPerformanceReportAsync(studentId, semesterId);

        if (!result.Success)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }

    /// <summary>
    /// Export student performance report to PDF
    /// </summary>
    [HttpGet("student/{studentId}/performance/pdf")]
    [Authorize(Roles = "SuperAdmin,Admin,Teacher,Student")]
    public async Task<IActionResult> ExportStudentReportToPdf(int studentId, [FromQuery] int? semesterId)
    {
        var reportResult = await _reportingService.GetStudentPerformanceReportAsync(studentId, semesterId);

        if (!reportResult.Success || reportResult.Data == null)
            return StatusCode(reportResult.StatusCode, reportResult);

        var pdfBytes = await _pdfService.GenerateStudentReportPdfAsync(reportResult.Data);

        return File(pdfBytes, "application/pdf", $"student_report_{studentId}.pdf");
    }

    /// <summary>
    /// Get course performance report
    /// </summary>
    [HttpGet("course/{courseId}/performance")]
    [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
    public async Task<IActionResult> GetCoursePerformanceReport(int courseId)
    {
        var result = await _reportingService.GetCoursePerformanceReportAsync(courseId);

        if (!result.Success)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }

    /// <summary>
    /// Export course performance report to PDF
    /// </summary>
    [HttpGet("course/{courseId}/performance/pdf")]
    [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
    public async Task<IActionResult> ExportCourseReportToPdf(int courseId)
    {
        var reportResult = await _reportingService.GetCoursePerformanceReportAsync(courseId);

        if (!reportResult.Success || reportResult.Data == null)
            return StatusCode(reportResult.StatusCode, reportResult);

        var pdfBytes = await _pdfService.GenerateCourseReportPdfAsync(reportResult.Data);

        return File(pdfBytes, "application/pdf", $"course_report_{courseId}.pdf");
    }

    /// <summary>
    /// Get teacher performance report
    /// </summary>
    [HttpGet("teacher/{teacherId}/performance")]
    [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
    public async Task<IActionResult> GetTeacherPerformanceReport(int teacherId, [FromQuery] int? semesterId)
    {
        var result = await _reportingService.GetTeacherPerformanceReportAsync(teacherId, semesterId);

        if (!result.Success)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }

    /// <summary>
    /// Get department report
    /// </summary>
    [HttpGet("department/{departmentId}")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> GetDepartmentReport(int departmentId)
    {
        var result = await _reportingService.GetDepartmentReportAsync(departmentId);

        if (!result.Success)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }

    /// <summary>
    /// Get attendance report
    /// </summary>
    [HttpGet("attendance")]
    [Authorize(Roles = "SuperAdmin,Admin,Teacher,Student")]
    public async Task<IActionResult> GetAttendanceReport([FromQuery] int studentId, [FromQuery] int? courseId)
    {
        var result = await _reportingService.GetAttendanceReportAsync(studentId, courseId);

        if (!result.Success)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }

    /// <summary>
    /// Export attendance report to PDF
    /// </summary>
    [HttpGet("attendance/pdf")]
    [Authorize(Roles = "SuperAdmin,Admin,Teacher,Student")]
    public async Task<IActionResult> ExportAttendanceReportToPdf([FromQuery] int studentId, [FromQuery] int? courseId)
    {
        var reportResult = await _reportingService.GetAttendanceReportAsync(studentId, courseId);

        if (!reportResult.Success || reportResult.Data == null)
            return StatusCode(reportResult.StatusCode, reportResult);

        var pdfBytes = await _pdfService.GenerateAttendanceReportPdfAsync(reportResult.Data);

        return File(pdfBytes, "application/pdf", $"attendance_report_{studentId}.pdf");
    }

    /// <summary>
    /// Get enrollment report
    /// </summary>
    [HttpGet("enrollment")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> GetEnrollmentReport([FromQuery] int? departmentId, [FromQuery] int? semesterId)
    {
        var result = await _reportingService.GetEnrollmentReportAsync(departmentId, semesterId);

        if (!result.Success)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }

    /// <summary>
    /// Export students to Excel
    /// </summary>
    [HttpPost("export/students/excel")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> ExportStudentsToExcel([FromBody] List<StudentListDto> students)
    {
        var excelBytes = await _excelService.ExportStudentsToExcelAsync(students);

        return File(excelBytes, "text/csv", $"students_{DateTime.UtcNow:yyyyMMdd}.csv");
    }

    /// <summary>
    /// Export grades to Excel
    /// </summary>
    [HttpPost("export/grades/excel")]
    [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
    public async Task<IActionResult> ExportGradesToExcel([FromBody] List<GradeDto> grades)
    {
        var excelBytes = await _excelService.ExportGradesToExcelAsync(grades);

        return File(excelBytes, "text/csv", $"grades_{DateTime.UtcNow:yyyyMMdd}.csv");
    }

    /// <summary>
    /// Export attendance to Excel
    /// </summary>
    [HttpPost("export/attendance/excel")]
    [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
    public async Task<IActionResult> ExportAttendanceToExcel([FromBody] List<AttendanceDto> attendances)
    {
        var excelBytes = await _excelService.ExportAttendanceToExcelAsync(attendances);

        return File(excelBytes, "text/csv", $"attendance_{DateTime.UtcNow:yyyyMMdd}.csv");
    }

    /// <summary>
    /// Import students from Excel
    /// </summary>
    [HttpPost("import/students/excel")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> ImportStudentsFromExcel([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(ApiResponse<object>.ErrorResponse("No file provided", 400));

        using var stream = file.OpenReadStream();
        var result = await _excelService.ImportStudentsFromExcelAsync(stream);

        if (!result.Success)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }

    /// <summary>
    /// Import grades from Excel
    /// </summary>
    [HttpPost("import/grades/excel")]
    [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
    public async Task<IActionResult> ImportGradesFromExcel([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(ApiResponse<object>.ErrorResponse("No file provided", 400));

        using var stream = file.OpenReadStream();
        var result = await _excelService.ImportGradesFromExcelAsync(stream);

        if (!result.Success)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }

    /// <summary>
    /// Generate course grade sheet PDF
    /// </summary>
    [HttpGet("course/{courseId}/gradesheet/pdf")]
    [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
    public async Task<IActionResult> GenerateGradeSheetPdf(int courseId)
    {
        var pdfBytes = await _pdfService.GenerateGradeSheetPdfAsync(courseId);

        return File(pdfBytes, "application/pdf", $"gradesheet_{courseId}.pdf");
    }

    /// <summary>
    /// Generate student certificate
    /// </summary>
    [HttpGet("student/{studentId}/certificate/{certificateType}")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> GenerateCertificate(int studentId, string certificateType)
    {
        var pdfBytes = await _pdfService.GenerateCertificatePdfAsync(studentId, certificateType);

        return File(pdfBytes, "application/pdf", $"{certificateType}_{studentId}.pdf");
    }
}
