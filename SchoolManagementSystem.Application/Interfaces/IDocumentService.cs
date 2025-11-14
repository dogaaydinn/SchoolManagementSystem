using SchoolManagementSystem.Core.DTOs;

namespace SchoolManagementSystem.Application.Interfaces;

public interface IDocumentService
{
    Task<ApiResponse<DocumentDto>> UploadDocumentAsync(UploadDocumentRequestDto request, CancellationToken cancellationToken = default);
    Task<ApiResponse<DocumentDto>> GetDocumentByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ApiResponse<PagedResult<DocumentDto>>> GetDocumentsAsync(PagedRequest request, int? entityId = null, string? entityType = null, CancellationToken cancellationToken = default);
    Task<ApiResponse<byte[]>> DownloadDocumentAsync(int id, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> DeleteDocumentAsync(int id, string deletedBy, CancellationToken cancellationToken = default);
    Task<ApiResponse<DocumentDto>> UpdateDocumentMetadataAsync(int id, UpdateDocumentMetadataDto request, CancellationToken cancellationToken = default);
    Task<ApiResponse<IEnumerable<DocumentDto>>> GetStudentDocumentsAsync(int studentId, CancellationToken cancellationToken = default);
    Task<ApiResponse<IEnumerable<DocumentDto>>> GetCourseDocumentsAsync(int courseId, CancellationToken cancellationToken = default);
    Task<ApiResponse<long>> GetStorageUsageAsync(int userId, CancellationToken cancellationToken = default);
}
