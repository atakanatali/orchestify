using MediatR;
using Orchestify.Shared.Results;

namespace Orchestify.Application.Actions.Attempts.CancelAttempt;

/// <summary>
/// Command to cancel a running attempt.
/// </summary>
/// <param name="AttemptId">The attempt identifier.</param>
public record CancelAttemptCommand(Guid AttemptId) : IRequest<ServiceResult>;
