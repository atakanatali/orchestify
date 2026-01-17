using MediatR;
using Orchestify.Contracts.Attempts;
using Orchestify.Shared.Results;

namespace Orchestify.Application.Actions.Attempts.GetAttemptById;

/// <summary>
/// Query to get an attempt by ID.
/// </summary>
public record GetAttemptByIdQuery(Guid AttemptId) : IRequest<ServiceResult<AttemptDto>>;
