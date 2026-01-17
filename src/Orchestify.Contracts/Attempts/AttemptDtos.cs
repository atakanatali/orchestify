namespace Orchestify.Contracts.Attempts;

/// <summary>
/// DTO representing an execution attempt.
/// </summary>
public class AttemptDto
{
    /// <summary>
    /// Unique identifier for the attempt.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Parent task identifier.
    /// </summary>
    public Guid TaskId { get; set; }

    /// <summary>
    /// Current state of the attempt.
    /// </summary>
    public string State { get; set; } = "Queued";

    /// <summary>
    /// Attempt number.
    /// </summary>
    public int AttemptNumber { get; set; }

    /// <summary>
    /// Date when queued.
    /// </summary>
    public DateTime QueuedAt { get; set; }

    /// <summary>
    /// Date when started.
    /// </summary>
    public DateTime? StartedAt { get; set; }

    /// <summary>
    /// Date when completed.
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Error message if failed.
    /// </summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Response DTO for run task operation.
/// </summary>
public class RunTaskResponseDto
{
    /// <summary>
    /// The created attempt.
    /// </summary>
    public AttemptDto Attempt { get; set; } = new();
}
