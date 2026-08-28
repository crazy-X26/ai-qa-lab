using System.Net.Http.Json;
using System.Text.Json.Nodes;

namespace AiQaLab.AI.Services
{
    public class OllamaAIClient : IAIClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _model;

        public OllamaAIClient(HttpClient httpClient, string model)
        {
            _httpClient = httpClient;
            _model = model;
        }
        
        public async Task<string> SendAsync(string prompt, CancellationToken cancellationToken = default)
        {
            var request = new
            {
                model = _model,
                messages = new[]
                {
                    new {
                        role = "user",
                        content = prompt
                    }
                },
                stream = false
            };

            using var response = await _httpClient.PostAsJsonAsync("/api/chat", request, cancellationToken);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<JsonNode>(cancellationToken);

            return result?["message"]?["content"]?.GetValue<string>() ?? string.Empty;
        }

        public async Task<string> SendStructuredAsync(string prompt, JsonNode responseSchema, CancellationToken cancellationToken = default)
        {
            var request = new
            {
                model = _model,
                messages = new[]
                {
                    new
                    {
                        role = "user",
                        content = prompt
                    }
                },
                stream = false,
                format = responseSchema
            };

            using var response = await _httpClient.PostAsJsonAsync("/api/chat", request, cancellationToken);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<JsonNode>(cancellationToken);

            return result?["message"]?["content"]?.GetValue<string>()?? string.Empty;
        }
    }
}
