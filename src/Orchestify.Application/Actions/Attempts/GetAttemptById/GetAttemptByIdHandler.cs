using MediatR;
using Microsoft.EntityFrameworkCore;
using Orchestify.Application.Common.Interfaces;
using Orchestify.Contracts.Attempts;
using Orchestify.Shared.Errors;
using Orchestify.Shared.Results;

namespace Orchestify.Application.Actions.Attempts.GetAttemptById;

/// <summary>
/// Handler for getting attempt by ID.
/// </summary>
public class GetAttemptByIdHandler : IRequestHandler<GetAttemptByIdQuery, ServiceResult<AttemptDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAttemptByIdHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResult<AttemptDto>> Handle(GetAttemptByIdQuery request, CancellationToken cancellationToken)
    {
        var attempt = await _context.Attempts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == request.AttemptId, cancellationToken);

        if (attempt == null)
        {
            return ServiceResult<AttemptDto>.Failure(ServiceError.Tasks.NotFound, $"Attempt {request.AttemptId} not found.");
        }

        return ServiceResult<AttemptDto>.Success(new AttemptDto
        {
            Id = attempt.Id,
            TaskId = attempt.TaskId,
            State = attempt.State.ToString(),
            AttemptNumber = attempt.AttemptNumber,
            QueuedAt = attempt.QueuedAt,
            StartedAt = attempt.StartedAt,
            CompletedAt = attempt.CompletedAt,
            ErrorMessage = attempt.ErrorMessage
        });
    }
}
