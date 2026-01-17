using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orchestify.Application.Common.Interfaces;

namespace Orchestify.Api.Controllers;

/// <summary>
/// Server-Sent Events endpoint for streaming task execution logs.
/// </summary>
[ApiController]
[Route("api/attempts/{attemptId:guid}/stream")]
public class StreamController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public StreamController(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Streams run step updates as Server-Sent Events.
    /// </summary>
    [HttpGet]
    public async Task StreamSteps(Guid attemptId, CancellationToken cancellationToken)
    {
        Response.ContentType = "text/event-stream";
        Response.Headers["Cache-Control"] = "no-cache";
        Response.Headers["Connection"] = "keep-alive";

        var attempt = await _context.Attempts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == attemptId, cancellationToken);

        if (attempt == null)
        {
            await WriteEventAsync("error", new { message = "Attempt not found" }, cancellationToken);
            return;
        }

        // Send initial state
        var steps = await _context.RunSteps
            .AsNoTracking()
            .Where(s => s.AttemptId == attemptId)
            .OrderBy(s => s.SequenceNumber)
            .Select(s => new
            {
                s.Id,
                s.Name,
                StepType = s.StepType.ToString(),
                State = s.State.ToString(),
                s.SequenceNumber,
                s.DurationMs
            })
            .ToListAsync(cancellationToken);

        await WriteEventAsync("init", new { attemptId, steps }, cancellationToken);

        // Poll for updates every 2 seconds
        var processedSteps = new HashSet<Guid>();
        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(2000, cancellationToken);

            var currentAttempt = await _context.Attempts
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == attemptId, cancellationToken);

            if (currentAttempt?.State is Domain.Enums.AttemptState.Succeeded or 
                Domain.Enums.AttemptState.Failed or 
                Domain.Enums.AttemptState.Cancelled)
            {
                await WriteEventAsync("complete", new { state = currentAttempt.State.ToString() }, cancellationToken);
                break;
            }

            var updatedSteps = await _context.RunSteps
                .AsNoTracking()
                .Where(s => s.AttemptId == attemptId && s.State != Domain.Enums.RunStepState.Pending)
                .ToListAsync(cancellationToken);

            foreach (var step in updatedSteps)
            {
                if (!processedSteps.Contains(step.Id) || step.State == Domain.Enums.RunStepState.Running)
                {
                    processedSteps.Add(step.Id);
                    await WriteEventAsync("step", new
                    {
                        step.Id,
                        step.Name,
                        State = step.State.ToString(),
                        step.DurationMs,
                        step.ErrorMessage
                    }, cancellationToken);
                }
            }
        }
    }

    private async Task WriteEventAsync(string eventType, object data, CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(data);
        await Response.WriteAsync($"event: {eventType}\n", cancellationToken);
        await Response.WriteAsync($"data: {json}\n\n", cancellationToken);
        await Response.Body.FlushAsync(cancellationToken);
    }
}
