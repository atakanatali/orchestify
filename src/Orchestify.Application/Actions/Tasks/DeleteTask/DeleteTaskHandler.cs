using MediatR;
using Microsoft.EntityFrameworkCore;
using Orchestify.Application.Common.Interfaces;
using Orchestify.Shared.Errors;
using Orchestify.Shared.Results;
using TaskStatus = Orchestify.Domain.Enums.TaskStatus;

namespace Orchestify.Application.Actions.Tasks.DeleteTask;

/// <summary>
/// Handler for deleting a task.
/// </summary>
public class DeleteTaskHandler : IRequestHandler<DeleteTaskCommand, ServiceResult>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteTaskHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public DeleteTaskHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the deletion of a task.
    /// </summary>
    /// <param name="request">The delete command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success or failure.</returns>
    public async Task<ServiceResult> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _context.Tasks
            .Include(t => t.Board)
            .FirstOrDefaultAsync(t => t.Id == request.TaskId, cancellationToken);

        if (task == null)
        {
            return ServiceResult.Failure(
                ServiceError.Tasks.NotFound,
                $"Task with ID {request.TaskId} was not found.");
        }

        // Update board counts
        if (task.Board != null)
        {
            task.Board.TotalTasks = Math.Max(0, task.Board.TotalTasks - 1);
            if (task.Status == TaskStatus.Done)
            {
                task.Board.CompletedTasks = Math.Max(0, task.Board.CompletedTasks - 1);
            }
        }

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync(cancellationToken);

        return ServiceResult.Success();
    }
}
