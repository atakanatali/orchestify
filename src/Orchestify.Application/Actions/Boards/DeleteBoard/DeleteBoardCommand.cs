using MediatR;
using Orchestify.Shared.Results;

namespace Orchestify.Application.Actions.Boards.DeleteBoard;

/// <summary>
/// Command to delete a board.
/// </summary>
/// <param name="BoardId">The unique identifier of the board to delete.</param>
public record DeleteBoardCommand(Guid BoardId) : IRequest<ServiceResult>;
