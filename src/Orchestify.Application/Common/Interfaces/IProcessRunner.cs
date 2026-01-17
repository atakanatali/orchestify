using System.Diagnostics;

namespace Orchestify.Application.Common.Interfaces;

/// <summary>
/// Service for running external processes (dotnet, npm, etc).
/// </summary>
public interface IProcessRunner
{
    Task<ProcessResult> RunAsync(string fileName, string arguments, string workingDirectory, CancellationToken cancellationToken);
    Task<ProcessResult> RunDotnetAsync(string arguments, string workingDirectory, CancellationToken cancellationToken);
}

/// <summary>
/// Result of a process execution.
/// </summary>
public class ProcessResult
{
    public bool Success { get; set; }
    public string Output { get; set; } = string.Empty;
    public string Error { get; set; } = string.Empty;
    public int ExitCode { get; set; }
    public TimeSpan Duration { get; set; }
}
