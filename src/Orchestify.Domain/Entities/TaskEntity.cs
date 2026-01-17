using System;

namespace Orchestify.Domain.Entities;

/// <summary>
/// Represents a task within a board.
/// Tasks are the fundamental unit of work in Orchestify.
/// </summary>
public class TaskEntity
{
    /// <summary>
    /// Unique identifier for the task.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key to the parent board.
    /// </summary>
    public Guid BoardId { get; set; }

    /// <summary>
    /// Human-readable title of the task.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description of the task.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Current status of the task.
    /// </summary>
    public Enums.TaskStatus Status { get; set; } = Enums.TaskStatus.Todo;

    /// <summary>
    /// Order key for Kanban positioning within the status column.
    /// </summary>
    public int OrderKey { get; set; }

    /// <summary>
    /// Priority level (1 = highest, 5 = lowest).
    /// </summary>
    public int Priority { get; set; } = 3;

    /// <summary>
    /// Number of execution attempts for this task.
    /// </summary>
    public int AttemptCount { get; set; }

    /// <summary>
    /// Date when the task was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date when the task was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Date when the task was completed.
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Navigation property to the parent board.
    /// </summary>
    public BoardEntity? Board { get; set; }
}
