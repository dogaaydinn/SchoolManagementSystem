using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Application.Interfaces;
using SchoolManagementSystem.Core.DTOs;
using System.Security.Claims;

namespace SchoolManagementSystem.API.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentService _documentService;
    private readonly ILogger<DocumentsController> _logger;

    public DocumentsController(
        IDocumentService documentService,
        ILogger<DocumentsController> logger)
    {
        _documentService = documentService;
        _logger = logger;
    }

    /// <summary>
    /// Upload a new document
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin,Teacher,Student")]
    public async Task<IActionResult> UploadDocument([FromForm] IFormFile file, [FromForm] string documentType, [FromForm] string entityType, [FromForm] int? entityId, [FromForm] string? description, [FromForm] string? tags, [FromForm] bool isPublic = false)
    {
        if (file == null || file.Length == 0)
            return BadRequest(ApiResponse<object>.ErrorResponse("No file provided", 400));

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);

        var request = new UploadDocumentRequestDto
        {
            FileName = file.FileName,
            ContentType = file.ContentType,
            FileContent = memoryStream.ToArray(),
            DocumentType = documentType,
            EntityType = entityType,
            EntityId = entityId,
            Description = description,
            Tags = tags,
            IsPublic = isPublic
        };

        var result = await _documentService.UploadDocumentAsync(request);

        if (!result.Success)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }

    /// <summary>
    /// Get document by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDocument(int id)
    {
        var result = await _documentService.GetDocumentByIdAsync(id);

        if (!result.Success)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }

    /// <summary>
    /// Get paginated list of documents
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetDocuments([FromQuery] PagedRequest request, [FromQuery] int? entityId, [FromQuery] string? entityType)
    {
        var result = await _documentService.GetDocumentsAsync(request, entityId, entityType);

        if (!result.Success)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }

    /// <summary>
    /// Download document
    /// </summary>
    [HttpGet("{id}/download")]
    public async Task<IActionResult> DownloadDocument(int id)
    {
        var documentResult = await _documentService.GetDocumentByIdAsync(id);
        if (!documentResult.Success || documentResult.Data == null)
            return NotFound(documentResult);

        var downloadResult = await _documentService.DownloadDocumentAsync(id);
        if (!downloadResult.Success || downloadResult.Data == null)
            return StatusCode(downloadResult.StatusCode, downloadResult);

        return File(downloadResult.Data, documentResult.Data.ContentType, documentResult.Data.OriginalFileName);
    }

    /// <summary>
    /// Update document metadata
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
    public async Task<IActionResult> UpdateDocumentMetadata(int id, [FromBody] UpdateDocumentMetadataDto request)
    {
        var result = await _documentService.UpdateDocumentMetadataAsync(id, request);

        if (!result.Success)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }

    /// <summary>
    /// Delete document
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> DeleteDocument(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";

        var result = await _documentService.DeleteDocumentAsync(id, userId);

        if (!result.Success)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }

    /// <summary>
    /// Get student documents
    /// </summary>
    [HttpGet("student/{studentId}")]
    [Authorize(Roles = "SuperAdmin,Admin,Teacher,Student")]
    public async Task<IActionResult> GetStudentDocuments(int studentId)
    {
        // Students can only access their own documents
        if (User.IsInRole("Student"))
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // Would need to verify studentId matches current user
        }

        var result = await _documentService.GetStudentDocumentsAsync(studentId);

        if (!result.Success)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }

    /// <summary>
    /// Get course documents
    /// </summary>
    [HttpGet("course/{courseId}")]
    public async Task<IActionResult> GetCourseDocuments(int courseId)
    {
        var result = await _documentService.GetCourseDocumentsAsync(courseId);

        if (!result.Success)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }

    /// <summary>
    /// Get storage usage for current user
    /// </summary>
    [HttpGet("storage/usage")]
    public async Task<IActionResult> GetStorageUsage()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var result = await _documentService.GetStorageUsageAsync(userId);

        if (!result.Success)
            return StatusCode(result.StatusCode, result);

        return Ok(new
        {
            userId,
            storageUsedBytes = result.Data,
            storageUsedMB = result.Data / (1024.0 * 1024.0),
            maxStorageGB = 5.0
        });
    }
}
