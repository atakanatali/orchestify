using MediatR;
using Microsoft.EntityFrameworkCore;
using Orchestify.Application.Common.Interfaces;
using Orchestify.Contracts.Boards;
using Orchestify.Shared.Errors;
using Orchestify.Shared.Results;

namespace Orchestify.Application.Actions.Boards.ListBoards;

/// <summary>
/// Handler for listing all boards within a workspace.
/// </summary>
public class ListBoardsHandler : IRequestHandler<ListBoardsQuery, ServiceResult<BoardsListResponseDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ListBoardsHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public ListBoardsHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the listing of boards.
    /// </summary>
    /// <param name="request">The query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The list of boards.</returns>
    public async Task<ServiceResult<BoardsListResponseDto>> Handle(ListBoardsQuery request, CancellationToken cancellationToken)
    {
        // Verify workspace exists
        var workspaceExists = await _context.Workspaces
            .AnyAsync(w => w.Id == request.WorkspaceId, cancellationToken);

        if (!workspaceExists)
        {
            return ServiceResult<BoardsListResponseDto>.Failure(
                ServiceError.Workspaces.NotFound,
                $"Workspace with ID {request.WorkspaceId} was not found.");
        }

        var boards = await _context.Boards
            .AsNoTracking()
            .Where(b => b.WorkspaceId == request.WorkspaceId)
            .OrderBy(b => b.Name)
            .Select(b => new BoardListItemDto
            {
                Id = b.Id,
                Name = b.Name,
                Description = b.Description,
                TotalTasks = b.TotalTasks,
                CompletedTasks = b.CompletedTasks
            })
            .ToListAsync(cancellationToken);

        return ServiceResult<BoardsListResponseDto>.Success(new BoardsListResponseDto
        {
            Items = boards
        });
    }
}
