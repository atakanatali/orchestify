using MassTransit;
using Microsoft.Extensions.Logging;
using Orchestify.Infrastructure.Messaging;
using StackExchange.Redis;
using System.Text.Json;

namespace Orchestify.Worker.Consumers;

/// <summary>
/// Consumer for RunTaskMessage. Executes tasks from the queue.
/// </summary>
public class RunTaskConsumer : IConsumer<RunTaskMessage>
{
    private readonly ILogger<RunTaskConsumer> _logger;
    private readonly IConnectionMultiplexer? _redis;

    public RunTaskConsumer(ILogger<RunTaskConsumer> logger, IConnectionMultiplexer? redis = null)
    {
        _logger = logger;
        _redis = redis;
    }

    public async Task Consume(ConsumeContext<RunTaskMessage> context)
    {
        var message = context.Message;
        _logger.LogInformation("Received task execution request: TaskId={TaskId}, CorrelationId={CorrelationId}",
            message.TaskId, message.CorrelationId);

        try
        {
            // Notify via Redis Pub/Sub that we're starting
            await PublishEventAsync(message.TaskId, new TaskEventMessage
            {
                TaskId = message.TaskId,
                EventType = "signal",
                Content = "Task execution started"
            });

            // TODO: Call actual task execution logic (AgentOrchestrator, etc.)
            // For now, simulate some work
            _logger.LogInformation("Processing task: {TaskId} with prompt: {Prompt}", 
                message.TaskId, message.Prompt?.Substring(0, Math.Min(100, message.Prompt?.Length ?? 0)));

            // Simulate a thought event
            await PublishEventAsync(message.TaskId, new TaskEventMessage
            {
                TaskId = message.TaskId,
                EventType = "thought",
                Content = $"Analyzing task: {message.Prompt}"
            });

            await Task.Delay(1000); // Simulate work

            // Notify completion
            await PublishEventAsync(message.TaskId, new TaskEventMessage
            {
                TaskId = message.TaskId,
                EventType = "signal",
                Content = "Task execution completed"
            });

            _logger.LogInformation("Task execution completed: TaskId={TaskId}", message.TaskId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Task execution failed: TaskId={TaskId}", message.TaskId);
            throw;
        }
    }

    private async Task PublishEventAsync(Guid taskId, TaskEventMessage eventMessage)
    {
        if (_redis == null) return;

        try
        {
            var subscriber = _redis.GetSubscriber();
            var payload = JsonSerializer.Serialize(eventMessage);
            await subscriber.PublishAsync(RedisChannel.Literal($"task:{taskId}:events"), payload);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to publish event to Redis for task {TaskId}", taskId);
        }
    }
}
