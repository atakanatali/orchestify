using MediatR;
using Microsoft.AspNetCore.Mvc;
using Orchestify.Application.Actions.Boards.CreateBoard;
using Orchestify.Application.Actions.Boards.DeleteBoard;
using Orchestify.Application.Actions.Boards.GetBoardById;
using Orchestify.Application.Actions.Boards.ListBoards;
using Orchestify.Application.Actions.Boards.UpdateBoard;
using Orchestify.Contracts.Boards;
using Orchestify.Contracts.Shared;
using Orchestify.Shared.Errors;
using Orchestify.Shared.Results;

namespace Orchestify.Api.Controllers;

/// <summary>
/// API controller for board management within workspaces.
/// </summary>
[ApiController]
[Route("api/workspaces/{workspaceId:guid}/boards")]
public class BoardsController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="BoardsController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator instance.</param>
    public BoardsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Lists all boards within a workspace.
    /// </summary>
    /// <param name="workspaceId">The workspace identifier.</param>
    /// <returns>List of boards.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(BoardsListResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListBoards(Guid workspaceId)
    {
        var result = await _mediator.Send(new ListBoardsQuery(workspaceId));

        if (result.IsFailure)
        {
            return MapFailure(result);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Creates a new board within a workspace.
    /// </summary>
    /// <param name="workspaceId">The workspace identifier.</param>
    /// <param name="request">The board creation request.</param>
    /// <returns>The created board.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(BoardResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateBoard(Guid workspaceId, [FromBody] CreateBoardRequestDto request)
    {
        var command = new CreateBoardCommand(workspaceId, request);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return MapFailure(result);
        }

        return CreatedAtAction(nameof(GetBoardById), new { workspaceId, boardId = result.Value!.Board.Id }, result.Value);
    }

    /// <summary>
    /// Gets a board by ID.
    /// </summary>
    /// <param name="workspaceId">The workspace identifier.</param>
    /// <param name="boardId">The board identifier.</param>
    /// <returns>The board details.</returns>
    [HttpGet("{boardId:guid}")]
    [ProducesResponseType(typeof(BoardResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBoardById(Guid workspaceId, Guid boardId)
    {
        var result = await _mediator.Send(new GetBoardByIdQuery(boardId));

        if (result.IsFailure)
        {
            return MapFailure(result);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Updates a board.
    /// </summary>
    /// <param name="workspaceId">The workspace identifier.</param>
    /// <param name="boardId">The board identifier.</param>
    /// <param name="request">The update request.</param>
    /// <returns>The updated board.</returns>
    [HttpPut("{boardId:guid}")]
    [ProducesResponseType(typeof(BoardResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateBoard(Guid workspaceId, Guid boardId, [FromBody] UpdateBoardRequestDto request)
    {
        var command = new UpdateBoardCommand(boardId, request);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return MapFailure(result);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Deletes a board.
    /// </summary>
    /// <param name="workspaceId">The workspace identifier.</param>
    /// <param name="boardId">The board identifier.</param>
    /// <returns>No content on success.</returns>
    [HttpDelete("{boardId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBoard(Guid workspaceId, Guid boardId)
    {
        var result = await _mediator.Send(new DeleteBoardCommand(boardId));

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

        if (result.ErrorCode == ServiceError.Workspaces.NotFound ||
            result.ErrorCode == ServiceError.Boards.NotFound)
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

        if (result.ErrorCode == ServiceError.Workspaces.NotFound ||
            result.ErrorCode == ServiceError.Boards.NotFound)
        {
            return NotFound(errorResponse);
        }

        return BadRequest(errorResponse);
    }
}
