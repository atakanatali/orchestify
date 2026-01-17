using MediatR;
using Microsoft.EntityFrameworkCore;
using Orchestify.Application.Common.Interfaces;
using Orchestify.Contracts.Boards;
using Orchestify.Shared.Errors;
using Orchestify.Shared.Results;

namespace Orchestify.Application.Actions.Boards.GetBoardById;

/// <summary>
/// Handler for retrieving a board by ID.
/// </summary>
public class GetBoardByIdHandler : IRequestHandler<GetBoardByIdQuery, ServiceResult<BoardResponseDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetBoardByIdHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public GetBoardByIdHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the retrieval of a board.
    /// </summary>
    /// <param name="request">The query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The board details.</returns>
    public async Task<ServiceResult<BoardResponseDto>> Handle(GetBoardByIdQuery request, CancellationToken cancellationToken)
    {
        var board = await _context.Boards
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == request.BoardId, cancellationToken);

        if (board == null)
        {
            return ServiceResult<BoardResponseDto>.Failure(
                ServiceError.Boards.NotFound,
                $"Board with ID {request.BoardId} was not found.");
        }

        return ServiceResult<BoardResponseDto>.Success(new BoardResponseDto
        {
            Board = new BoardDto
            {
                Id = board.Id,
                WorkspaceId = board.WorkspaceId,
                Name = board.Name,
                Description = board.Description,
                TotalTasks = board.TotalTasks,
                CompletedTasks = board.CompletedTasks,
                CreatedAt = board.CreatedAt
            }
        });
    }
}
