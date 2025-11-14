using AutoMapper;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Application.Interfaces;
using SchoolManagementSystem.Core.DTOs;
using SchoolManagementSystem.Core.Entities;
using SchoolManagementSystem.Core.Interfaces;

namespace SchoolManagementSystem.Application.Services;

public class DocumentService : IDocumentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<DocumentService> _logger;
    private readonly IFileStorageService _fileStorageService;

    public DocumentService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<DocumentService> logger,
        IFileStorageService fileStorageService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _fileStorageService = fileStorageService;
    }

    public async Task<ApiResponse<DocumentDto>> UploadDocumentAsync(
        UploadDocumentRequestDto request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate file size (max 50MB)
            const long maxFileSize = 50 * 1024 * 1024;
            if (request.FileContent.Length > maxFileSize)
                return ApiResponse<DocumentDto>.ErrorResponse("File size exceeds maximum limit of 50MB", 400);

            // Validate file extension
            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt", ".jpg", ".jpeg", ".png", ".zip" };
            var fileExtension = Path.GetExtension(request.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
                return ApiResponse<DocumentDto>.ErrorResponse("File type not allowed", 400);

            // Generate unique file name
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var uploadDate = DateTime.UtcNow;
            var folderPath = $"{request.EntityType}/{uploadDate.Year}/{uploadDate.Month:D2}";
            var fullPath = Path.Combine(folderPath, uniqueFileName);

            // Upload file to storage
            using var fileStream = new MemoryStream(request.FileContent);
            var filePath = await _fileStorageService.UploadFileAsync(fileStream, fullPath, request.ContentType, cancellationToken);

            // Create document record
            var document = new Document
            {
                FileName = uniqueFileName,
                OriginalFileName = request.FileName,
                FileExtension = fileExtension,
                FileSize = request.FileContent.Length,
                FilePath = filePath,
                ContentType = request.ContentType,
                DocumentType = request.DocumentType,
                EntityType = request.EntityType,
                EntityId = request.EntityId,
                Description = request.Description,
                Tags = request.Tags,
                IsPublic = request.IsPublic,
                Version = 1,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Documents.AddAsync(document, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Document uploaded: {FileName} ({FileSize} bytes) for {EntityType} {EntityId}",
                request.FileName,
                request.FileContent.Length,
                request.EntityType,
                request.EntityId
            );

            var documentDto = _mapper.Map<DocumentDto>(document);
            documentDto.FileUrl = _fileStorageService.GetFileUrl(filePath);

            return ApiResponse<DocumentDto>.SuccessResponse(documentDto, "Document uploaded successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading document {FileName}", request.FileName);
            return ApiResponse<DocumentDto>.ErrorResponse("Error uploading document", 500);
        }
    }

    public async Task<ApiResponse<DocumentDto>> GetDocumentByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var document = await _unitOfWork.Documents.GetByIdAsync(id, cancellationToken);
            if (document == null)
                return ApiResponse<DocumentDto>.ErrorResponse("Document not found", 404);

            var documentDto = _mapper.Map<DocumentDto>(document);
            documentDto.FileUrl = _fileStorageService.GetFileUrl(document.FilePath);

            return ApiResponse<DocumentDto>.SuccessResponse(documentDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting document {DocumentId}", id);
            return ApiResponse<DocumentDto>.ErrorResponse("Error retrieving document", 500);
        }
    }

    public async Task<ApiResponse<PagedResult<DocumentDto>>> GetDocumentsAsync(
        PagedRequest request,
        int? entityId = null,
        string? entityType = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var (documents, totalCount) = await _unitOfWork.Documents.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                filter: d => (!entityId.HasValue || d.EntityId == entityId) &&
                            (string.IsNullOrEmpty(entityType) || d.EntityType == entityType) &&
                            (string.IsNullOrEmpty(request.SearchTerm) ||
                             d.OriginalFileName.Contains(request.SearchTerm) ||
                             d.Description != null && d.Description.Contains(request.SearchTerm)),
                orderBy: query => request.SortOrder?.ToLower() == "desc"
                    ? query.OrderByDescending(d => d.CreatedAt)
                    : query.OrderBy(d => d.CreatedAt)
            );

            var documentDtos = documents.Select(d =>
            {
                var dto = _mapper.Map<DocumentDto>(d);
                dto.FileUrl = _fileStorageService.GetFileUrl(d.FilePath);
                return dto;
            }).ToList();

            var pagedResult = new PagedResult<DocumentDto>
            {
                Items = documentDtos,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };

            return ApiResponse<PagedResult<DocumentDto>>.SuccessResponse(pagedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting documents");
            return ApiResponse<PagedResult<DocumentDto>>.ErrorResponse("Error retrieving documents", 500);
        }
    }

    public async Task<ApiResponse<byte[]>> DownloadDocumentAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var document = await _unitOfWork.Documents.GetByIdAsync(id, cancellationToken);
            if (document == null)
                return ApiResponse<byte[]>.ErrorResponse("Document not found", 404);

            var fileContent = await _fileStorageService.DownloadFileAsync(document.FilePath, cancellationToken);

            _logger.LogInformation("Document downloaded: {DocumentId} - {FileName}", id, document.OriginalFileName);

            return ApiResponse<byte[]>.SuccessResponse(fileContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading document {DocumentId}", id);
            return ApiResponse<byte[]>.ErrorResponse("Error downloading document", 500);
        }
    }

    public async Task<ApiResponse<bool>> DeleteDocumentAsync(
        int id,
        string deletedBy,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var document = await _unitOfWork.Documents.GetByIdAsync(id, cancellationToken);
            if (document == null)
                return ApiResponse<bool>.ErrorResponse("Document not found", 404);

            // Delete file from storage
            await _fileStorageService.DeleteFileAsync(document.FilePath, cancellationToken);

            // Soft delete document record
            await _unitOfWork.Documents.SoftDeleteAsync(id, deletedBy, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Document deleted: {DocumentId} by {DeletedBy}", id, deletedBy);

            return ApiResponse<bool>.SuccessResponse(true, "Document deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting document {DocumentId}", id);
            return ApiResponse<bool>.ErrorResponse("Error deleting document", 500);
        }
    }

    public async Task<ApiResponse<DocumentDto>> UpdateDocumentMetadataAsync(
        int id,
        UpdateDocumentMetadataDto request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var document = await _unitOfWork.Documents.GetByIdAsync(id, cancellationToken);
            if (document == null)
                return ApiResponse<DocumentDto>.ErrorResponse("Document not found", 404);

            if (!string.IsNullOrEmpty(request.Description))
                document.Description = request.Description;
            if (!string.IsNullOrEmpty(request.Tags))
                document.Tags = request.Tags;
            if (request.IsPublic.HasValue)
                document.IsPublic = request.IsPublic.Value;
            if (!string.IsNullOrEmpty(request.DocumentType))
                document.DocumentType = request.DocumentType;

            document.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Documents.Update(document);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var documentDto = _mapper.Map<DocumentDto>(document);
            documentDto.FileUrl = _fileStorageService.GetFileUrl(document.FilePath);

            return ApiResponse<DocumentDto>.SuccessResponse(documentDto, "Document metadata updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating document metadata {DocumentId}", id);
            return ApiResponse<DocumentDto>.ErrorResponse("Error updating document", 500);
        }
    }

    public async Task<ApiResponse<IEnumerable<DocumentDto>>> GetStudentDocumentsAsync(
        int studentId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var documents = await _unitOfWork.Documents.FindAsync(
                d => d.EntityType == "Student" && d.EntityId == studentId,
                cancellationToken
            );

            var documentDtos = documents.Select(d =>
            {
                var dto = _mapper.Map<DocumentDto>(d);
                dto.FileUrl = _fileStorageService.GetFileUrl(d.FilePath);
                return dto;
            }).ToList();

            return ApiResponse<IEnumerable<DocumentDto>>.SuccessResponse(documentDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting documents for student {StudentId}", studentId);
            return ApiResponse<IEnumerable<DocumentDto>>.ErrorResponse("Error retrieving student documents", 500);
        }
    }

    public async Task<ApiResponse<IEnumerable<DocumentDto>>> GetCourseDocumentsAsync(
        int courseId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var documents = await _unitOfWork.Documents.FindAsync(
                d => d.EntityType == "Course" && d.EntityId == courseId,
                cancellationToken
            );

            var documentDtos = documents.Select(d =>
            {
                var dto = _mapper.Map<DocumentDto>(d);
                dto.FileUrl = _fileStorageService.GetFileUrl(d.FilePath);
                return dto;
            }).ToList();

            return ApiResponse<IEnumerable<DocumentDto>>.SuccessResponse(documentDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting documents for course {CourseId}", courseId);
            return ApiResponse<IEnumerable<DocumentDto>>.ErrorResponse("Error retrieving course documents", 500);
        }
    }

    public async Task<ApiResponse<long>> GetStorageUsageAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var documents = await _unitOfWork.Documents.FindAsync(
                d => d.UploadedBy == userId,
                cancellationToken
            );

            var totalSize = documents.Sum(d => d.FileSize);

            return ApiResponse<long>.SuccessResponse(totalSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating storage usage for user {UserId}", userId);
            return ApiResponse<long>.ErrorResponse("Error calculating storage usage", 500);
        }
    }
}
