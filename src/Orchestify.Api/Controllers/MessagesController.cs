using MediatR;
using Microsoft.AspNetCore.Mvc;
using Orchestify.Application.Actions.Tasks.GetTaskMessages;
using Orchestify.Application.Actions.Tasks.SendTaskMessage;

namespace Orchestify.Api.Controllers;

[ApiController]
[Route("api/tasks/{taskId:guid}/messages")]
public class MessagesController : ControllerBase
{
    private readonly IMediator _mediator;

    public MessagesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetMessages(Guid taskId)
    {
        var result = await _mediator.Send(new GetTaskMessagesQuery(taskId));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage(Guid taskId, [FromBody] SendMessageRequest request)
    {
        var result = await _mediator.Send(new SendTaskMessageCommand(taskId, request.Content));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
    }
}

public record SendMessageRequest(string Content);
