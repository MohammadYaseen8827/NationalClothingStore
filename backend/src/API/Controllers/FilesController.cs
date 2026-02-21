using Microsoft.AspNetCore.Mvc;
using NationalClothingStore.Application.Interfaces;

namespace NationalClothingStore.API.Controllers;

/// <summary>
/// API controller for file management operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly IFileUploadService _fileUploadService;
    private readonly ILogger<FilesController> _logger;

    public FilesController(
        IFileUploadService fileUploadService,
        ILogger<FilesController> logger)
    {
        _fileUploadService = fileUploadService;
        _logger = logger;
    }

    /// <summary>
    /// Upload a single file
    /// </summary>
    [HttpPost("upload")]
    public async Task<ActionResult<FileUploadResult>> UploadFile(
        IFormFile file,
        [FromForm] string folder = "uploads",
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _fileUploadService.UploadFileAsync(file, folder, cancellationToken);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Upload multiple files
    /// </summary>
    [HttpPost("upload-multiple")]
    public async Task<ActionResult<IEnumerable<FileUploadResult>>> UploadFiles(
        IFormFileCollection files,
        [FromForm] string folder = "uploads",
        CancellationToken cancellationToken = default)
    {
        try
        {
            var results = await _fileUploadService.UploadFilesAsync(files, folder, cancellationToken);
            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading files");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Delete a file
    /// </summary>
    [HttpDelete]
    public async Task<ActionResult> DeleteFile(
        [FromBody] DeleteFileRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.FilePath))
            {
                return BadRequest("File path is required");
            }

            var deleted = await _fileUploadService.DeleteFileAsync(request.FilePath, cancellationToken);
            if (!deleted)
            {
                return NotFound("File not found");
            }
            return Ok(new { success = true, message = "File deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file: {FilePath}", request.FilePath);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get file information
    /// </summary>
    [HttpGet("info")]
    public async Task<ActionResult> GetFileInfo(
        [FromQuery] string filePath,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return BadRequest("File path is required");
            }

            var fileInfo = await _fileUploadService.GetFileInfoAsync(filePath, cancellationToken);
            if (fileInfo == null)
            {
                return NotFound("File not found");
            }
            
            return Ok(new
            {
                name = fileInfo.Name,
                size = fileInfo.Length,
                lastModified = fileInfo.LastWriteTimeUtc,
                exists = fileInfo.Exists
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting file info: {FilePath}", filePath);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Check if file exists
    /// </summary>
    [HttpGet("exists")]
    public async Task<ActionResult<bool>> FileExists(
        [FromQuery] string filePath,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return BadRequest("File path is required");
            }

            var exists = await _fileUploadService.FileExistsAsync(filePath, cancellationToken);
            return Ok(exists);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking file existence: {FilePath}", filePath);
            return StatusCode(500, "Internal server error");
        }
    }
}

/// <summary>
/// Request model for file deletion
/// </summary>
public class DeleteFileRequest
{
    public string FilePath { get; set; } = string.Empty;
}
