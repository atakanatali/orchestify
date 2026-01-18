using System;

namespace Orchestify.Domain.Entities;

/// <summary>
/// Represents a board within a workspace for organizing tasks.
/// Boards are scoped to a single workspace and must have unique names within that workspace.
/// </summary>
public class BoardEntity
{
    /// <summary>
    /// Unique identifier for the board.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key to the parent workspace.
    /// </summary>
    public Guid WorkspaceId { get; set; }

    /// <summary>
    /// Human-readable name of the board. Must be unique within the workspace.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional description of the board's purpose.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Total number of tasks in this board.
    /// </summary>
    public int TotalTasks { get; set; }

    /// <summary>
    /// Number of completed tasks in this board.
    /// </summary>
    public int CompletedTasks { get; set; }

    /// <summary>
    /// Date when the board was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date when the board was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Navigation property to the parent workspace.
    /// </summary>
    public WorkspaceEntity? Workspace { get; set; }

    /// <summary>
    /// Navigation property to tasks in this board.
    /// </summary>
    public virtual ICollection<TaskEntity> Tasks { get; set; } = new List<TaskEntity>();
}
