using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orchestify.Application.Common.Interfaces;

namespace Orchestify.Api.Controllers;

/// <summary>
/// Health check endpoints for monitoring.
/// </summary>
[ApiController]
[Route("api/health")]
public class HealthController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public HealthController(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Returns basic health status.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(HealthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetHealth()
    {
        try
        {
            // Check database connectivity
            var canConnect = await _context.Workspaces.AnyAsync();
            return Ok(new HealthResponse
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Database = "Connected"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(503, new HealthResponse
            {
                Status = "Unhealthy",
                Timestamp = DateTime.UtcNow,
                Database = $"Error: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Returns detailed health with component status.
    /// </summary>
    [HttpGet("detailed")]
    [ProducesResponseType(typeof(DetailedHealthResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDetailedHealth()
    {
        var dbStatus = "Unknown";
        try
        {
            await _context.Workspaces.AnyAsync();
            dbStatus = "Healthy";
        }
        catch { dbStatus = "Unhealthy"; }

        return Ok(new DetailedHealthResponse
        {
            Status = dbStatus == "Healthy" ? "Healthy" : "Degraded",
            Timestamp = DateTime.UtcNow,
            Components = new Dictionary<string, string>
            {
                ["Database"] = dbStatus,
                ["Worker"] = "Unknown", // Would check via SignalR or Redis
                ["Cache"] = "Unknown"
            }
        });
    }
}

public class HealthResponse
{
    public string Status { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Database { get; set; } = string.Empty;
}

public class DetailedHealthResponse
{
    public string Status { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public Dictionary<string, string> Components { get; set; } = new();
}
