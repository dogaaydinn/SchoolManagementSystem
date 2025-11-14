using SchoolManagementSystem.Application.DTOs;

namespace SchoolManagementSystem.Application.Interfaces
{
    public interface IBulkImportService
    {
        // Import students from CSV/Excel
        Task<ApiResponse<BulkImportResultDto>> ImportStudentsAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default);

        // Import courses from CSV/Excel
        Task<ApiResponse<BulkImportResultDto>> ImportCoursesAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default);

        // Import grades from CSV/Excel
        Task<ApiResponse<BulkImportResultDto>> ImportGradesAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default);

        // Import enrollments from CSV/Excel
        Task<ApiResponse<BulkImportResultDto>> ImportEnrollmentsAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default);

        // Validate import file without importing
        Task<ApiResponse<BulkImportValidationDto>> ValidateImportFileAsync(Stream fileStream, string fileName, string importType, CancellationToken cancellationToken = default);
    }

    public class BulkImportValidationDto
    {
        public bool IsValid { get; set; }
        public int TotalRows { get; set; }
        public int ValidRows { get; set; }
        public int InvalidRows { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
        public string[] ExpectedColumns { get; set; } = Array.Empty<string>();
        public string[] ActualColumns { get; set; } = Array.Empty<string>();
    }
}
