using System.Diagnostics;
using Orchestify.Contracts.Protos;

namespace Orchestify.Infrastructure.Services;

public class MetricsService
{
    private Stopwatch? _ttftStopwatch;
    private bool _hasReceivedFirstToken;

    public void StartRequest()
    {
        _ttftStopwatch = Stopwatch.StartNew();
        _hasReceivedFirstToken = false;
    }

    public Metrics ProcessResponse(AgentResponse response)
    {
        if (!_hasReceivedFirstToken && response.PayloadCase == AgentResponse.PayloadOneofCase.Delta)
        {
            _hasReceivedFirstToken = true;
            _ttftStopwatch?.Stop();
        }

        // Return current metrics based on the response or backend measurement
        return new Metrics
        {
            TtftMs = _ttftStopwatch?.Elapsed.TotalMilliseconds ?? 0,
            // These would typically come from the LLM engine response
            TokensPerSecond = response.Metrics?.TokensPerSecond ?? 0,
            RamUsageGb = response.Metrics?.RamUsageGb ?? 0,
            VramUsageGb = response.Metrics?.VramUsageGb ?? 0
        };
    }
}
