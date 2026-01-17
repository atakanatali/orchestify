using MediatR;
using Microsoft.EntityFrameworkCore;
using Orchestify.Application.Common.Interfaces;
using Orchestify.Contracts.Attempts;
using Orchestify.Domain.Entities;
using Orchestify.Domain.Enums;
using Orchestify.Shared.Errors;
using Orchestify.Shared.Results;

namespace Orchestify.Application.Actions.Tasks.RunTask;

/// <summary>
/// Handler for triggering a task execution run.
/// Creates a new attempt in Queued state and returns immediately (202 pattern).
/// </summary>
public class RunTaskHandler : IRequestHandler<RunTaskCommand, ServiceResult<RunTaskResponseDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="RunTaskHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public RunTaskHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the task run trigger.
    /// </summary>
    /// <param name="request">The run command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created attempt response.</returns>
    public async Task<ServiceResult<RunTaskResponseDto>> Handle(RunTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == request.TaskId, cancellationToken);

        if (task == null)
        {
            return ServiceResult<RunTaskResponseDto>.Failure(
                ServiceError.Tasks.NotFound,
                $"Task with ID {request.TaskId} was not found.");
        }

        // Get next attempt number
        var maxAttemptNumber = await _context.Attempts
            .Where(a => a.TaskId == request.TaskId)
            .MaxAsync(a => (int?)a.AttemptNumber, cancellationToken) ?? 0;

        var attempt = new AttemptEntity
        {
            Id = Guid.NewGuid(),
            TaskId = request.TaskId,
            State = AttemptState.Queued,
            AttemptNumber = maxAttemptNumber + 1,
            QueuedAt = DateTime.UtcNow,
            CorrelationId = Guid.NewGuid().ToString()
        };

        _context.Attempts.Add(attempt);

        // Update task attempt count and status
        task.AttemptCount++;
        task.Status = Domain.Enums.TaskStatus.InProgress;
        task.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return ServiceResult<RunTaskResponseDto>.Success(new RunTaskResponseDto
        {
            Attempt = new AttemptDto
            {
                Id = attempt.Id,
                TaskId = attempt.TaskId,
                State = attempt.State.ToString(),
                AttemptNumber = attempt.AttemptNumber,
                QueuedAt = attempt.QueuedAt,
                StartedAt = attempt.StartedAt,
                CompletedAt = attempt.CompletedAt,
                ErrorMessage = attempt.ErrorMessage
            }
        });
    }
}
