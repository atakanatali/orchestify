namespace Orchestify.Application.Common.Interfaces;

public interface ITaskExecutionNotifier
{
    Task NotifyAttemptStarted(Guid taskId, Guid attemptId);
    Task NotifyAttemptCompleted(Guid taskId, Guid attemptId, bool success);
    Task NotifyStepStarted(Guid attemptId, Guid stepId, string stepName);
    Task NotifyStepCompleted(Guid attemptId, Guid stepId, bool success, long durationMs);
    Task NotifyStepOutput(Guid attemptId, Guid stepId, string output);
    
    // AI Agent Specific Notifiers
    Task NotifyAgentThought(Guid taskId, string content);
    Task NotifyAgentTerminalAction(Guid taskId, string command, string output, int exitCode);
    Task NotifyAgentMetrics(Guid taskId, double ttft, double tps, double ram, double vram);
    Task NotifyAgentSignal(Guid taskId, string stepName, string status, string metadata);
}
