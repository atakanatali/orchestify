using MediatR;
using Orchestify.Application.Common.Interfaces;
using Orchestify.Domain.Entities;
using Orchestify.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace Orchestify.Application.Actions.Tasks.SendTaskMessage;

public record SendTaskMessageCommand(Guid TaskId, string Content) : IRequest<ServiceResult<TaskMessageDto>>;

public record TaskMessageDto(
    Guid Id,
    string Content,
    string Sender,
    DateTime CreatedAt
);

public class SendTaskMessageHandler : IRequestHandler<SendTaskMessageCommand, ServiceResult<TaskMessageDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IAgentService _agentService;

    public SendTaskMessageHandler(IApplicationDbContext context, IAgentService agentService)
    {
        _context = context;
        _agentService = agentService;
    }

    public async Task<ServiceResult<TaskMessageDto>> Handle(SendTaskMessageCommand request, CancellationToken cancellationToken)
    {
        var task = await _context.Tasks
            .Include(t => t.Board)
                .ThenInclude(b => b!.Workspace)
            .FirstOrDefaultAsync(t => t.Id == request.TaskId, cancellationToken);
            
        if (task == null) return ServiceResult<TaskMessageDto>.Failure("TASK_NOT_FOUND", "Task not found");

        // 1. Save User Message
        var userMessage = new TaskMessageEntity
        {
            Id = Guid.NewGuid(),
            TaskId = request.TaskId,
            Content = request.Content,
            Sender = MessageSender.User,
            CreatedAt = DateTime.UtcNow
        };

        _context.TaskMessages.Add(userMessage);
        
        // 2. Process with Antigravity Agent
        var agentResponse = await _agentService.ProcessMessageAsync(task, request.Content, cancellationToken);
        
        var agentMessage = new TaskMessageEntity
        {
            Id = Guid.NewGuid(),
            TaskId = request.TaskId,
            Content = agentResponse.Content,
            Sender = MessageSender.Agent,
            SuggestedAction = agentResponse.SuggestedAction != null 
                ? System.Text.Json.JsonSerializer.Serialize(agentResponse.SuggestedAction) 
                : null,
            CreatedAt = DateTime.UtcNow.AddMilliseconds(200)
        };

        _context.TaskMessages.Add(agentMessage);
        
        await _context.SaveChangesAsync(cancellationToken);

        return ServiceResult<TaskMessageDto>.Success(new TaskMessageDto(
            userMessage.Id,
            userMessage.Content,
            userMessage.Sender.ToString(),
            userMessage.CreatedAt
        ));
    }
}
