using Orchestify.Domain.Entities;

namespace Orchestify.Application.Common.Interfaces;

/// <summary>
/// Service interface for queue operations on attempts.
/// </summary>
public interface IAttemptQueueService
{
    /// <summary>
    /// Dequeues the next available attempt using SELECT FOR UPDATE SKIP LOCKED.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The dequeued attempt or null if queue is empty.</returns>
    Task<AttemptEntity?> DequeueNextAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Marks an attempt as running.
    /// </summary>
    /// <param name="attemptId">The attempt ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task MarkAsRunningAsync(Guid attemptId, CancellationToken cancellationToken);

    /// <summary>
    /// Marks an attempt as succeeded.
    /// </summary>
    /// <param name="attemptId">The attempt ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task MarkAsSucceededAsync(Guid attemptId, CancellationToken cancellationToken);

    /// <summary>
    /// Marks an attempt as failed.
    /// </summary>
    /// <param name="attemptId">The attempt ID.</param>
    /// <param name="errorMessage">The error message.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task MarkAsFailedAsync(Guid attemptId, string errorMessage, CancellationToken cancellationToken);

    /// <summary>
    /// Updates the heartbeat timestamp for stale detection.
    /// </summary>
    /// <param name="attemptId">The attempt ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task UpdateHeartbeatAsync(Guid attemptId, CancellationToken cancellationToken);

    /// <summary>
    /// Recovers stale attempts that have gone past heartbeat timeout.
    /// </summary>
    /// <param name="heartbeatTimeoutSeconds">Seconds before an attempt is considered stale.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Number of recovered attempts.</returns>
    Task<int> RecoverStaleAttemptsAsync(int heartbeatTimeoutSeconds, CancellationToken cancellationToken);
}
