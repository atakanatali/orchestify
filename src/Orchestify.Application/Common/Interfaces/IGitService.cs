using System.Diagnostics;

namespace Orchestify.Application.Common.Interfaces;

/// <summary>
/// Service for Git operations.
/// </summary>
public interface IGitService
{
    Task<GitCommandResult> CloneAsync(string repoUrl, string targetPath, CancellationToken cancellationToken);
    Task<GitCommandResult> PullAsync(string repoPath, CancellationToken cancellationToken);
    Task<GitCommandResult> CheckoutAsync(string repoPath, string branch, CancellationToken cancellationToken);
    Task<GitCommandResult> GetCurrentBranchAsync(string repoPath, CancellationToken cancellationToken);
    Task<GitCommandResult> GetLastCommitAsync(string repoPath, CancellationToken cancellationToken);
    Task<GitCommandResult> ListBranchesAsync(string repoPath, CancellationToken cancellationToken);
}

/// <summary>
/// Result of a Git command execution.
/// </summary>
public class GitCommandResult
{
    public bool Success { get; set; }
    public string Output { get; set; } = string.Empty;
    public string Error { get; set; } = string.Empty;
    public int ExitCode { get; set; }

    public static GitCommandResult Ok(string output) => new() { Success = true, Output = output, ExitCode = 0 };
    public static GitCommandResult Fail(string error, int exitCode) => new() { Success = false, Error = error, ExitCode = exitCode };
}
