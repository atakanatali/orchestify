using MediatR;
using Orchestify.Contracts.Attempts;
using Orchestify.Shared.Results;

namespace Orchestify.Application.Actions.Attempts.ListAttempts;

/// <summary>
/// Query to list all attempts for a task.
/// </summary>
/// <param name="TaskId">The task identifier.</param>
public record ListAttemptsQuery(Guid TaskId) : IRequest<ServiceResult<AttemptsListResponseDto>>;
