using Microsoft.AspNetCore.SignalR;

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

/// <summary>
/// Interface for sending task execution notifications.
/// </summary>
public interface ITaskExecutionNotifier
{
    Task NotifyAttemptStarted(Guid taskId, Guid attemptId);
    Task NotifyAttemptCompleted(Guid taskId, Guid attemptId, bool success);
    Task NotifyStepStarted(Guid attemptId, Guid stepId, string stepName);
    Task NotifyStepCompleted(Guid attemptId, Guid stepId, bool success, long durationMs);
    Task NotifyStepOutput(Guid attemptId, Guid stepId, string output);
}

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
}
