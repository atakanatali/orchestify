using Orchestify.Contracts.Protos;

namespace Orchestify.Application.Common.Interfaces;

public interface IAgentOrchestrator
{
    IAsyncEnumerable<AgentResponse> ExecuteTaskStreamAsync(string taskId, string prompt, CancellationToken cancellationToken);
}
