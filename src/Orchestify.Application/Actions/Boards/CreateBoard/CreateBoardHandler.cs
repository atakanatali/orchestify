using MediatR;
using Microsoft.EntityFrameworkCore;
using Orchestify.Application.Common.Interfaces;
using Orchestify.Contracts.Boards;
using Orchestify.Domain.Entities;
using Orchestify.Shared.Errors;
using Orchestify.Shared.Results;

namespace Orchestify.Application.Actions.Boards.CreateBoard;

/// <summary>
/// Handler for creating a new board within a workspace.
/// </summary>
public class CreateBoardHandler : IRequestHandler<CreateBoardCommand, ServiceResult<BoardResponseDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateBoardHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public CreateBoardHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the creation of a new board.
    /// </summary>
    /// <param name="request">The create command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created board response or failure.</returns>
    public async Task<ServiceResult<BoardResponseDto>> Handle(CreateBoardCommand request, CancellationToken cancellationToken)
    {
        // Verify workspace exists
        var workspaceExists = await _context.Workspaces
            .AnyAsync(w => w.Id == request.WorkspaceId, cancellationToken);

        if (!workspaceExists)
        {
            return ServiceResult<BoardResponseDto>.Failure(
                ServiceError.Workspaces.NotFound,
                $"Workspace with ID {request.WorkspaceId} was not found.");
        }

        // Check for duplicate board name within workspace
        var nameExists = await _context.Boards
            .AnyAsync(b => b.WorkspaceId == request.WorkspaceId && b.Name == request.Request.Name, cancellationToken);

        if (nameExists)
        {
            return ServiceResult<BoardResponseDto>.Failure(
                ServiceError.Boards.NameAlreadyExists,
                $"A board with name '{request.Request.Name}' already exists in this workspace.");
        }

        var board = new BoardEntity
        {
            Id = Guid.NewGuid(),
            WorkspaceId = request.WorkspaceId,
            Name = request.Request.Name,
            Description = request.Request.Description,
            CreatedAt = DateTime.UtcNow,
            TotalTasks = 0,
            CompletedTasks = 0
        };

        _context.Boards.Add(board);
        await _context.SaveChangesAsync(cancellationToken);

        return ServiceResult<BoardResponseDto>.Success(MapToResponseDto(board));
    }

    private static BoardResponseDto MapToResponseDto(BoardEntity board)
    {
        return new BoardResponseDto
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
        };
    }
}
