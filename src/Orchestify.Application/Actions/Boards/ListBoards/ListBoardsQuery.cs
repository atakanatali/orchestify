using MediatR;
using Orchestify.Contracts.Boards;
using Orchestify.Shared.Results;

namespace Orchestify.Application.Actions.Boards.ListBoards;

/// <summary>
/// Query to list all boards within a workspace.
/// </summary>
/// <param name="WorkspaceId">The parent workspace identifier.</param>
public record ListBoardsQuery(Guid WorkspaceId) : IRequest<ServiceResult<BoardsListResponseDto>>;
