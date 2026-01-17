using MediatR;
using Microsoft.EntityFrameworkCore;
using Orchestify.Application.Common.Interfaces;
using Orchestify.Shared.Errors;
using Orchestify.Shared.Results;

namespace Orchestify.Application.Actions.Boards.DeleteBoard;

/// <summary>
/// Handler for deleting a board.
/// </summary>
public class DeleteBoardHandler : IRequestHandler<DeleteBoardCommand, ServiceResult>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteBoardHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public DeleteBoardHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the deletion of a board.
    /// </summary>
    /// <param name="request">The delete command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success or failure.</returns>
    public async Task<ServiceResult> Handle(DeleteBoardCommand request, CancellationToken cancellationToken)
    {
        var board = await _context.Boards
            .FirstOrDefaultAsync(b => b.Id == request.BoardId, cancellationToken);

        if (board == null)
        {
            return ServiceResult.Failure(
                ServiceError.Boards.NotFound,
                $"Board with ID {request.BoardId} was not found.");
        }

        _context.Boards.Remove(board);
        await _context.SaveChangesAsync(cancellationToken);

        return ServiceResult.Success();
    }
}
