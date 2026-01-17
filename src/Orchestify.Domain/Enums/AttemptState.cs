namespace Orchestify.Domain.Enums;

/// <summary>
/// Represents the state of an execution attempt.
/// </summary>
public enum AttemptState
{
    /// <summary>
    /// Attempt is queued and waiting to be processed.
    /// </summary>
    Queued = 0,

    /// <summary>
    /// Attempt is currently running.
    /// </summary>
    Running = 1,

    /// <summary>
    /// Attempt completed successfully.
    /// </summary>
    Succeeded = 2,

    /// <summary>
    /// Attempt failed.
    /// </summary>
    Failed = 3,

    /// <summary>
    /// Attempt was cancelled.
    /// </summary>
    Cancelled = 4,

    /// <summary>
    /// Attempt is being cancelled.
    /// </summary>
    Cancelling = 5
}
