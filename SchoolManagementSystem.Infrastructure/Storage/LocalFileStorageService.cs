using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Application.Interfaces;

namespace SchoolManagementSystem.Infrastructure.Storage;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _storageRootPath;
    private readonly string _baseUrl;
    private readonly ILogger<LocalFileStorageService> _logger;

    public LocalFileStorageService(
        IConfiguration configuration,
        ILogger<LocalFileStorageService> logger)
    {
        _logger = logger;
        _storageRootPath = configuration["FileStorage:RootPath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        _baseUrl = configuration["FileStorage:BaseUrl"] ?? "/files";

        // Ensure storage directory exists
        if (!Directory.Exists(_storageRootPath))
        {
            Directory.CreateDirectory(_storageRootPath);
            _logger.LogInformation("Created storage directory: {Path}", _storageRootPath);
        }
    }

    public async Task<string> UploadFileAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var fullPath = Path.Combine(_storageRootPath, fileName);
            var directory = Path.GetDirectoryName(fullPath);

            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (var fileStreamOutput = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
            {
                await fileStream.CopyToAsync(fileStreamOutput, cancellationToken);
            }

            _logger.LogInformation("File uploaded successfully: {FileName}", fileName);
            return fileName;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file {FileName}", fileName);
            throw;
        }
    }

    public async Task<byte[]> DownloadFileAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var fullPath = Path.Combine(_storageRootPath, filePath);

            if (!File.Exists(fullPath))
                throw new FileNotFoundException($"File not found: {filePath}");

            return await File.ReadAllBytesAsync(fullPath, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading file {FilePath}", filePath);
            throw;
        }
    }

    public async Task<bool> DeleteFileAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var fullPath = Path.Combine(_storageRootPath, filePath);

            if (File.Exists(fullPath))
            {
                await Task.Run(() => File.Delete(fullPath), cancellationToken);
                _logger.LogInformation("File deleted: {FilePath}", filePath);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file {FilePath}", filePath);
            return false;
        }
    }

    public async Task<bool> FileExistsAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_storageRootPath, filePath);
        return await Task.FromResult(File.Exists(fullPath));
    }

    public async Task<long> GetFileSizeAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_storageRootPath, filePath);

        if (!File.Exists(fullPath))
            return 0;

        var fileInfo = new FileInfo(fullPath);
        return await Task.FromResult(fileInfo.Length);
    }

    public async Task<Stream> GetFileStreamAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_storageRootPath, filePath);

        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"File not found: {filePath}");

        return await Task.FromResult<Stream>(new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read));
    }

    public string GetFileUrl(string filePath)
    {
        // Return relative URL for client access
        return $"{_baseUrl}/{filePath.Replace("\\", "/")}";
    }
}
