using MediatR;
using Microsoft.EntityFrameworkCore;
using Orchestify.Application.Common.Interfaces;
using Orchestify.Domain.Enums;
using Orchestify.Shared.Errors;
using Orchestify.Shared.Results;

namespace Orchestify.Application.Actions.Attempts.CancelAttempt;

/// <summary>
/// Handler for cancelling an attempt.
/// </summary>
public class CancelAttemptHandler : IRequestHandler<CancelAttemptCommand, ServiceResult>
{
    private readonly IApplicationDbContext _context;

    public CancelAttemptHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResult> Handle(CancelAttemptCommand request, CancellationToken cancellationToken)
    {
        var attempt = await _context.Attempts.FirstOrDefaultAsync(a => a.Id == request.AttemptId, cancellationToken);
        
        if (attempt == null)
        {
            return ServiceResult.Failure(ServiceError.Tasks.NotFound, $"Attempt {request.AttemptId} not found.");
        }

        if (attempt.State is AttemptState.Succeeded or AttemptState.Failed or AttemptState.Cancelled)
        {
            return ServiceResult.Failure(ServiceError.Tasks.AlreadyCompleted, "Attempt already completed.");
        }

        attempt.State = AttemptState.Cancelling;
        attempt.CancellationReason = "User requested cancellation";
        await _context.SaveChangesAsync(cancellationToken);

        return ServiceResult.Success();
    }
}
