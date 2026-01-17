using MediatR;
using Orchestify.Contracts.Attempts;
using Orchestify.Shared.Results;

namespace Orchestify.Application.Actions.Tasks.RunTask;

/// <summary>
/// Command to trigger a task execution run.
/// </summary>
/// <param name="TaskId">The unique identifier of the task to run.</param>
public record RunTaskCommand(Guid TaskId) : IRequest<ServiceResult<RunTaskResponseDto>>;
