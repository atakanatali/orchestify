using MediatR;
using Orchestify.Contracts.Workspaces;
using Orchestify.Shared.Results;

namespace Orchestify.Application.Actions.Workspaces.GetWorkspaceById;

/// <summary>
/// Query to get a workspace by its unique identifier.
/// </summary>
/// <param name="Id">The unique identifier of the workspace.</param>
public record GetWorkspaceByIdQuery(Guid Id) : IRequest<ServiceResult<WorkspaceResponseDto>>;
