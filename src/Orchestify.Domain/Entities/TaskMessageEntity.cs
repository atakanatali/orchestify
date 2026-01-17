using System;

namespace Orchestify.Domain.Entities;

/// <summary>
/// Represents a message in a task-linked AI chat.
/// </summary>
public class TaskMessageEntity
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the task identifier.
    /// </summary>
    public Guid TaskId { get; set; }

    /// <summary>
    /// Gets or sets the message content.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the sender type (User or Agent).
    /// </summary>
    public string Sender { get; set; } = "User";

    /// <summary>
    /// Gets or sets a suggested action (serialized JSON), if any.
    /// </summary>
    public string? SuggestedAction { get; set; }

    /// <summary>
    /// Gets or sets the date when the message was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the navigation property for the task.
    /// </summary>
    public virtual TaskEntity Task { get; set; } = null!;
}
