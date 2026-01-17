using MediatR;
using Microsoft.EntityFrameworkCore;
using Orchestify.Application.Common.Interfaces;
using Orchestify.Contracts.Boards;
using Orchestify.Shared.Errors;
using Orchestify.Shared.Results;

namespace Orchestify.Application.Actions.Boards.UpdateBoard;

/// <summary>
/// Handler for updating an existing board.
/// </summary>
public class UpdateBoardHandler : IRequestHandler<UpdateBoardCommand, ServiceResult<BoardResponseDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateBoardHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public UpdateBoardHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the update of an existing board.
    /// </summary>
    /// <param name="request">The update command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated board response or failure.</returns>
    public async Task<ServiceResult<BoardResponseDto>> Handle(UpdateBoardCommand request, CancellationToken cancellationToken)
    {
        var board = await _context.Boards
            .FirstOrDefaultAsync(b => b.Id == request.BoardId, cancellationToken);

        if (board == null)
        {
            return ServiceResult<BoardResponseDto>.Failure(
                ServiceError.Boards.NotFound,
                $"Board with ID {request.BoardId} was not found.");
        }

        // Check for duplicate name within the same workspace (excluding current board)
        var nameExists = await _context.Boards
            .AnyAsync(b => b.WorkspaceId == board.WorkspaceId 
                          && b.Name == request.Request.Name 
                          && b.Id != request.BoardId, cancellationToken);

        if (nameExists)
        {
            return ServiceResult<BoardResponseDto>.Failure(
                ServiceError.Boards.NameAlreadyExists,
                $"A board with name '{request.Request.Name}' already exists in this workspace.");
        }

        board.Name = request.Request.Name;
        board.Description = request.Request.Description;
        board.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

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
