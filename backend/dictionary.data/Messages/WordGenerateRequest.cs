namespace Dictionary.Data.Messages;

/// <summary>
/// Message contract for generating a dictionary word.
/// This is published when a new word should be processed and definitions generated.
/// </summary>
public record WordGenerateRequest
{
    public string Word { get; init; } = string.Empty;
    public DateTime RequestedAt { get; init; } = DateTime.UtcNow;
}