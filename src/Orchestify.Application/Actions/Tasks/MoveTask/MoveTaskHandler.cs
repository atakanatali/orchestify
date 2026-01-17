using MediatR;
using Microsoft.EntityFrameworkCore;
using Orchestify.Application.Common.Interfaces;
using Orchestify.Contracts.Tasks;
using Orchestify.Shared.Errors;
using Orchestify.Shared.Results;
using TaskStatus = Orchestify.Domain.Enums.TaskStatus;

namespace Orchestify.Application.Actions.Tasks.MoveTask;

/// <summary>
/// Handler for moving a task to a new position or status.
/// Uses optimistic concurrency for safe reordering.
/// </summary>
public class MoveTaskHandler : IRequestHandler<MoveTaskCommand, ServiceResult<TaskResponseDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="MoveTaskHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public MoveTaskHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles moving a task to a new position.
    /// </summary>
    /// <param name="request">The move command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated task response or failure.</returns>
    public async Task<ServiceResult<TaskResponseDto>> Handle(MoveTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _context.Tasks
            .Include(t => t.Board)
            .FirstOrDefaultAsync(t => t.Id == request.TaskId, cancellationToken);

        if (task == null)
        {
            return ServiceResult<TaskResponseDto>.Failure(
                ServiceError.Tasks.NotFound,
                $"Task with ID {request.TaskId} was not found.");
        }

        var previousStatus = task.Status;
        var newOrderKey = request.Request.TargetOrderKey;

        // Handle status change if provided
        if (!string.IsNullOrEmpty(request.Request.NewStatus))
        {
            if (!Enum.TryParse<TaskStatus>(request.Request.NewStatus, true, out var newStatus))
            {
                return ServiceResult<TaskResponseDto>.Failure(
                    ServiceError.Tasks.InvalidStatus,
                    $"Invalid status: {request.Request.NewStatus}.");
            }

            task.Status = newStatus;

            // Update completed timestamp if transitioning to Done
            if (newStatus == TaskStatus.Done && previousStatus != TaskStatus.Done)
            {
                task.CompletedAt = DateTime.UtcNow;
                if (task.Board != null)
                {
                    task.Board.CompletedTasks++;
                }
            }
            else if (newStatus != TaskStatus.Done && previousStatus == TaskStatus.Done)
            {
                task.CompletedAt = null;
                if (task.Board != null)
                {
                    task.Board.CompletedTasks = Math.Max(0, task.Board.CompletedTasks - 1);
                }
            }
        }

        // Calculate new order key if positioning relative to other tasks
        if (request.Request.AfterTaskId.HasValue)
        {
            var afterTask = await _context.Tasks
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == request.Request.AfterTaskId.Value, cancellationToken);

            if (afterTask != null)
            {
                newOrderKey = afterTask.OrderKey + 500;
            }
        }
        else if (request.Request.BeforeTaskId.HasValue)
        {
            var beforeTask = await _context.Tasks
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == request.Request.BeforeTaskId.Value, cancellationToken);

            if (beforeTask != null)
            {
                newOrderKey = beforeTask.OrderKey - 500;
            }
        }

        task.OrderKey = newOrderKey;
        task.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return ServiceResult<TaskResponseDto>.Success(new TaskResponseDto
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
        });
    }
}
