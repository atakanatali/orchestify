namespace Orchestify.Infrastructure.Messaging;

/// <summary>
/// Message contract for task execution requests.
/// This is published to RabbitMQ when a task needs to be executed.
/// </summary>
public record RunTaskMessage
{
    /// <summary>
    /// The task identifier.
    /// </summary>
    public Guid TaskId { get; init; }

    /// <summary>
    /// The board identifier (for context).
    /// </summary>
    public Guid BoardId { get; init; }

    /// <summary>
    /// The task description/prompt to execute.
    /// </summary>
    public string Prompt { get; init; } = string.Empty;

    /// <summary>
    /// Correlation ID for tracking.
    /// </summary>
    public string? CorrelationId { get; init; }
}

/// <summary>
/// Message contract for real-time task events.
/// Published to Redis Pub/Sub for SignalR bridging.
/// </summary>
public record TaskEventMessage
{
    public Guid TaskId { get; init; }
    public string EventType { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}
