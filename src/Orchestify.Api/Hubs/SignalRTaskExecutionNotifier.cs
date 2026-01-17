using Microsoft.AspNetCore.SignalR;
using Orchestify.Application.Common.Interfaces;

namespace Orchestify.Api.Hubs;

/// <summary>
/// SignalR hub for real-time task execution updates.
/// </summary>
public class TaskExecutionHub : Hub
{
    /// <summary>
    /// Joins a task group for receiving updates.
    /// </summary>
    public async Task JoinTaskGroup(Guid taskId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"task-{taskId}");
    }

    /// <summary>
    /// Leaves a task group.
    /// </summary>
    public async Task LeaveTaskGroup(Guid taskId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"task-{taskId}");
    }

    /// <summary>
    /// Joins an attempt group for receiving step updates.
    /// </summary>
    public async Task JoinAttemptGroup(Guid attemptId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"attempt-{attemptId}");
    }

    /// <summary>
    /// Leaves an attempt group.
    /// </summary>
    public async Task LeaveAttemptGroup(Guid attemptId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"attempt-{attemptId}");
    }
}

// Interface moved to Application layer

/// <summary>
/// SignalR-based implementation of task execution notifier.
/// </summary>
public class SignalRTaskExecutionNotifier : ITaskExecutionNotifier
{
    private readonly IHubContext<TaskExecutionHub> _hubContext;

    public SignalRTaskExecutionNotifier(IHubContext<TaskExecutionHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyAttemptStarted(Guid taskId, Guid attemptId)
    {
        await _hubContext.Clients.Group($"task-{taskId}").SendAsync("AttemptStarted", new { taskId, attemptId });
    }

    public async Task NotifyAttemptCompleted(Guid taskId, Guid attemptId, bool success)
    {
        await _hubContext.Clients.Group($"task-{taskId}").SendAsync("AttemptCompleted", new { taskId, attemptId, success });
    }

    public async Task NotifyStepStarted(Guid attemptId, Guid stepId, string stepName)
    {
        await _hubContext.Clients.Group($"attempt-{attemptId}").SendAsync("StepStarted", new { attemptId, stepId, stepName });
    }

    public async Task NotifyStepCompleted(Guid attemptId, Guid stepId, bool success, long durationMs)
    {
        await _hubContext.Clients.Group($"attempt-{attemptId}").SendAsync("StepCompleted", new { attemptId, stepId, success, durationMs });
    }

    public async Task NotifyStepOutput(Guid attemptId, Guid stepId, string output)
    {
        await _hubContext.Clients.Group($"attempt-{attemptId}").SendAsync("StepOutput", new { attemptId, stepId, output });
    }

    public async Task NotifyAgentThought(Guid taskId, string content)
    {
        await _hubContext.Clients.Group($"task-{taskId}").SendAsync("AgentThought", new { taskId, content });
    }

    public async Task NotifyAgentTerminalAction(Guid taskId, string command, string output, int exitCode)
    {
        await _hubContext.Clients.Group($"task-{taskId}").SendAsync("AgentTerminalAction", new { taskId, command, output, exitCode });
    }

    public async Task NotifyAgentMetrics(Guid taskId, double ttft, double tps, double ram, double vram)
    {
        await _hubContext.Clients.Group($"task-{taskId}").SendAsync("AgentMetrics", new { taskId, ttft, tps, ram, vram });
    }

    public async Task NotifyAgentSignal(Guid taskId, string stepName, string status, string metadata)
    {
        await _hubContext.Clients.Group($"task-{taskId}").SendAsync("AgentSignal", new { taskId, stepName, status, metadata });
    }
}
