using MediatR;
using Microsoft.EntityFrameworkCore;
using Orchestify.Application.Common.Interfaces;
using Orchestify.Contracts.Tasks;
using Orchestify.Shared.Errors;
using Orchestify.Shared.Results;

namespace Orchestify.Application.Actions.Tasks.ListTasks;

/// <summary>
/// Handler for listing all tasks within a board.
/// </summary>
public class ListTasksHandler : IRequestHandler<ListTasksQuery, ServiceResult<TasksListResponseDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ListTasksHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public ListTasksHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the listing of tasks.
    /// </summary>
    /// <param name="request">The query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The list of tasks.</returns>
    public async Task<ServiceResult<TasksListResponseDto>> Handle(ListTasksQuery request, CancellationToken cancellationToken)
    {
        // Verify board exists
        var boardExists = await _context.Boards
            .AnyAsync(b => b.Id == request.BoardId, cancellationToken);

        if (!boardExists)
        {
            return ServiceResult<TasksListResponseDto>.Failure(
                ServiceError.Boards.NotFound,
                $"Board with ID {request.BoardId} was not found.");
        }

        var tasks = await _context.Tasks
            .AsNoTracking()
            .Where(t => t.BoardId == request.BoardId)
            .OrderBy(t => t.OrderKey)
            .Select(t => new TaskListItemDto
            {
                Id = t.Id,
                Title = t.Title,
                Status = t.Status.ToString(),
                OrderKey = t.OrderKey,
                Priority = t.Priority
            })
            .ToListAsync(cancellationToken);

        return ServiceResult<TasksListResponseDto>.Success(new TasksListResponseDto
        {
            Items = tasks
        });
    }
}
