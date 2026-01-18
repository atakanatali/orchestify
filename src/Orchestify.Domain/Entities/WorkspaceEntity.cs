using System;

namespace Orchestify.Domain.Entities;

/// <summary>
/// Represents a workspace which maps to a single repository/project boundary.
/// </summary>
public class WorkspaceEntity
{
    /// <summary>
    /// Unique identifier for the workspace.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Human-readable name of the workspace.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Absolute path to the local repository.
    /// </summary>
    public string RepositoryPath { get; set; } = string.Empty;

    /// <summary>
    /// Default git branch for the workspace (e.g., main, master).
    /// </summary>
    public string DefaultBranch { get; set; } = string.Empty;

    /// <summary>
    /// Total number of tasks associated with this workspace.
    /// </summary>
    public int TotalTasks { get; set; }

    /// <summary>
    /// Number of currently running tasks.
    /// </summary>
    public int RunningTasks { get; set; }

    /// <summary>
    /// Timestamp of the last activity in this workspace.
    /// </summary>
    public DateTime? LastActivityAt { get; set; }

    /// <summary>
    /// Overall progress percentage of tasks in the workspace.
    /// </summary>
    public int ProgressPercent { get; set; }

    /// <summary>
    /// Date when the workspace was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date when the workspace was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Navigation property to boards in this workspace.
    /// </summary>
    public virtual ICollection<BoardEntity> Boards { get; set; } = new List<BoardEntity>();
}
