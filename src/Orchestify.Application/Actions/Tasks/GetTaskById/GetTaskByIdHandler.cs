using MediatR;
using Microsoft.EntityFrameworkCore;
using Orchestify.Application.Common.Interfaces;
using Orchestify.Contracts.Tasks;
using Orchestify.Shared.Errors;
using Orchestify.Shared.Results;

namespace Orchestify.Application.Actions.Tasks.GetTaskById;

/// <summary>
/// Handler for retrieving a task by ID.
/// </summary>
public class GetTaskByIdHandler : IRequestHandler<GetTaskByIdQuery, ServiceResult<TaskResponseDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetTaskByIdHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public GetTaskByIdHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the retrieval of a task.
    /// </summary>
    /// <param name="request">The query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The task details.</returns>
    public async Task<ServiceResult<TaskResponseDto>> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        var task = await _context.Tasks
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == request.TaskId, cancellationToken);

        if (task == null)
        {
            return ServiceResult<TaskResponseDto>.Failure(
                ServiceError.Tasks.NotFound,
                $"Task with ID {request.TaskId} was not found.");
        }

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
