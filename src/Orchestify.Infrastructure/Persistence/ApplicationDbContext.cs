using Microsoft.EntityFrameworkCore;
using Orchestify.Application.Common.Interfaces;
using Orchestify.Domain.Entities;
using System.Reflection;

namespace Orchestify.Infrastructure.Persistence;

/// <summary>
/// Application database context for Entity Framework Core.
/// </summary>
public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
    /// </summary>
    /// <param name="options">DbContext options.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    /// <inheritdoc />
    public DbSet<WorkspaceEntity> Workspaces => Set<WorkspaceEntity>();

    /// <inheritdoc />
    public DbSet<BoardEntity> Boards => Set<BoardEntity>();

    /// <inheritdoc />
    public DbSet<TaskEntity> Tasks => Set<TaskEntity>();

    /// <inheritdoc />
    public DbSet<AttemptEntity> Attempts => Set<AttemptEntity>();

    /// <inheritdoc />
    public DbSet<RunStepEntity> RunSteps => Set<RunStepEntity>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }
}
