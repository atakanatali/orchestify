using System.Threading.Tasks;
using Orchestify.Domain.Entities;

namespace Orchestify.Application.Common.Interfaces;

public interface IAgentService
{
    /// <summary>
    /// Processes a user message in the context of a task and workspace.
    /// Returns the agent's response and any suggested tool calls.
    /// </summary>
    Task<AgentResponse> ProcessMessageAsync(TaskEntity task, string userMessage, CancellationToken cancellationToken);
}

public class AgentResponse
{
    public string Content { get; set; } = string.Empty;
    public AgentToolCall? SuggestedAction { get; set; }
}

public class AgentToolCall
{
    public string Type { get; set; } = "terminal"; // terminal, file_read, file_write
    public string Command { get; set; } = string.Empty;
    public string? Description { get; set; }
}
