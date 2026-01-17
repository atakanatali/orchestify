using MediatR;
using Microsoft.EntityFrameworkCore;
using Orchestify.Application.Common.Interfaces;
using Orchestify.Shared.Errors;
using Orchestify.Shared.Results;

namespace Orchestify.Application.Actions.Workspaces.DeleteWorkspace;

/// <summary>
/// Handler for deleting a workspace.
/// </summary>
public class DeleteWorkspaceHandler : IRequestHandler<DeleteWorkspaceCommand, ServiceResult>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteWorkspaceHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public DeleteWorkspaceHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the deletion of a workspace.
    /// </summary>
    /// <param name="request">The delete command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success or failure.</returns>
    public async Task<ServiceResult> Handle(DeleteWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _context.Workspaces
            .FirstOrDefaultAsync(w => w.Id == request.Id, cancellationToken);

        if (workspace == null)
        {
             return ServiceResult.Failure(ServiceError.Workspaces.NotFound, $"Workspace with ID {request.Id} was not found.");
        }

        _context.Workspaces.Remove(workspace);
        await _context.SaveChangesAsync(cancellationToken);

        return ServiceResult.Success();
    }
}
