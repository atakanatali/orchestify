using MediatR;
using Orchestify.Contracts.Tasks;
using Orchestify.Shared.Results;

namespace Orchestify.Application.Actions.Tasks.UpdateTask;

/// <summary>
/// Command to update an existing task.
/// </summary>
/// <param name="TaskId">The unique identifier of the task to update.</param>
/// <param name="Request">The update details.</param>
public record UpdateTaskCommand(Guid TaskId, UpdateTaskRequestDto Request) : IRequest<ServiceResult<TaskResponseDto>>;
