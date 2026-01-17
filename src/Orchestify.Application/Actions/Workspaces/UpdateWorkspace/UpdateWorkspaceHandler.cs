using MediatR;
using Microsoft.EntityFrameworkCore;
using Orchestify.Application.Common.Interfaces;
using Orchestify.Contracts.Workspaces;
using Orchestify.Shared.Errors;
using Orchestify.Shared.Results;

namespace Orchestify.Application.Actions.Workspaces.UpdateWorkspace;

/// <summary>
/// Handler for updating an existing workspace.
/// </summary>
public class UpdateWorkspaceHandler : IRequestHandler<UpdateWorkspaceCommand, ServiceResult<WorkspaceResponseDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateWorkspaceHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public UpdateWorkspaceHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the update of an existing workspace.
    /// </summary>
    /// <param name="request">The update command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated workspace response or failure.</returns>
    public async Task<ServiceResult<WorkspaceResponseDto>> Handle(UpdateWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _context.Workspaces
            .FirstOrDefaultAsync(w => w.Id == request.Id, cancellationToken);

        if (workspace == null)
        {
            return ServiceResult<WorkspaceResponseDto>.Failure(ServiceError.Workspaces.NotFound, $"Workspace with ID {request.Id} was not found.");
        }

        workspace.Name = request.Request.Name;
        workspace.DefaultBranch = request.Request.DefaultBranch;
        workspace.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

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
