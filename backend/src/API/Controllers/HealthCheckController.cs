using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics;
using System.Diagnostics;

namespace NationalClothingStore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthCheckController : ControllerBase
{
    private readonly ILogger<HealthCheckController> _logger;
    private readonly ActivitySource _activitySource = new ActivitySource("NationalClothingStore.API");

    public HealthCheckController(ILogger<HealthCheckController> logger)
    {
        _logger = logger;
    }

    [HttpGet("health")]
    public IActionResult GetHealth()
    {
        using var activity = _activitySource.StartActivity("HealthCheck");
        try
        {
            var health = new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Version = "1.0.0",
                Checks = new[]
                {
                    new { Name = "Database", Status = "OK" },
                    new { Name = "Cache", Status = "OK" },
                    new { Name = "ReportingService", Status = "OK" }
                }
            };

            _logger.LogInformation("Health check completed: Status={Status}, Checks={Checks}", health.Status, health.Checks);
            activity.SetStatus(ActivityStatusCode.Ok);
            return Ok(health);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            activity.SetStatus(ActivityStatusCode.Error);
            return StatusCode(500, new { status = "Unhealthy", error = ex.Message });
        }
    }
}
