using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace NationalClothingStore.API.Controllers;

/// <summary>
/// Health check controller for monitoring application health
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[AllowAnonymous]
public class HealthController : ControllerBase
{
    private readonly HealthCheckService _healthCheckService;
    private readonly ILogger<HealthController> _logger;

    public HealthController(HealthCheckService healthCheckService, ILogger<HealthController> logger)
    {
        _healthCheckService = healthCheckService;
        _logger = logger;
    }

    /// <summary>
    /// Basic health check endpoint
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var report = await _healthCheckService.CheckHealthAsync();
        
        var response = new
        {
            Status = report.Status.ToString(),
            Timestamp = DateTime.UtcNow,
            Duration = report.TotalDuration,
            Results = report.Entries.Select(e => new
            {
                Key = e.Key,
                Status = e.Value.Status.ToString(),
                Description = e.Value.Description,
                Duration = e.Value.Duration,
                Data = e.Value.Data,
                Exception = e.Value.Exception?.Message
            })
        };

        return report.Status == HealthStatus.Healthy 
            ? Ok(response) 
            : StatusCode(StatusCodes.Status503ServiceUnavailable, response);
    }

    /// <summary>
    /// Liveness probe - checks if the application is running
    /// </summary>
    [HttpGet("liveness")]
    public IActionResult GetLiveness()
    {
        var response = new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Application = "National Clothing Store API"
        };

        return Ok(response);
    }

    /// <summary>
    /// Readiness probe - checks if the application is ready to serve traffic
    /// </summary>
    [HttpGet("readiness")]
    public async Task<IActionResult> GetReadiness()
    {
        try
        {
            // Check database connectivity
            var report = await _healthCheckService.CheckHealthAsync();
            
            var isReady = report.Status == HealthStatus.Healthy;
            
            var response = new
            {
                Status = isReady ? "Ready" : "Not Ready",
                Timestamp = DateTime.UtcNow,
                Checks = report.Entries.Select(e => new
                {
                    Name = e.Key,
                    Status = e.Value.Status.ToString(),
                    Description = e.Value.Description
                })
            };

            return isReady 
                ? Ok(response) 
                : StatusCode(StatusCodes.Status503ServiceUnavailable, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Readiness check failed");
            
            var response = new
            {
                Status = "Not Ready",
                Timestamp = DateTime.UtcNow,
                Error = ex.Message
            };

            return StatusCode(StatusCodes.Status503ServiceUnavailable, response);
        }
    }

    /// <summary>
    /// Detailed health information with system metrics
    /// </summary>
    [HttpGet("detailed")]
    public async Task<IActionResult> GetDetailedHealth()
    {
        try
        {
            var report = await _healthCheckService.CheckHealthAsync();
            
            var response = new
            {
                Status = report.Status.ToString(),
                Timestamp = DateTime.UtcNow,
                Duration = report.TotalDuration,
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                Version = typeof(Program).Assembly.GetName().Version?.ToString() ?? "Unknown",
                MachineName = Environment.MachineName,
                ProcessId = Environment.ProcessId,
                UpTime = DateTime.UtcNow - Process.GetCurrentProcess().StartTime,
                MemoryUsage = GC.GetTotalMemory(false),
                ThreadCount = Process.GetCurrentProcess().Threads.Count,
                Results = report.Entries.Select(e => new
                {
                    Key = e.Key,
                    Status = e.Value.Status.ToString(),
                    Description = e.Value.Description,
                    Duration = e.Value.Duration,
                    Data = e.Value.Data,
                    Exception = e.Value.Exception?.Message,
                    Tags = e.Value.Tags
                })
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Detailed health check failed");
            
            var response = new
            {
                Status = "Unhealthy",
                Timestamp = DateTime.UtcNow,
                Error = ex.Message
            };

            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }
}
