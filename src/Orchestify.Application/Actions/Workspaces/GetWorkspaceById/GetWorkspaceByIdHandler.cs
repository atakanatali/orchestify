using MediatR;
using Microsoft.EntityFrameworkCore;
using Orchestify.Application.Common.Interfaces;
using Orchestify.Contracts.Workspaces;
using Orchestify.Shared.Errors;
using Orchestify.Shared.Results;

namespace Orchestify.Application.Actions.Workspaces.GetWorkspaceById;

/// <summary>
/// Handler for retrieving a workspace by ID.
/// </summary>
public class GetWorkspaceByIdHandler : IRequestHandler<GetWorkspaceByIdQuery, ServiceResult<WorkspaceResponseDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetWorkspaceByIdHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public GetWorkspaceByIdHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the retrieval of a workspace.
    /// </summary>
    /// <param name="request">The query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The workspace details.</returns>
    public async Task<ServiceResult<WorkspaceResponseDto>> Handle(GetWorkspaceByIdQuery request, CancellationToken cancellationToken)
    {
        var workspace = await _context.Workspaces
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.Id == request.Id, cancellationToken);

        if (workspace == null)
        {
             return ServiceResult<WorkspaceResponseDto>.Failure(ServiceError.Workspaces.NotFound, $"Workspace with ID {request.Id} was not found.");
        }

        var responseDto = new WorkspaceResponseDto
        {
            Workspace = new WorkspaceDto
            {
                Id = workspace.Id,
                Name = workspace.Name,
                RepositoryPath = workspace.RepositoryPath,
                DefaultBranch = workspace.DefaultBranch,
                TotalTasks = workspace.TotalTasks,
                RunningTasks = workspace.RunningTasks,
                LastActivityAt = workspace.LastActivityAt,
                ProgressPercent = workspace.ProgressPercent,
                CreatedAt = workspace.CreatedAt
            }
        };

        return ServiceResult<WorkspaceResponseDto>.Success(responseDto);
    }
}
