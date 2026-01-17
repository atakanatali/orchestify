using MediatR;
using Orchestify.Contracts.Attempts;
using Orchestify.Shared.Results;

namespace Orchestify.Application.Actions.Attempts.ListRunSteps;

/// <summary>
/// Query to list all run steps for an attempt.
/// </summary>
public record ListRunStepsQuery(Guid AttemptId) : IRequest<ServiceResult<RunStepsListResponseDto>>;
