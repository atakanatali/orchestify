using MediatR;
using Orchestify.Contracts.Boards;
using Orchestify.Shared.Results;

namespace Orchestify.Application.Actions.Boards.CreateBoard;

/// <summary>
/// Command to create a new board within a workspace.
/// </summary>
/// <param name="WorkspaceId">The parent workspace identifier.</param>
/// <param name="Request">The board creation details.</param>
public record CreateBoardCommand(Guid WorkspaceId, CreateBoardRequestDto Request) : IRequest<ServiceResult<BoardResponseDto>>;
