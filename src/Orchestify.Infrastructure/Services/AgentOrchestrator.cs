using Microsoft.Extensions.Logging;
using Orchestify.Application.Common.Interfaces;
using Orchestify.Contracts.Protos;
using Orchestify.Infrastructure.Communication;
using Grpc.Core;

namespace Orchestify.Infrastructure.Services;

public class AgentOrchestrator : IAgentOrchestrator
{
    private readonly ILogger<AgentOrchestrator> _logger;
    private readonly MetricsService _metricsService;
    private readonly ITaskExecutionNotifier _notifier;
    private readonly string _socketPath = "/tmp/orchestify-agent.sock"; // TODO: Load from config
    private const int ContextLimit = 32768;

    public AgentOrchestrator(
        ILogger<AgentOrchestrator> logger, 
        MetricsService metricsService,
        ITaskExecutionNotifier notifier)
    {
        _logger = logger;
        _metricsService = metricsService;
        _notifier = notifier;
    }

    public async IAsyncEnumerable<AgentResponse> ExecuteTaskStreamAsync(string taskId, string prompt, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Guid taskGuid = Guid.Parse(taskId);
        using var channel = UdsGrpcChannel.Create(_socketPath);
        var client = new AgentService.AgentServiceClient(channel);

        using var call = client.ExecuteTask(cancellationToken: cancellationToken);
        
        _metricsService.StartRequest();

        // Send initial request
        await call.RequestStream.WriteAsync(new AgentRequest
        {
            TaskId = taskId,
            Prompt = prompt,
            Context = { ["ContextLimit"] = ContextLimit.ToString() }
        });

        // Loop through responses from the LLM engine
        while (await call.ResponseStream.MoveNext(cancellationToken))
        {
            var response = call.ResponseStream.Current;
            
            // Track and notify metrics
            var updatedMetrics = _metricsService.ProcessResponse(response);
            response.Metrics = updatedMetrics;
            
            await _notifier.NotifyAgentMetrics(taskGuid, 
                updatedMetrics.TtftMs, 
                updatedMetrics.TokensPerSecond, 
                updatedMetrics.RamUsageGb, 
                updatedMetrics.VramUsageGb);

            // Notify UI based on payload type
            switch (response.PayloadCase)
            {
                case AgentResponse.PayloadOneofCase.Thought:
                    await _notifier.NotifyAgentThought(taskGuid, response.Thought.Content);
                    break;
                case AgentResponse.PayloadOneofCase.Terminal:
                    await _notifier.NotifyAgentTerminalAction(taskGuid, response.Terminal.Command, response.Terminal.Output, response.Terminal.ExitCode);
                    break;
                case AgentResponse.PayloadOneofCase.Signal:
                    await _notifier.NotifyAgentSignal(taskGuid, response.Signal.StepName, response.Signal.Type.ToString(), response.Signal.Metadata);
                    
                    // Handle Self-Healing
                    if (response.Signal.Type == StepSignal.Types.Type.Failed)
                    {
                        _logger.LogWarning("Step {StepName} failed. Initiating self-healing...", response.Signal.StepName);
                        var selfHealPrompt = $"The previous step '{response.Signal.StepName}' failed with error: {response.Signal.Metadata}. Please attempt to resolve this issue.";
                        await call.RequestStream.WriteAsync(new AgentRequest { TaskId = taskId, Prompt = selfHealPrompt });
                        continue;
                    }
                    break;
            }

            yield return response;
        }

        await call.RequestStream.CompleteAsync();
    }
}
