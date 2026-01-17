using MediatR;
using Microsoft.EntityFrameworkCore;
using Orchestify.Application.Common.Interfaces;
using Orchestify.Domain.Entities;
using Orchestify.Shared.Results;

namespace Orchestify.Application.Actions.Tasks.GetTaskMessages;

public record GetTaskMessagesQuery(Guid TaskId) : IRequest<ServiceResult<List<TaskMessageDto>>>;

public record TaskMessageDto(
    Guid Id,
    string Content,
    string Sender,
    string? SuggestedAction,
    DateTime CreatedAt
);

public class GetTaskMessagesHandler : IRequestHandler<GetTaskMessagesQuery, ServiceResult<List<TaskMessageDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetTaskMessagesHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResult<List<TaskMessageDto>>> Handle(GetTaskMessagesQuery request, CancellationToken cancellationToken)
    {
        var messages = await _context.TaskMessages
            .Where(m => m.TaskId == request.TaskId)
            .OrderBy(m => m.CreatedAt)
            .Select(m => new TaskMessageDto(
                m.Id,
                m.Content,
                m.Sender.ToString(),
                m.SuggestedAction,
                m.CreatedAt
            ))
            .ToListAsync(cancellationToken);

        return ServiceResult<List<TaskMessageDto>>.Success(messages);
    }
}
