using Microsoft.EntityFrameworkCore;
using Orchestify.Domain.Entities;

namespace Orchestify.Application.Common.Interfaces;

/// <summary>
/// Application database context interface for data access.
/// </summary>
public interface IApplicationDbContext
{
    /// <summary>
    /// Workspaces table.
    /// </summary>
    DbSet<WorkspaceEntity> Workspaces { get; }

    /// <summary>
    /// Boards table.
    /// </summary>
    DbSet<BoardEntity> Boards { get; }

    /// <summary>
    /// Tasks table.
    /// </summary>
    DbSet<TaskEntity> Tasks { get; }

    /// <summary>
    /// Attempts table.
    /// </summary>
    DbSet<AttemptEntity> Attempts { get; }

    /// <summary>
    /// RunSteps table.
    /// </summary>
    DbSet<RunStepEntity> RunSteps { get; }

    /// <summary>
    /// Saves all changes to the database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Number of affected rows.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
