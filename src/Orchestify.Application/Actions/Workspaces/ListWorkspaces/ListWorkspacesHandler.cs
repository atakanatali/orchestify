using MediatR;
using Microsoft.EntityFrameworkCore;
using Orchestify.Application.Common.Interfaces;
using Orchestify.Contracts.Workspaces;
using Orchestify.Shared.Results;

namespace Orchestify.Application.Actions.Workspaces.ListWorkspaces;

/// <summary>
/// Handler for listing all workspaces.
/// </summary>
public class ListWorkspacesHandler : IRequestHandler<ListWorkspacesQuery, ServiceResult<WorkspacesListResponseDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ListWorkspacesHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public ListWorkspacesHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the listing of workspaces.
    /// </summary>
    /// <param name="request">The query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The list of workspaces.</returns>
    public async Task<ServiceResult<WorkspacesListResponseDto>> Handle(ListWorkspacesQuery request, CancellationToken cancellationToken)
    {
        var workspaces = await _context.Workspaces
            .AsNoTracking()
            .OrderByDescending(w => w.LastActivityAt)
            .Select(w => new WorkspaceListItemDto
            {
                Id = w.Id,
                Name = w.Name,
                RepositoryPath = w.RepositoryPath,
                DefaultBranch = w.DefaultBranch,
                TotalTasks = w.TotalTasks,
                RunningTasks = w.RunningTasks,
                LastActivityAt = w.LastActivityAt,
                ProgressPercent = w.ProgressPercent
            })
            .ToListAsync(cancellationToken);

        return ServiceResult<WorkspacesListResponseDto>.Success(new WorkspacesListResponseDto
        {
            Items = workspaces
        });
    }
}
