using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Orchestify.Api.Services;

/// <summary>
/// Background service that bridges Redis Pub/Sub events to SignalR clients.
/// Workers publish events to Redis, and this service forwards them to connected clients.
/// </summary>
public class RedisToSignalRBridge : BackgroundService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IHubContext<Hubs.TaskExecutionHub> _hubContext;
    private readonly ILogger<RedisToSignalRBridge> _logger;

    public RedisToSignalRBridge(
        IConnectionMultiplexer redis,
        IHubContext<Hubs.TaskExecutionHub> hubContext,
        ILogger<RedisToSignalRBridge> logger)
    {
        _redis = redis;
        _hubContext = hubContext;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var subscriber = _redis.GetSubscriber();

        _logger.LogInformation("RedisToSignalRBridge started. Subscribing to task:*:events...");

        // Subscribe to all task events using pattern matching
        await subscriber.SubscribeAsync(
            RedisChannel.Pattern("task:*:events"),
            async (channel, message) =>
            {
                try
                {
                    var taskId = ExtractTaskId(channel.ToString());
                    if (taskId == null)
                    {
                        _logger.LogWarning("Could not extract task ID from channel: {Channel}", channel);
                        return;
                    }

                    var eventData = JsonSerializer.Deserialize<TaskEventPayload>(message.ToString());
                    if (eventData == null) return;

                    // Forward to SignalR clients in the task group
                    await ForwardToSignalR(taskId.Value, eventData);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing Redis message from channel: {Channel}", channel);
                }
            });

        // Keep the service running
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private Guid? ExtractTaskId(string channel)
    {
        // Channel format: task:{taskId}:events
        var parts = channel.Split(':');
        if (parts.Length >= 2 && Guid.TryParse(parts[1], out var taskId))
        {
            return taskId;
        }
        return null;
    }

    private async Task ForwardToSignalR(Guid taskId, TaskEventPayload payload)
    {
        var group = $"task-{taskId}";

        switch (payload.EventType)
        {
            case "thought":
                await _hubContext.Clients.Group(group).SendAsync("AgentThought", new { taskId, content = payload.Content });
                break;
            case "terminal":
                await _hubContext.Clients.Group(group).SendAsync("AgentTerminalAction", new { taskId, command = payload.Command, output = payload.Content, exitCode = payload.ExitCode });
                break;
            case "signal":
                await _hubContext.Clients.Group(group).SendAsync("AgentSignal", new { taskId, stepName = payload.StepName, status = payload.Status, metadata = payload.Content });
                break;
            case "metrics":
                await _hubContext.Clients.Group(group).SendAsync("AgentMetrics", new { taskId, ttft = payload.Ttft, tps = payload.Tps, ram = payload.Ram, vram = payload.Vram });
                break;
            default:
                _logger.LogWarning("Unknown event type: {EventType}", payload.EventType);
                break;
        }
    }

    private class TaskEventPayload
    {
        public string EventType { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? Command { get; set; }
        public int? ExitCode { get; set; }
        public string? StepName { get; set; }
        public string? Status { get; set; }
        public double? Ttft { get; set; }
        public double? Tps { get; set; }
        public double? Ram { get; set; }
        public double? Vram { get; set; }
    }
}
