namespace AiQaLab.AI.Services.Interfaces
{
    public interface IAIClient
    {
        Task<string> SendAsync(string prompt, CancellationToken cancellationToken = default);
    }
}
