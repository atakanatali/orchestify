using MediatR;
using Orchestify.Application.Common.Interfaces;
using Orchestify.Contracts.Workspaces;
using Orchestify.Domain.Entities;
using Orchestify.Shared.Results;

namespace Orchestify.Application.Actions.Workspaces.CreateWorkspace;

/// <summary>
/// Handler for creating a new workspace.
/// </summary>
public class CreateWorkspaceHandler : IRequestHandler<CreateWorkspaceCommand, ServiceResult<WorkspaceResponseDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateWorkspaceHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public CreateWorkspaceHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the creation of a new workspace.
    /// </summary>
    /// <param name="request">The create command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created workspace response.</returns>
    public async Task<ServiceResult<WorkspaceResponseDto>> Handle(CreateWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Request;

        var workspace = new WorkspaceEntity
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            RepositoryPath = dto.RepositoryPath,
            DefaultBranch = dto.DefaultBranch,
            CreatedAt = DateTime.UtcNow,
            TotalTasks = 0,
            RunningTasks = 0,
            ProgressPercent = 0
        };

        _context.Workspaces.Add(workspace);
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
