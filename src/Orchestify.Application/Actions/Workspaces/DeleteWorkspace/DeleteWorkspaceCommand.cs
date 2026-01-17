using MediatR;
using Orchestify.Shared.Results;

namespace Orchestify.Application.Actions.Workspaces.DeleteWorkspace;

/// <summary>
/// Command to delete a workspace.
/// </summary>
/// <param name="Id">The unique identifier of the workspace to delete.</param>
public record DeleteWorkspaceCommand(Guid Id) : IRequest<ServiceResult>;
