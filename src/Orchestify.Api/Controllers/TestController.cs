using Microsoft.AspNetCore.Mvc;

namespace Orchestify.Api.Controllers;

/// <summary>
/// Test controller for demonstrating middleware functionality.
/// Provides endpoints that trigger various scenarios for testing
/// correlation ID tracking and global exception handling.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class TestController : ControllerBase
{
    /// <summary>
    /// Test endpoint that returns success with correlation information.
    /// </summary>
    /// <returns>A success response with correlation IDs.</returns>
    [HttpGet("success")]
    public IActionResult GetSuccess()
    {
        return Ok(new
        {
            message = "Request processed successfully",
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Test endpoint that throws an exception to demonstrate global exception handling.
    /// </summary>
    /// <returns>Never returns; always throws an exception.</returns>
    [HttpGet("error")]
    public IActionResult GetError()
    {
        throw new InvalidOperationException("This is a test exception to demonstrate global exception handling.");
    }

    /// <summary>
    /// Test endpoint that throws a nested exception to verify inner exception logging.
    /// </summary>
    /// <returns>Never returns; always throws a nested exception.</returns>
    [HttpGet("nested-error")]
    public IActionResult GetNestedError()
    {
        try
        {
            try
            {
                throw new ArgumentException("Invalid argument provided.");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Outer exception wrapping inner exception.", ex);
            }
        }
        catch (Exception ex)
        {
            throw new AggregateException("Aggregate exception with multiple causes.", ex);
        }
    }

    /// <summary>
    /// Test endpoint that returns the current correlation ID from the request context.
    /// </summary>
    /// <returns>The correlation ID for the current request.</returns>
    [HttpGet("correlation")]
    public IActionResult GetCorrelationId()
    {
        string? correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        string? requestId = HttpContext.Items["RequestId"]?.ToString();

        return Ok(new
        {
            correlationId,
            requestId,
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Test endpoint that accepts a custom correlation ID header.
    /// </summary>
    /// <param name="correlationId">Optional correlation ID from X-Correlation-Id header.</param>
    /// <returns>The correlation ID that was used.</returns>
    [HttpGet("custom-correlation")]
    public IActionResult GetCustomCorrelationId([FromHeader(Name = "X-Correlation-Id")] string? correlationId = null)
    {
        string? actualCorrelationId = HttpContext.Items["CorrelationId"]?.ToString();

        return Ok(new
        {
            providedCorrelationId = correlationId,
            actualCorrelationId,
            wasProvided = !string.IsNullOrWhiteSpace(correlationId),
            timestamp = DateTime.UtcNow
        });
    }
}
