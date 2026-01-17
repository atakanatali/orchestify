using System;

namespace Orchestify.Domain.Entities;

/// <summary>
/// Represents a message within a task's chat history (Antigravity Chat).
/// </summary>
public class TaskMessageEntity
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public string Content { get; set; } = string.Empty;
    public MessageSender Sender { get; set; }
    public string? SuggestedAction { get; set; } // JSON string for tool calls
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public TaskEntity? Task { get; set; }
}

public enum MessageSender
{
    User,
    Agent // Antigravity
}
