using MediatR;
using Microsoft.EntityFrameworkCore;
using Orchestify.Application.Common.Interfaces;
using Orchestify.Contracts.Tasks;
using Orchestify.Domain.Entities;
using Orchestify.Shared.Errors;
using Orchestify.Shared.Results;
using TaskStatus = Orchestify.Domain.Enums.TaskStatus;

namespace Orchestify.Application.Actions.Tasks.CreateTask;

/// <summary>
/// Handler for creating a new task within a board.
/// </summary>
public class CreateTaskHandler : IRequestHandler<CreateTaskCommand, ServiceResult<TaskResponseDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateTaskHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public CreateTaskHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the creation of a new task.
    /// </summary>
    /// <param name="request">The create command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created task response or failure.</returns>
    public async Task<ServiceResult<TaskResponseDto>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        // Verify board exists
        var board = await _context.Boards
            .FirstOrDefaultAsync(b => b.Id == request.BoardId, cancellationToken);

        if (board == null)
        {
            return ServiceResult<TaskResponseDto>.Failure(
                ServiceError.Boards.NotFound,
                $"Board with ID {request.BoardId} was not found.");
        }

        // Get max order key for this board
        var maxOrderKey = await _context.Tasks
            .Where(t => t.BoardId == request.BoardId)
            .MaxAsync(t => (int?)t.OrderKey, cancellationToken) ?? 0;

        var task = new TaskEntity
        {
            Id = Guid.NewGuid(),
            BoardId = request.BoardId,
            Title = request.Request.Title,
            Description = request.Request.Description,
            Status = TaskStatus.Todo,
            Priority = request.Request.Priority,
            OrderKey = maxOrderKey + 1000, // Leave gaps for reordering
            CreatedAt = DateTime.UtcNow,
            AttemptCount = 0
        };

        _context.Tasks.Add(task);

        // Update board task count
        board.TotalTasks++;
        
        await _context.SaveChangesAsync(cancellationToken);

        return ServiceResult<TaskResponseDto>.Success(MapToResponseDto(task));
    }

    private static TaskResponseDto MapToResponseDto(TaskEntity task)
    {
        return new TaskResponseDto
        {
            Task = new TaskDto
            {
                Id = task.Id,
                BoardId = task.BoardId,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status.ToString(),
                OrderKey = task.OrderKey,
                Priority = task.Priority,
                AttemptCount = task.AttemptCount,
                CreatedAt = task.CreatedAt,
                CompletedAt = task.CompletedAt
            }
        };
    }
}
