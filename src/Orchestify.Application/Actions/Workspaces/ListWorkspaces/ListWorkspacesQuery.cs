using MediatR;
using Orchestify.Contracts.Workspaces;
using Orchestify.Shared.Results;

namespace Orchestify.Application.Actions.Workspaces.ListWorkspaces;

/// <summary>
/// Query to list all workspaces.
/// </summary>
public record ListWorkspacesQuery : IRequest<ServiceResult<WorkspacesListResponseDto>>;
