using AiQaLab.AI.Services.Interfaces;
using Anthropic;

namespace AiQaLab.AI.Services
{
    public class AnthropicAIClient : IAIClient
    {
        private readonly AnthropicClient _client;

        public AnthropicAIClient()
        {
            _client = new AnthropicClient();
        }

        public Task<string> SendAsync(string prompt, CancellationToken cancellationToken = default)
        {
            // Implement the logic to send the prompt to the Anthropic AI API and return the response
            throw new NotImplementedException();
        }
    }
}
