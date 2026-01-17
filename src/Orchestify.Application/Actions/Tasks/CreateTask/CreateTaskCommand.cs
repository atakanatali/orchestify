using MediatR;
using Orchestify.Contracts.Tasks;
using Orchestify.Shared.Results;

namespace Orchestify.Application.Actions.Tasks.CreateTask;

/// <summary>
/// Command to create a new task within a board.
/// </summary>
/// <param name="BoardId">The parent board identifier.</param>
/// <param name="Request">The task creation details.</param>
public record CreateTaskCommand(Guid BoardId, CreateTaskRequestDto Request) : IRequest<ServiceResult<TaskResponseDto>>;
