using AiQaLab.AI.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace AiQaLab.Tests.Integration.AI
{
    public class GeminiAIClientTests
    {
        [Trait("Category", "AI")]
        [Fact]
        public async Task SendAsync_WithValidPrompt_ReturnsResponse()
        {
            // Arrange
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<GeminiAIClientTests>()
                .Build();

            var apiKey = configuration["AI:Gemini:ApiKey"];

            apiKey.Should().NotBeNullOrWhiteSpace(
                "the Gemini API key must be configured for this integration test");

            var client = new GeminiAIClient(apiKey!);

            // Act
            var response = await client.SendAsync(
                "Answer with exactly one short sentence: What is a unit test?");

            // Assert
            response.Should().NotBeNullOrWhiteSpace();
        }
    }
}
