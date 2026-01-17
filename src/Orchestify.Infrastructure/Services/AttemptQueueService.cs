using Microsoft.EntityFrameworkCore;
using Orchestify.Application.Common.Interfaces;
using Orchestify.Domain.Entities;
using Orchestify.Domain.Enums;

namespace Orchestify.Infrastructure.Services;

/// <summary>
/// PostgreSQL-based queue service using SELECT FOR UPDATE SKIP LOCKED pattern.
/// </summary>
public class AttemptQueueService : IAttemptQueueService
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="AttemptQueueService"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public AttemptQueueService(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<AttemptEntity?> DequeueNextAsync(CancellationToken cancellationToken)
    {
        // Using raw SQL for SELECT FOR UPDATE SKIP LOCKED
        // This ensures no duplicate processing in multi-instance scenarios
        var attempt = await _context.Attempts
            .FromSqlRaw(@"
                SELECT * FROM ""Attempts"" 
                WHERE ""State"" = 'Queued' 
                ORDER BY ""QueuedAt"" ASC 
                LIMIT 1 
                FOR UPDATE SKIP LOCKED")
            .FirstOrDefaultAsync(cancellationToken);

        return attempt;
    }

    /// <inheritdoc />
    public async Task MarkAsRunningAsync(Guid attemptId, CancellationToken cancellationToken)
    {
        var attempt = await _context.Attempts
            .FirstOrDefaultAsync(a => a.Id == attemptId, cancellationToken);

        if (attempt != null)
        {
            attempt.State = AttemptState.Running;
            attempt.StartedAt = DateTime.UtcNow;
            attempt.LastHeartbeatAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task MarkAsSucceededAsync(Guid attemptId, CancellationToken cancellationToken)
    {
        var attempt = await _context.Attempts
            .Include(a => a.Task)
            .ThenInclude(t => t!.Board)
            .FirstOrDefaultAsync(a => a.Id == attemptId, cancellationToken);

        if (attempt != null)
        {
            attempt.State = AttemptState.Succeeded;
            attempt.CompletedAt = DateTime.UtcNow;

            // Update task status to Done
            if (attempt.Task != null)
            {
                attempt.Task.Status = Domain.Enums.TaskStatus.Done;
                attempt.Task.CompletedAt = DateTime.UtcNow;
                attempt.Task.UpdatedAt = DateTime.UtcNow;

                if (attempt.Task.Board != null)
                {
                    attempt.Task.Board.CompletedTasks++;
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task MarkAsFailedAsync(Guid attemptId, string errorMessage, CancellationToken cancellationToken)
    {
        var attempt = await _context.Attempts
            .Include(a => a.Task)
            .FirstOrDefaultAsync(a => a.Id == attemptId, cancellationToken);

        if (attempt != null)
        {
            attempt.State = AttemptState.Failed;
            attempt.CompletedAt = DateTime.UtcNow;
            attempt.ErrorMessage = errorMessage;

            // Update task back to Todo for retry
            if (attempt.Task != null)
            {
                attempt.Task.Status = Domain.Enums.TaskStatus.Todo;
                attempt.Task.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task UpdateHeartbeatAsync(Guid attemptId, CancellationToken cancellationToken)
    {
        var attempt = await _context.Attempts
            .FirstOrDefaultAsync(a => a.Id == attemptId, cancellationToken);

        if (attempt != null)
        {
            attempt.LastHeartbeatAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task<int> RecoverStaleAttemptsAsync(int heartbeatTimeoutSeconds, CancellationToken cancellationToken)
    {
        var staleThreshold = DateTime.UtcNow.AddSeconds(-heartbeatTimeoutSeconds);

        var staleAttempts = await _context.Attempts
            .Where(a => a.State == AttemptState.Running 
                       && a.LastHeartbeatAt < staleThreshold)
            .ToListAsync(cancellationToken);

        foreach (var attempt in staleAttempts)
        {
            attempt.State = AttemptState.Queued;
            attempt.StartedAt = null;
            attempt.LastHeartbeatAt = null;
        }

        if (staleAttempts.Count > 0)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        return staleAttempts.Count;
    }
}
