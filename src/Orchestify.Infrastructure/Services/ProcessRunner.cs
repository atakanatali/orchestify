using System.Diagnostics;
using Orchestify.Application.Common.Interfaces;

namespace Orchestify.Infrastructure.Services;

/// <summary>
/// Runs external processes for build/test operations.
/// </summary>
public class ProcessRunner : IProcessRunner
{
    public Task<ProcessResult> RunDotnetAsync(string arguments, string workingDirectory, CancellationToken cancellationToken)
    {
        return RunAsync("dotnet", arguments, workingDirectory, cancellationToken);
    }

    public async Task<ProcessResult> RunAsync(string fileName, string arguments, string workingDirectory, CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();
        var psi = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        try
        {
            using var process = Process.Start(psi) ?? throw new InvalidOperationException($"Failed to start {fileName}");
            var output = await process.StandardOutput.ReadToEndAsync(cancellationToken);
            var error = await process.StandardError.ReadToEndAsync(cancellationToken);
            await process.WaitForExitAsync(cancellationToken);
            sw.Stop();

            return new ProcessResult
            {
                Success = process.ExitCode == 0,
                Output = output,
                Error = error,
                ExitCode = process.ExitCode,
                Duration = sw.Elapsed
            };
        }
        catch (Exception ex)
        {
            sw.Stop();
            return new ProcessResult
            {
                Success = false,
                Error = ex.Message,
                ExitCode = -1,
                Duration = sw.Elapsed
            };
        }
    }
}
