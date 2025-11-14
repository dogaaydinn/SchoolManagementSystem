using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Application.DTOs;
using SchoolManagementSystem.Application.Interfaces;

namespace SchoolManagementSystem.API.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class BulkOperationsController : ControllerBase
    {
        private readonly IBulkImportService _bulkImportService;
        private readonly IExcelService _excelService;
        private readonly ILogger<BulkOperationsController> _logger;

        public BulkOperationsController(
            IBulkImportService bulkImportService,
            IExcelService excelService,
            ILogger<BulkOperationsController> logger)
        {
            _bulkImportService = bulkImportService;
            _excelService = excelService;
            _logger = logger;
        }

        /// <summary>
        /// Validate import file before importing
        /// </summary>
        [HttpPost("validate/{importType}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> ValidateImportFile([FromRoute] string importType, IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse("No file uploaded", 400));
            }

            using var stream = file.OpenReadStream();
            var result = await _bulkImportService.ValidateImportFileAsync(stream, file.FileName, importType, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Import students from CSV or Excel file
        /// </summary>
        [HttpPost("import/students")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> ImportStudents(IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse("No file uploaded", 400));
            }

            if (!file.FileName.EndsWith(".csv") && !file.FileName.EndsWith(".xlsx"))
            {
                return BadRequest(ApiResponse<string>.ErrorResponse("Only CSV and Excel files are supported", 400));
            }

            _logger.LogInformation("Starting student import from file: {FileName}", file.FileName);

            using var stream = file.OpenReadStream();
            var result = await _bulkImportService.ImportStudentsAsync(stream, file.FileName, cancellationToken);

            _logger.LogInformation("Student import completed: {Success} successful, {Failed} failed",
                result.Data?.SuccessfulRows, result.Data?.FailedRows);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Import courses from CSV or Excel file
        /// </summary>
        [HttpPost("import/courses")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> ImportCourses(IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse("No file uploaded", 400));
            }

            if (!file.FileName.EndsWith(".csv") && !file.FileName.EndsWith(".xlsx"))
            {
                return BadRequest(ApiResponse<string>.ErrorResponse("Only CSV and Excel files are supported", 400));
            }

            _logger.LogInformation("Starting course import from file: {FileName}", file.FileName);

            using var stream = file.OpenReadStream();
            var result = await _bulkImportService.ImportCoursesAsync(stream, file.FileName, cancellationToken);

            _logger.LogInformation("Course import completed: {Success} successful, {Failed} failed",
                result.Data?.SuccessfulRows, result.Data?.FailedRows);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Import grades from CSV or Excel file
        /// </summary>
        [HttpPost("import/grades")]
        [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
        public async Task<IActionResult> ImportGrades(IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse("No file uploaded", 400));
            }

            if (!file.FileName.EndsWith(".csv") && !file.FileName.EndsWith(".xlsx"))
            {
                return BadRequest(ApiResponse<string>.ErrorResponse("Only CSV and Excel files are supported", 400));
            }

            _logger.LogInformation("Starting grades import from file: {FileName}", file.FileName);

            using var stream = file.OpenReadStream();
            var result = await _bulkImportService.ImportGradesAsync(stream, file.FileName, cancellationToken);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Import enrollments from CSV or Excel file
        /// </summary>
        [HttpPost("import/enrollments")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> ImportEnrollments(IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse("No file uploaded", 400));
            }

            if (!file.FileName.EndsWith(".csv") && !file.FileName.EndsWith(".xlsx"))
            {
                return BadRequest(ApiResponse<string>.ErrorResponse("Only CSV and Excel files are supported", 400));
            }

            _logger.LogInformation("Starting enrollments import from file: {FileName}", file.FileName);

            using var stream = file.OpenReadStream();
            var result = await _bulkImportService.ImportEnrollmentsAsync(stream, file.FileName, cancellationToken);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Download import template for students
        /// </summary>
        [HttpGet("template/students")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public IActionResult GetStudentImportTemplate()
        {
            var headers = new[] { "FirstName", "LastName", "Email", "PhoneNumber", "DateOfBirth", "EnrollmentDate", "Address", "City", "State", "ZipCode" };
            var csvContent = string.Join(",", headers);

            var bytes = System.Text.Encoding.UTF8.GetBytes(csvContent);
            return File(bytes, "text/csv", "student_import_template.csv");
        }

        /// <summary>
        /// Download import template for courses
        /// </summary>
        [HttpGet("template/courses")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public IActionResult GetCourseImportTemplate()
        {
            var headers = new[] { "CourseCode", "Title", "Description", "Credits", "MaxStudents" };
            var csvContent = string.Join(",", headers);

            var bytes = System.Text.Encoding.UTF8.GetBytes(csvContent);
            return File(bytes, "text/csv", "course_import_template.csv");
        }

        /// <summary>
        /// Download import template for grades
        /// </summary>
        [HttpGet("template/grades")]
        [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
        public IActionResult GetGradeImportTemplate()
        {
            var headers = new[] { "StudentEmail", "CourseCode", "AssignmentTitle", "Value", "MaxValue" };
            var csvContent = string.Join(",", headers);

            var bytes = System.Text.Encoding.UTF8.GetBytes(csvContent);
            return File(bytes, "text/csv", "grade_import_template.csv");
        }

        /// <summary>
        /// Download import template for enrollments
        /// </summary>
        [HttpGet("template/enrollments")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public IActionResult GetEnrollmentImportTemplate()
        {
            var headers = new[] { "StudentEmail", "CourseCode", "EnrollmentDate" };
            var csvContent = string.Join(",", headers);

            var bytes = System.Text.Encoding.UTF8.GetBytes(csvContent);
            return File(bytes, "text/csv", "enrollment_import_template.csv");
        }
    }
}
