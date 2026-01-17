using MediatR;
using Orchestify.Contracts.Tasks;
using Orchestify.Shared.Results;

namespace Orchestify.Application.Actions.Tasks.GetTaskById;

/// <summary>
/// Query to get a task by its unique identifier.
/// </summary>
/// <param name="TaskId">The unique identifier of the task.</param>
public record GetTaskByIdQuery(Guid TaskId) : IRequest<ServiceResult<TaskResponseDto>>;
