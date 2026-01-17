using MediatR;
using Microsoft.AspNetCore.Mvc;
using Orchestify.Application.Actions.Workspaces.CreateWorkspace;
using Orchestify.Application.Actions.Workspaces.DeleteWorkspace;
using Orchestify.Application.Actions.Workspaces.GetWorkspaceById;
using Orchestify.Application.Actions.Workspaces.ListWorkspaces;
using Orchestify.Application.Actions.Workspaces.UpdateWorkspace;
using Orchestify.Contracts.Shared;
using Orchestify.Contracts.Workspaces;
using Orchestify.Shared.Errors;
using Orchestify.Shared.Results;

namespace Orchestify.Api.Controllers;

[ApiController]
[Route("api/workspaces")]
public class WorkspacesController : ControllerBase
{
    private readonly IMediator _mediator;

    public WorkspacesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Lists all workspaces.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(WorkspacesListResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListWorkspaces()
    {
        var result = await _mediator.Send(new ListWorkspacesQuery());
        return Ok(result.Value);
    }

    /// <summary>
    /// Creates a new workspace.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(WorkspaceResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateWorkspace([FromBody] CreateWorkspaceRequestDto request)
    {
        var command = new CreateWorkspaceCommand(request);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return MapFailure(result);
        }

        return CreatedAtAction(nameof(GetWorkspaceById), new { id = result.Value!.Workspace.Id }, result.Value);
    }

    /// <summary>
    /// Gets a workspace by ID.
    /// </summary>
    [HttpGet("{workspaceId:guid}")]
    [ProducesResponseType(typeof(WorkspaceResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWorkspaceById(Guid workspaceId)
    {
        var result = await _mediator.Send(new GetWorkspaceByIdQuery(workspaceId));

        if (result.IsFailure)
        {
            return MapFailure(result);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Updates a workspace.
    /// </summary>
    [HttpPut("{workspaceId:guid}")]
    [ProducesResponseType(typeof(WorkspaceResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateWorkspace(Guid workspaceId, [FromBody] UpdateWorkspaceRequestDto request)
    {
        var command = new UpdateWorkspaceCommand(workspaceId, request);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return MapFailure(result);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Deletes a workspace.
    /// </summary>
    [HttpDelete("{workspaceId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteWorkspace(Guid workspaceId)
    {
        var result = await _mediator.Send(new DeleteWorkspaceCommand(workspaceId));

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

        if (result.ErrorCode == ServiceError.Workspaces.NotFound)
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

        if (result.ErrorCode == ServiceError.Workspaces.NotFound)
        {
            return NotFound(errorResponse);
        }

        return BadRequest(errorResponse);
    }
}
