using MediatR;
using Orchestify.Contracts.Workspaces;
using Orchestify.Shared.Results;

namespace Orchestify.Application.Actions.Workspaces.CreateWorkspace;

/// <summary>
/// Command to create a new workspace.
/// </summary>
/// <param name="Request">The workspace creation details.</param>
public record CreateWorkspaceCommand(CreateWorkspaceRequestDto Request) : IRequest<ServiceResult<WorkspaceResponseDto>>;
