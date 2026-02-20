using NationalClothingStore.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Linq;

namespace NationalClothingStore.Infrastructure.External;

/// <summary>
/// Service for file upload operations
/// </summary>
public class FileUploadService : IFileUploadService
{
    private readonly ILogger<FileUploadService> _logger;
    private readonly IConfiguration _configuration;
    private readonly string _baseUploadPath;
    private readonly string _baseUrl;

    public FileUploadService(ILogger<FileUploadService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        _baseUploadPath = configuration["FileUpload:BasePath"] ?? "uploads";
        _baseUrl = configuration["FileUpload:BaseUrl"] ?? "/uploads";
        
        // Ensure upload directory exists
        if (!Directory.Exists(_baseUploadPath))
        {
            Directory.CreateDirectory(_baseUploadPath);
        }
    }

    public async Task<FileUploadResult> UploadFileAsync(IFormFile file, string folder, CancellationToken cancellationToken = default)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return new FileUploadResult
                {
                    Success = false,
                    ErrorMessage = "No file provided or file is empty"
                };
            }

            // Validate file
            var allowedExtensions = _configuration.GetSection("FileUpload:AllowedExtensions").Get<string[]>() ?? 
                new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp" };
            var maxFileSize = _configuration.GetValue<long>("FileUpload:MaxFileSize", 5 * 1024 * 1024); // 5MB default

            var validationResult = await ValidateFile(file, allowedExtensions, maxFileSize);
            if (!validationResult.IsValid)
            {
                return new FileUploadResult
                {
                    Success = false,
                    ErrorMessage = string.Join(", ", validationResult.Errors)
                };
            }

            // Create folder if it doesn't exist
            var folderPath = Path.Combine(_baseUploadPath, folder);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Generate unique file name
            var uniqueFileName = GenerateUniqueFileName(file.FileName);
            var filePath = Path.Combine(folderPath, uniqueFileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            var relativePath = Path.Combine(folder, uniqueFileName).Replace("\\", "/");
            var fileUrl = GetFileUrl(relativePath);

            _logger.LogInformation("File uploaded successfully: {FileName} to {FilePath}", file.FileName, filePath);

            return new FileUploadResult
            {
                Success = true,
                FileName = uniqueFileName,
                OriginalFileName = file.FileName,
                FilePath = filePath,
                FileUrl = fileUrl,
                FileSize = file.Length,
                ContentType = file.ContentType,
                UploadedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file: {FileName}", file?.FileName);
            return new FileUploadResult
            {
                Success = false,
                ErrorMessage = "An error occurred while uploading the file"
            };
        }
    }

    public async Task<IEnumerable<FileUploadResult>> UploadFilesAsync(IEnumerable<IFormFile> files, string folder, CancellationToken cancellationToken = default)
    {
        var uploadTasks = new List<Task<FileUploadResult>>();
        
        foreach (var file in files)
        {
            uploadTasks.Add(UploadFileAsync(file, folder, cancellationToken));
        }

        var results = await Task.WhenAll(uploadTasks);
        return results;
    }

    public async Task<bool> DeleteFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                return false;
            }

            await Task.Run(() => File.Delete(filePath), cancellationToken);
            
            _logger.LogInformation("File deleted successfully: {FilePath}", filePath);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file: {FilePath}", filePath);
            return false;
        }
    }

    public async Task<FileValidationResult> ValidateFile(IFormFile file, string[] allowedExtensions, long maxFileSize)
    {
        var errors = new List<string>();

        // Check file size
        if (file.Length > maxFileSize)
        {
            errors.Add($"File size exceeds maximum allowed size of {maxFileSize / (1024 * 1024)}MB");
        }

        // Check file extension
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(fileExtension))
        {
            errors.Add($"File extension '{fileExtension}' is not allowed. Allowed extensions: {string.Join(", ", allowedExtensions)}");
        }

        // Check content type
        var allowedContentTypes = new[]
        {
            "image/jpeg", "image/jpg", "image/png", "image/gif", 
            "image/webp", "image/bmp", "image/svg+xml"
        };
        
        if (!allowedContentTypes.Contains(file.ContentType.ToLowerInvariant()))
        {
            errors.Add($"Content type '{file.ContentType}' is not allowed");
        }

        // Additional validation for image files (basic validation without System.Drawing)
        if (file.ContentType.StartsWith("image/"))
        {
            try
            {
                // Basic validation - just check if we can read the stream
                await using var imageStream = file.OpenReadStream();
                var buffer = new byte[1024];
                await imageStream.ReadExactlyAsync(buffer);
                
                // Reset stream position
                imageStream.Position = 0;
                
                _logger.LogDebug("Image file validation passed for: {FileName}", file.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to validate image file: {FileName}", file.FileName);
                errors.Add("Invalid or corrupted image file");
            }
        }

        return errors.Any() ? FileValidationResult.Failure(errors.ToArray()) : FileValidationResult.Success();
    }

    public async Task<FileInfo?> GetFileInfoAsync(string filePath, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                return null;
            }

            var fileInfo = new FileInfo(filePath);
            return await Task.FromResult(fileInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting file info: {FilePath}", filePath);
            return null;
        }
    }

    public async Task<bool> FileExistsAsync(string filePath, CancellationToken cancellationToken = default)
    {
        try
        {
            return await Task.FromResult(File.Exists(filePath));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking file existence: {FilePath}", filePath);
            return false;
        }
    }

    public string GenerateUniqueFileName(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        
        // Sanitize file name
        var sanitizedFileName = SanitizeFileName(fileNameWithoutExtension);
        
        // Add timestamp and random suffix for uniqueness
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var randomSuffix = Guid.NewGuid().ToString("N")[..8];
        
        return $"{sanitizedFileName}_{timestamp}_{randomSuffix}{extension}";
    }

    public string GetFileUrl(string filePath)
    {
        return $"{_baseUrl.TrimEnd('/')}/{filePath.Replace("\\", "/")}";
    }

    private string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = new StringBuilder(fileName.Length);
        
        foreach (var c in fileName)
        {
            if (!invalidChars.Contains(c))
            {
                sanitized.Append(c);
            }
            else
            {
                sanitized.Append('_');
            }
        }
        
        // Remove or replace other problematic characters
        return sanitized.ToString()
            .Replace(" ", "_")
            .Replace("..", "_")
            .Replace("/", "_")
            .Replace("\\", "_");
    }

    /// <summary>
    /// Get upload statistics
    /// </summary>
    /// <param name="folder">Folder to check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Upload statistics</returns>
    public async Task<UploadStatistics> GetUploadStatisticsAsync(string folder, CancellationToken cancellationToken = default)
    {
        try
        {
            var folderPath = Path.Combine(_baseUploadPath, folder);
            
            if (!Directory.Exists(folderPath))
            {
                return new UploadStatistics
                {
                    TotalFiles = 0,
                    TotalSize = 0,
                    FileCountByType = new Dictionary<string, int>()
                };
            }

            var files = await Task.Run(() => Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories), cancellationToken);
            var fileCountByType = new Dictionary<string, int>();
            var totalSize = 0L;

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                var extension = fileInfo.Extension.ToLowerInvariant();
                
                if (!fileCountByType.ContainsKey(extension))
                {
                    fileCountByType[extension] = 0;
                }
                
                fileCountByType[extension]++;
                totalSize += fileInfo.Length;
            }

            return new UploadStatistics
            {
                TotalFiles = files.Length,
                TotalSize = totalSize,
                FileCountByType = fileCountByType,
                LastUpdated = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting upload statistics for folder: {Folder}", folder);
            return new UploadStatistics
            {
                TotalFiles = 0,
                TotalSize = 0,
                FileCountByType = new Dictionary<string, int>(),
                ErrorMessage = "Failed to retrieve statistics"
            };
        }
    }

    /// <summary>
    /// Clean up old files
    /// </summary>
    /// <param name="folder">Folder to clean</param>
    /// <param name="olderThanDays">Delete files older than this many days</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of files deleted</returns>
    public async Task<int> CleanupOldFilesAsync(string folder, int olderThanDays, CancellationToken cancellationToken = default)
    {
        try
        {
            var folderPath = Path.Combine(_baseUploadPath, folder);
            
            if (!Directory.Exists(folderPath))
            {
                return 0;
            }

            var cutoffDate = DateTime.UtcNow.AddDays(-olderThanDays);
            var files = await Task.Run(() => Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories), cancellationToken);
            var deletedCount = 0;

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                if (fileInfo.CreationTime < cutoffDate)
                {
                    try
                    {
                        File.Delete(file);
                        deletedCount++;
                        _logger.LogInformation("Deleted old file: {FilePath}", file);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to delete old file: {FilePath}", file);
                    }
                }
            }

            return deletedCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up old files in folder: {Folder}", folder);
            return 0;
        }
    }
}

/// <summary>
/// Upload statistics
/// </summary>
public class UploadStatistics
{
    public int TotalFiles { get; set; }
    public long TotalSize { get; set; }
    public Dictionary<string, int> FileCountByType { get; set; } = new();
    public DateTime LastUpdated { get; set; }
    public string? ErrorMessage { get; set; }
}
