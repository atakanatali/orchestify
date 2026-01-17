using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orchestify.Application.Common.Interfaces;
using Orchestify.Contracts.Tasks;
using Orchestify.Domain.Entities;

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
    public async Task<ActionResult<TaskMessageDto>> SendMessage(Guid taskId, [FromBody] SendMessageRequestDto request)
    {
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
