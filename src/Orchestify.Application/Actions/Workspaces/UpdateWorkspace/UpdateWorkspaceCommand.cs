using MediatR;
using Orchestify.Contracts.Workspaces;
using Orchestify.Shared.Results;

namespace Orchestify.Application.Actions.Workspaces.UpdateWorkspace;

/// <summary>
/// Command to update an existing workspace.
/// </summary>
/// <param name="Id">The unique identifier of the workspace to update.</param>
/// <param name="Request">The update details.</param>
public record UpdateWorkspaceCommand(Guid Id, UpdateWorkspaceRequestDto Request) : IRequest<ServiceResult<WorkspaceResponseDto>>;
