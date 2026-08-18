using AiQaLab.AI.Services.Interfaces;
using Anthropic;
using Google.GenAI.Types;

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

        public Task<string> SendStructuredAsync(string prompt, Schema responseSchema, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
