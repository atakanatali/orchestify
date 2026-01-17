using MediatR;
using Orchestify.Shared.Results;

namespace Orchestify.Application.Actions.Tasks.DeleteTask;

/// <summary>
/// Command to delete a task.
/// </summary>
/// <param name="TaskId">The unique identifier of the task to delete.</param>
public record DeleteTaskCommand(Guid TaskId) : IRequest<ServiceResult>;
