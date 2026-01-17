using MediatR;
using Orchestify.Contracts.Tasks;
using Orchestify.Shared.Results;

namespace Orchestify.Application.Actions.Tasks.MoveTask;

/// <summary>
/// Command to move a task to a new position or status.
/// </summary>
/// <param name="TaskId">The unique identifier of the task to move.</param>
/// <param name="Request">The move details.</param>
public record MoveTaskCommand(Guid TaskId, MoveTaskRequestDto Request) : IRequest<ServiceResult<TaskResponseDto>>;
