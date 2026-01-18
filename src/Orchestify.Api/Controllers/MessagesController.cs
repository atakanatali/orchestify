using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orchestify.Application.Common.Interfaces;
using Orchestify.Contracts.Tasks;
using Orchestify.Contracts.Shared;
using Orchestify.Domain.Entities;
using Orchestify.Shared.Errors;

namespace Orchestify.Api.Controllers;

[ApiController]
[Route("api/tasks/{taskId:guid}/messages")]
public class MessagesController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public MessagesController(IApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskMessageDto>>> GetMessages(Guid taskId)
    {
        var messages = await _context.TaskMessages
            .Where(m => m.TaskId == taskId)
            .OrderBy(m => m.CreatedAt)
            .Select(m => new TaskMessageDto
            {
                Id = m.Id,
                TaskId = m.TaskId,
                Content = m.Content,
                Sender = m.Sender,
                SuggestedAction = m.SuggestedAction,
                CreatedAt = m.CreatedAt
            })
            .ToListAsync();

        return Ok(messages);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TaskMessageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskMessageDto>> SendMessage(Guid taskId, [FromBody] SendMessageRequestDto request)
    {
        // Check if task exists and get its status
        var task = await _context.Tasks.FindAsync(taskId);
        if (task == null)
        {
            return NotFound(new ApiErrorResponseDto(
                ServiceError.Tasks.NotFound,
                $"Task with ID {taskId} was not found.",
                HttpContext.TraceIdentifier));
        }

        // Block messaging while task is in progress
        if (task.Status == Domain.Enums.TaskStatus.InProgress)
        {
            return BadRequest(new ApiErrorResponseDto(
                ServiceError.Tasks.CannotMessageWhileRunning,
                "Cannot send messages while the task is running. Please wait for the task to complete.",
                HttpContext.TraceIdentifier));
        }

        var message = new TaskMessageEntity
        {
            TaskId = taskId,
            Content = request.Content,
            Sender = "User",
            CreatedAt = DateTime.UtcNow
        };

        _context.TaskMessages.Add(message);
        await _context.SaveChangesAsync(default);

        return Ok(new TaskMessageDto
        {
            Id = message.Id,
            TaskId = message.TaskId,
            Content = message.Content,
            Sender = message.Sender,
            CreatedAt = message.CreatedAt
        });
    }
}
