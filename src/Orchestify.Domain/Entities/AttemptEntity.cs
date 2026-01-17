using System;

namespace Orchestify.Domain.Entities;

/// <summary>
/// Represents an execution attempt for a task.
/// Each run of a task creates a new attempt with its own lifecycle.
/// </summary>
public class AttemptEntity
{
    /// <summary>
    /// Unique identifier for the attempt.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key to the parent task.
    /// </summary>
    public Guid TaskId { get; set; }

    /// <summary>
    /// Current state of the attempt.
    /// </summary>
    public Enums.AttemptState State { get; set; } = Enums.AttemptState.Queued;

    /// <summary>
    /// Attempt number (1-based, increments per task).
    /// </summary>
    public int AttemptNumber { get; set; }

    /// <summary>
    /// Date when the attempt was queued.
    /// </summary>
    public DateTime QueuedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date when the attempt started running.
    /// </summary>
    public DateTime? StartedAt { get; set; }

    /// <summary>
    /// Date when the attempt completed.
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Last heartbeat timestamp for stale detection.
    /// </summary>
    public DateTime? LastHeartbeatAt { get; set; }

    /// <summary>
    /// Error message if the attempt failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Cancellation reason if cancelled.
    /// </summary>
    public string? CancellationReason { get; set; }

    /// <summary>
    /// Correlation ID for distributed tracing.
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Navigation property to the parent task.
    /// </summary>
    public TaskEntity? Task { get; set; }
}
