using AiQaLab.AI.Services.Interfaces;
using Google.GenAI;
using Google.GenAI.Types;

namespace AiQaLab.AI.Services
{
    public class GeminiAIClient : IAIClient
    {
        private readonly Client _client;

        public GeminiAIClient(string apiKey)
        {
            _client = new Client(apiKey: apiKey);
        }

        public async Task<string> SendAsync(string prompt, CancellationToken cancellationToken = default)
        {
            var response = await _client.Models.GenerateContentAsync(
                model: "gemini-3.1-flash-lite",
                contents: prompt,
                cancellationToken: cancellationToken
            );

            return response.Text ?? string.Empty;
        }
    }
}
