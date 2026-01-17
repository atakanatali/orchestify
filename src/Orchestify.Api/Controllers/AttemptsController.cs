using MediatR;
using Microsoft.AspNetCore.Mvc;
using Orchestify.Application.Actions.Attempts.CancelAttempt;
using Orchestify.Application.Actions.Attempts.GetAttemptById;
using Orchestify.Application.Actions.Attempts.ListAttempts;
using Orchestify.Application.Actions.Attempts.ListRunSteps;
using Orchestify.Contracts.Attempts;
using Orchestify.Contracts.Shared;
using Orchestify.Shared.Errors;
using Orchestify.Shared.Results;

namespace Orchestify.Api.Controllers;

/// <summary>
/// API controller for attempt management.
/// </summary>
[ApiController]
[Route("api/tasks/{taskId:guid}/attempts")]
public class AttemptsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AttemptsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Lists all attempts for a task.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(AttemptsListResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListAttempts(Guid taskId)
    {
        var result = await _mediator.Send(new ListAttemptsQuery(taskId));
        if (result.IsFailure) return MapFailure(result);
        return Ok(result.Value);
    }

    /// <summary>
    /// Gets an attempt by ID.
    /// </summary>
    [HttpGet("{attemptId:guid}")]
    [ProducesResponseType(typeof(AttemptDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAttemptById(Guid taskId, Guid attemptId)
    {
        var result = await _mediator.Send(new GetAttemptByIdQuery(attemptId));
        if (result.IsFailure) return MapFailure(result);
        return Ok(result.Value);
    }

    /// <summary>
    /// Lists run steps for an attempt.
    /// </summary>
    [HttpGet("{attemptId:guid}/steps")]
    [ProducesResponseType(typeof(RunStepsListResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListRunSteps(Guid taskId, Guid attemptId)
    {
        var result = await _mediator.Send(new ListRunStepsQuery(attemptId));
        if (result.IsFailure) return MapFailure(result);
        return Ok(result.Value);
    }

    /// <summary>
    /// Cancels a running attempt.
    /// </summary>
    [HttpPost("{attemptId:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelAttempt(Guid taskId, Guid attemptId)
    {
        var result = await _mediator.Send(new CancelAttemptCommand(attemptId));
        if (result.IsFailure) return MapFailure(result);
        return NoContent();
    }

    private IActionResult MapFailure(ServiceResult result)
    {
        var error = new ApiErrorResponseDto(
            result.ErrorCode ?? ServiceError.Core.Unknown,
            result.ErrorMessage ?? "Unknown error",
            HttpContext.TraceIdentifier);
        if (result.ErrorCode == ServiceError.Tasks.NotFound) return NotFound(error);
        return BadRequest(error);
    }

    private IActionResult MapFailure<T>(ServiceResult<T> result)
    {
        var error = new ApiErrorResponseDto(
            result.ErrorCode ?? ServiceError.Core.Unknown,
            result.ErrorMessage ?? "Unknown error",
            HttpContext.TraceIdentifier);
        if (result.ErrorCode == ServiceError.Tasks.NotFound) return NotFound(error);
        return BadRequest(error);
    }
}
