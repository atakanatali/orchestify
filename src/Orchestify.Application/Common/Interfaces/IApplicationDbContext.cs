using Microsoft.EntityFrameworkCore;
using Orchestify.Domain.Entities;

namespace Orchestify.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<WorkspaceEntity> Workspaces { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
