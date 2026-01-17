using MediatR;
using Microsoft.EntityFrameworkCore;
using Orchestify.Application.Common.Interfaces;
using Orchestify.Contracts.Attempts;
using Orchestify.Shared.Errors;
using Orchestify.Shared.Results;

namespace Orchestify.Application.Actions.Attempts.ListAttempts;

/// <summary>
/// Handler for listing attempts.
/// </summary>
public class ListAttemptsHandler : IRequestHandler<ListAttemptsQuery, ServiceResult<AttemptsListResponseDto>>
{
    private readonly IApplicationDbContext _context;

    public ListAttemptsHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResult<AttemptsListResponseDto>> Handle(ListAttemptsQuery request, CancellationToken cancellationToken)
    {
        var taskExists = await _context.Tasks.AnyAsync(t => t.Id == request.TaskId, cancellationToken);
        if (!taskExists)
        {
            return ServiceResult<AttemptsListResponseDto>.Failure(ServiceError.Tasks.NotFound, $"Task {request.TaskId} not found.");
        }

        var attempts = await _context.Attempts
            .AsNoTracking()
            .Where(a => a.TaskId == request.TaskId)
            .OrderByDescending(a => a.QueuedAt)
            .Select(a => new AttemptDto
            {
                Id = a.Id,
                TaskId = a.TaskId,
                State = a.State.ToString(),
                AttemptNumber = a.AttemptNumber,
                QueuedAt = a.QueuedAt,
                StartedAt = a.StartedAt,
                CompletedAt = a.CompletedAt,
                ErrorMessage = a.ErrorMessage
            })
            .ToListAsync(cancellationToken);

        return ServiceResult<AttemptsListResponseDto>.Success(new AttemptsListResponseDto { Items = attempts });
    }
}
