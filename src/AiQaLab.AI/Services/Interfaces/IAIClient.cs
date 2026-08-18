using Google.GenAI.Types;

namespace AiQaLab.AI.Services.Interfaces
{
    public interface IAIClient
    {
        Task<string> SendAsync(
            string prompt,
            CancellationToken cancellationToken = default);

        Task<string> SendStructuredAsync(
            string prompt,
            Schema responseSchema,
            CancellationToken cancellationToken = default);
    }
}
