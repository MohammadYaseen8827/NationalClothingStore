using Microsoft.AspNetCore.Http;

namespace NationalClothingStore.Application.Interfaces;

/// <summary>
/// Service interface for file upload operations
/// </summary>
public interface IFileUploadService
{
    /// <summary>
    /// Upload a file asynchronously
    /// </summary>
    /// <param name="file">File to upload</param>
    /// <param name="folder">Target folder path</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Upload result with file information</returns>
    Task<FileUploadResult> UploadFileAsync(IFormFile file, string folder, CancellationToken cancellationToken = default);

    /// <summary>
    /// Upload multiple files asynchronously
    /// </summary>
    /// <param name="files">Files to upload</param>
    /// <param name="folder">Target folder path</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of upload results</returns>
    Task<IEnumerable<FileUploadResult>> UploadFilesAsync(IEnumerable<IFormFile> files, string folder, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a file asynchronously
    /// </summary>
    /// <param name="filePath">Path to file to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if file was deleted successfully</returns>
    Task<bool> DeleteFileAsync(string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validate file upload
    /// </summary>
    /// <param name="file">File to validate</param>
    /// <param name="allowedExtensions">Allowed file extensions</param>
    /// <param name="maxFileSize">Maximum file size in bytes</param>
    /// <returns>Validation result</returns>
    Task<FileValidationResult> ValidateFile(IFormFile file, string[] allowedExtensions, long maxFileSize);

    /// <summary>
    /// Get file information
    /// </summary>
    /// <param name="filePath">Path to file</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>File information</returns>
    Task<FileInfo?> GetFileInfoAsync(string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if file exists
    /// </summary>
    /// <param name="filePath">Path to file</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if file exists</returns>
    Task<bool> FileExistsAsync(string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generate unique file name
    /// </summary>
    /// <param name="fileName">Original file name</param>
    /// <returns>Unique file name</returns>
    string GenerateUniqueFileName(string fileName);

    /// <summary>
    /// Get file URL from file path
    /// </summary>
    /// <param name="filePath">File path</param>
    /// <returns>File URL</returns>
    string GetFileUrl(string filePath);
}

/// <summary>
/// File upload result
/// </summary>
public class FileUploadResult
{
    public bool Success { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// File validation result
/// </summary>
public class FileValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new List<string>();

    public static FileValidationResult Success() => new FileValidationResult { IsValid = true };

    public static FileValidationResult Failure(params string[] errors)
    {
        return new FileValidationResult
        {
            IsValid = false,
            Errors = errors.ToList()
        };
    }

    public void AddError(string error)
    {
        Errors.Add(error);
        IsValid = false;
    }
}
