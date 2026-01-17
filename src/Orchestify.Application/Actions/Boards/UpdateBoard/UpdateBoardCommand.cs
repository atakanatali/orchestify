using MediatR;
using Orchestify.Contracts.Boards;
using Orchestify.Shared.Results;

namespace Orchestify.Application.Actions.Boards.UpdateBoard;

/// <summary>
/// Command to update an existing board.
/// </summary>
/// <param name="BoardId">The unique identifier of the board to update.</param>
/// <param name="Request">The update details.</param>
public record UpdateBoardCommand(Guid BoardId, UpdateBoardRequestDto Request) : IRequest<ServiceResult<BoardResponseDto>>;
