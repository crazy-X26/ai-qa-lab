using Google.GenAI;
using Google.GenAI.Types;
using System.Text.Json.Nodes;

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

        public async Task<string> SendStructuredAsync(string prompt, JsonNode responseSchema, CancellationToken cancellationToken = default)
        {
            var config = new GenerateContentConfig
            {
                ResponseMimeType = "application/json",
                ResponseJsonSchema = responseSchema
            };

            var response = await _client.Models.GenerateContentAsync(
                model: "gemini-3.1-flash-lite",
                contents: prompt,
                config: config,
                cancellationToken: cancellationToken
            );

            return response.Text ?? string.Empty;
        }
    }
}
