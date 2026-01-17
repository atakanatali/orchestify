using MediatR;
using Orchestify.Contracts.Tasks;
using Orchestify.Shared.Results;

namespace Orchestify.Application.Actions.Tasks.ListTasks;

/// <summary>
/// Query to list all tasks within a board.
/// </summary>
/// <param name="BoardId">The parent board identifier.</param>
public record ListTasksQuery(Guid BoardId) : IRequest<ServiceResult<TasksListResponseDto>>;
