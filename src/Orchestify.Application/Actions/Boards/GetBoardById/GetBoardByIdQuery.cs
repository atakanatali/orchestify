using MediatR;
using Orchestify.Contracts.Boards;
using Orchestify.Shared.Results;

namespace Orchestify.Application.Actions.Boards.GetBoardById;

/// <summary>
/// Query to get a board by its unique identifier.
/// </summary>
/// <param name="BoardId">The unique identifier of the board.</param>
public record GetBoardByIdQuery(Guid BoardId) : IRequest<ServiceResult<BoardResponseDto>>;
