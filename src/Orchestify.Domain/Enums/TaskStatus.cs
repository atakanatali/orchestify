namespace Orchestify.Domain.Enums;

/// <summary>
/// Represents the status of a task in the system.
/// </summary>
public enum TaskStatus
{
    /// <summary>
    /// Task is created but not yet started.
    /// </summary>
    Todo = 0,

    /// <summary>
    /// Task is currently being worked on.
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// Task is under review.
    /// </summary>
    Review = 2,

    /// <summary>
    /// Task has been completed successfully.
    /// </summary>
    Done = 3,

    /// <summary>
    /// Task has been cancelled.
    /// </summary>
    Cancelled = 4
}
