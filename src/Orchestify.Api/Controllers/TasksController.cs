using MediatR;
using Microsoft.AspNetCore.Mvc;
using Orchestify.Application.Actions.Tasks.CreateTask;
using Orchestify.Application.Actions.Tasks.DeleteTask;
using Orchestify.Application.Actions.Tasks.GetTaskById;
using Orchestify.Application.Actions.Tasks.ListTasks;
using Orchestify.Application.Actions.Tasks.UpdateTask;
using Orchestify.Contracts.Shared;
using Orchestify.Contracts.Tasks;
using Orchestify.Shared.Errors;
using Orchestify.Shared.Results;

namespace Orchestify.Api.Controllers;

/// <summary>
/// API controller for task management within boards.
/// </summary>
[ApiController]
[Route("api/boards/{boardId:guid}/tasks")]
public class TasksController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="TasksController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator instance.</param>
    public TasksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Lists all tasks within a board.
    /// </summary>
    /// <param name="boardId">The board identifier.</param>
    /// <returns>List of tasks.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(TasksListResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListTasks(Guid boardId)
    {
        var result = await _mediator.Send(new ListTasksQuery(boardId));

        if (result.IsFailure)
        {
            return MapFailure(result);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Creates a new task within a board.
    /// </summary>
    /// <param name="boardId">The board identifier.</param>
    /// <param name="request">The task creation request.</param>
    /// <returns>The created task.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateTask(Guid boardId, [FromBody] CreateTaskRequestDto request)
    {
        var command = new CreateTaskCommand(boardId, request);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return MapFailure(result);
        }

        return CreatedAtAction(nameof(GetTaskById), new { boardId, taskId = result.Value!.Task.Id }, result.Value);
    }

    /// <summary>
    /// Gets a task by ID.
    /// </summary>
    /// <param name="boardId">The board identifier.</param>
    /// <param name="taskId">The task identifier.</param>
    /// <returns>The task details.</returns>
    [HttpGet("{taskId:guid}")]
    [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTaskById(Guid boardId, Guid taskId)
    {
        var result = await _mediator.Send(new GetTaskByIdQuery(taskId));

        if (result.IsFailure)
        {
            return MapFailure(result);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Updates a task.
    /// </summary>
    /// <param name="boardId">The board identifier.</param>
    /// <param name="taskId">The task identifier.</param>
    /// <param name="request">The update request.</param>
    /// <returns>The updated task.</returns>
    [HttpPut("{taskId:guid}")]
    [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTask(Guid boardId, Guid taskId, [FromBody] UpdateTaskRequestDto request)
    {
        var command = new UpdateTaskCommand(taskId, request);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return MapFailure(result);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Deletes a task.
    /// </summary>
    /// <param name="boardId">The board identifier.</param>
    /// <param name="taskId">The task identifier.</param>
    /// <returns>No content on success.</returns>
    [HttpDelete("{taskId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTask(Guid boardId, Guid taskId)
    {
        var result = await _mediator.Send(new DeleteTaskCommand(taskId));

        if (result.IsFailure)
        {
            return MapFailure(result);
        }

        return NoContent();
    }

    private IActionResult MapFailure(ServiceResult result)
    {
        var errorResponse = new ApiErrorResponseDto(
            result.ErrorCode ?? ServiceError.Core.Unknown,
            result.ErrorMessage ?? "An unknown error occurred.",
            result.CorrelationId ?? HttpContext.TraceIdentifier
        );

        if (result.ErrorCode == ServiceError.Boards.NotFound ||
            result.ErrorCode == ServiceError.Tasks.NotFound)
        {
            return NotFound(errorResponse);
        }

        return BadRequest(errorResponse);
    }

    private IActionResult MapFailure<T>(ServiceResult<T> result)
    {
        var errorResponse = new ApiErrorResponseDto(
            result.ErrorCode ?? ServiceError.Core.Unknown,
            result.ErrorMessage ?? "An unknown error occurred.",
            result.CorrelationId ?? HttpContext.TraceIdentifier
        );

        if (result.ErrorCode == ServiceError.Boards.NotFound ||
            result.ErrorCode == ServiceError.Tasks.NotFound)
        {
            return NotFound(errorResponse);
        }

        return BadRequest(errorResponse);
    }
}
