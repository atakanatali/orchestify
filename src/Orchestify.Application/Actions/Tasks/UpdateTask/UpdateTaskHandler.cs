using MediatR;
using Microsoft.EntityFrameworkCore;
using Orchestify.Application.Common.Interfaces;
using Orchestify.Contracts.Tasks;
using Orchestify.Shared.Errors;
using Orchestify.Shared.Results;
using TaskStatus = Orchestify.Domain.Enums.TaskStatus;

namespace Orchestify.Application.Actions.Tasks.UpdateTask;

/// <summary>
/// Handler for updating an existing task.
/// </summary>
public class UpdateTaskHandler : IRequestHandler<UpdateTaskCommand, ServiceResult<TaskResponseDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateTaskHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public UpdateTaskHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the update of an existing task.
    /// </summary>
    /// <param name="request">The update command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated task response or failure.</returns>
    public async Task<ServiceResult<TaskResponseDto>> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
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

        // Parse and validate status
        if (!Enum.TryParse<TaskStatus>(request.Request.Status, true, out var newStatus))
        {
            return ServiceResult<TaskResponseDto>.Failure(
                ServiceError.Tasks.InvalidStatus,
                $"Invalid status: {request.Request.Status}. Valid values: Todo, InProgress, Review, Done, Cancelled.");
        }

        var previousStatus = task.Status;
        
        task.Title = request.Request.Title;
        task.Description = request.Request.Description;
        task.Status = newStatus;
        task.Priority = request.Request.Priority;
        task.UpdatedAt = DateTime.UtcNow;

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
