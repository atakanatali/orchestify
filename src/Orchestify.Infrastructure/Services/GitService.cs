using System.Diagnostics;
using Orchestify.Application.Common.Interfaces;

namespace Orchestify.Infrastructure.Services;

/// <summary>
/// Git service using CLI commands.
/// </summary>
public class GitService : IGitService
{
    public async Task<GitCommandResult> CloneAsync(string repoUrl, string targetPath, CancellationToken cancellationToken)
    {
        return await RunGitAsync($"clone {repoUrl} {targetPath}", null, cancellationToken);
    }

    public async Task<GitCommandResult> PullAsync(string repoPath, CancellationToken cancellationToken)
    {
        return await RunGitAsync("pull", repoPath, cancellationToken);
    }

    public async Task<GitCommandResult> CheckoutAsync(string repoPath, string branch, CancellationToken cancellationToken)
    {
        return await RunGitAsync($"checkout {branch}", repoPath, cancellationToken);
    }

    public async Task<GitCommandResult> GetCurrentBranchAsync(string repoPath, CancellationToken cancellationToken)
    {
        return await RunGitAsync("branch --show-current", repoPath, cancellationToken);
    }

    public async Task<GitCommandResult> GetLastCommitAsync(string repoPath, CancellationToken cancellationToken)
    {
        return await RunGitAsync("log -1 --format=%H", repoPath, cancellationToken);
    }

    public async Task<GitCommandResult> ListBranchesAsync(string repoPath, CancellationToken cancellationToken)
    {
        // List local and remote branches, clean output
        return await RunGitAsync("branch -a --format=%(refname:short)", repoPath, cancellationToken);
    }

    public async Task<GitCommandResult> InitAsync(string repoPath, CancellationToken cancellationToken)
    {
        if (!Directory.Exists(repoPath))
        {
            Directory.CreateDirectory(repoPath);
        }
        return await RunGitAsync("init", repoPath, cancellationToken);
    }

    private static async Task<GitCommandResult> RunGitAsync(string arguments, string? workingDirectory, CancellationToken cancellationToken)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "git",
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        if (!string.IsNullOrEmpty(workingDirectory))
            psi.WorkingDirectory = workingDirectory;

        try
        {
            using var process = Process.Start(psi) ?? throw new InvalidOperationException("Failed to start git process");
            var output = await process.StandardOutput.ReadToEndAsync(cancellationToken);
            var error = await process.StandardError.ReadToEndAsync(cancellationToken);
            await process.WaitForExitAsync(cancellationToken);

            return process.ExitCode == 0
                ? GitCommandResult.Ok(output.Trim())
                : GitCommandResult.Fail(error.Trim(), process.ExitCode);
        }
        catch (Exception ex)
        {
            return GitCommandResult.Fail(ex.Message, -1);
        }
    }
}
