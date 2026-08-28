using System.Text.Json.Nodes;

public interface IAIClient
{
    Task<string> SendAsync(
        string prompt,
        CancellationToken cancellationToken = default);

    Task<string> SendStructuredAsync(
        string prompt,
        JsonNode responseSchema,
        CancellationToken cancellationToken = default);
}