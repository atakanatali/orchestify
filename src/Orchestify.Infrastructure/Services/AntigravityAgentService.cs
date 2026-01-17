using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Orchestify.Application.Common.Interfaces;
using Orchestify.Domain.Entities;

namespace Orchestify.Infrastructure.Services;

public class AntigravityAgentService : IAgentService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AntigravityAgentService> _logger;
    private readonly string? _apiKey;
    private readonly string _model = "gemini-2.0-flash";

    public AntigravityAgentService(HttpClient httpClient, IConfiguration configuration, ILogger<AntigravityAgentService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiKey = configuration["Gemini:ApiKey"] ?? Environment.GetEnvironmentVariable("GEMINI_API_KEY");
    }

    public async Task<AgentResponse> ProcessMessageAsync(TaskEntity task, string userMessage, CancellationToken cancellationToken)
    {
        // If no API key, fall back to mock responses
        if (string.IsNullOrEmpty(_apiKey))
        {
            _logger.LogWarning("Gemini API key not configured. Using mock responses.");
            return GetMockResponse(task, userMessage);
        }

        try
        {
            var systemPrompt = BuildSystemPrompt(task);
            var response = await CallGeminiApiAsync(systemPrompt, userMessage, cancellationToken);
            return ParseGeminiResponse(response, task);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Gemini API");
            return new AgentResponse
            {
                Content = $"I encountered an error processing your request. Let me try a simpler approach. Error: {ex.Message}"
            };
        }
    }

    private string BuildSystemPrompt(TaskEntity task)
    {
        var workspacePath = task.Board?.Workspace?.RepositoryPath ?? "unknown";
        var taskTitle = task.Title;
        var taskDescription = task.Description ?? "No description provided";

        return $@"You are Antigravity, an AI coding assistant integrated into Orchestify - a task orchestration platform.

## Context
- **Task**: {taskTitle}
- **Description**: {taskDescription}
- **Workspace Path**: {workspacePath}

## Your Capabilities
1. Suggest terminal commands to execute in the workspace
2. Help with code analysis and debugging
3. Provide step-by-step guidance for completing the task
4. Generate code snippets when requested

## Response Format
When suggesting a command, include it in a JSON block like this:
```json
{{""command"": ""git status"", ""description"": ""Check repository status""}}
```

Be concise, helpful, and action-oriented. Always relate your responses to the current task context.";
    }

    private async Task<string> CallGeminiApiAsync(string systemPrompt, string userMessage, CancellationToken cancellationToken)
    {
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKey}";

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = $"{systemPrompt}\n\nUser: {userMessage}" }
                    }
                }
            },
            generationConfig = new
            {
                temperature = 0.7,
                topK = 40,
                topP = 0.95,
                maxOutputTokens = 1024
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content, cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Gemini API error: {StatusCode} - {Body}", response.StatusCode, responseBody);
            throw new Exception($"Gemini API returned {response.StatusCode}");
        }

        return responseBody;
    }

    private AgentResponse ParseGeminiResponse(string jsonResponse, TaskEntity task)
    {
        try
        {
            using var doc = JsonDocument.Parse(jsonResponse);
            var textContent = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString() ?? "I couldn't generate a response.";

            var response = new AgentResponse { Content = textContent };

            // Try to extract command suggestion from markdown code block
            var jsonMatch = System.Text.RegularExpressions.Regex.Match(
                textContent,
                @"```json\s*\{[^}]*""command""\s*:\s*""([^""]+)""[^}]*""description""\s*:\s*""([^""]+)""[^}]*\}\s*```",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase
            );

            if (jsonMatch.Success)
            {
                response.SuggestedAction = new AgentToolCall
                {
                    Type = "terminal",
                    Command = jsonMatch.Groups[1].Value,
                    Description = jsonMatch.Groups[2].Value
                };
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing Gemini response");
            return new AgentResponse { Content = "I received a response but couldn't parse it properly." };
        }
    }

    private AgentResponse GetMockResponse(TaskEntity task, string userMessage)
    {
        var content = userMessage.ToLower();
        var response = new AgentResponse();

        if (content.Contains("git status") || content.Contains("check repo"))
        {
            response.Content = "I'll check the current status of your repository in the workspace.";
            response.SuggestedAction = new AgentToolCall
            {
                Type = "terminal",
                Command = "git status",
                Description = "Check git repository status"
            };
        }
        else if (content.Contains("list") || content.Contains("files"))
        {
            response.Content = "Let me list the files in the workspace for you.";
            response.SuggestedAction = new AgentToolCall
            {
                Type = "terminal",
                Command = "ls -la",
                Description = "List all files in workspace"
            };
        }
        else if (content.Contains("build") || content.Contains("run"))
        {
            response.Content = $"I can trigger a build for the project associated with '{task.Title}'.";
            response.SuggestedAction = new AgentToolCall
            {
                Type = "terminal",
                Command = "npm run build",
                Description = "Execute project build"
            };
        }
        else
        {
            response.Content = $"I am Antigravity. I see you're working on '{task.Title}' at '{task.Board?.Workspace?.RepositoryPath}'. How can I assist with your orchestration today? 🚀\n\n**Note**: To enable real AI responses, configure GEMINI_API_KEY in your environment.";
        }

        return response;
    }
}
