namespace Orchestify.Contracts.Attempts;

/// <summary>
/// Response DTO for list of attempts.
/// </summary>
public class AttemptsListResponseDto
{
    /// <summary>
    /// List of attempts.
    /// </summary>
    public List<AttemptDto> Items { get; set; } = new();
}
