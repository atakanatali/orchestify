using MediatR;
using Microsoft.EntityFrameworkCore;
using Orchestify.Application.Common.Interfaces;
using Orchestify.Contracts.Attempts;
using Orchestify.Shared.Errors;
using Orchestify.Shared.Results;

namespace Orchestify.Application.Actions.Attempts.ListRunSteps;

/// <summary>
/// Handler for listing run steps.
/// </summary>
public class ListRunStepsHandler : IRequestHandler<ListRunStepsQuery, ServiceResult<RunStepsListResponseDto>>
{
    private readonly IApplicationDbContext _context;

    public ListRunStepsHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResult<RunStepsListResponseDto>> Handle(ListRunStepsQuery request, CancellationToken cancellationToken)
    {
        var attemptExists = await _context.Attempts.AnyAsync(a => a.Id == request.AttemptId, cancellationToken);
        if (!attemptExists)
        {
            return ServiceResult<RunStepsListResponseDto>.Failure(ServiceError.Tasks.NotFound, $"Attempt {request.AttemptId} not found.");
        }

        var steps = await _context.RunSteps
            .AsNoTracking()
            .Where(s => s.AttemptId == request.AttemptId)
            .OrderBy(s => s.SequenceNumber)
            .Select(s => new RunStepDto
            {
                Id = s.Id,
                AttemptId = s.AttemptId,
                StepType = s.StepType.ToString(),
                Name = s.Name,
                State = s.State.ToString(),
                SequenceNumber = s.SequenceNumber,
                StartedAt = s.StartedAt,
                CompletedAt = s.CompletedAt,
                DurationMs = s.DurationMs,
                ErrorMessage = s.ErrorMessage
            })
            .ToListAsync(cancellationToken);

        return ServiceResult<RunStepsListResponseDto>.Success(new RunStepsListResponseDto { Items = steps });
    }
}
