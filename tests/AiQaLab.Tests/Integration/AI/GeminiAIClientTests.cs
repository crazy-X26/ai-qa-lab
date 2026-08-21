using AiQaLab.AI.Services;
using AiQaLab.AI.Schemas;
using AiQaLab.AI.Models;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

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

        [Trait("Category", "AI")]
        [Fact]
        public async Task SendStructuredAsync_WithTestRequirement_ReturnsValidTestAnalysisResult()
        {
            // Arrange
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<GeminiAIClientTests>()
                .Build();

            var apiKey = configuration["AI:Gemini:ApiKey"];

            apiKey.Should().NotBeNullOrWhiteSpace(
                "the Gemini API key must be configured for this integration test");

            var client = new GeminiAIClient(apiKey!);
            var schema = TestAnalysisSchema.Create();

            // Act
            var response = await client.SendStructuredAsync(
                """
                Analyze this requirement:

                A user must be able to log in with valid credentials.
                When the credentials are valid, the user should be redirected to the home page.

                Suggest appropriate tests.
                """,
                schema);
            
            // Assert
            response.Should().NotBeNullOrWhiteSpace();

            var result = JsonSerializer.Deserialize<TestAnalysisResult>(
                response,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            result.Should().NotBeNull("the response should be a valid JSON object");
            result!.Suggestions.Should().NotBeNullOrEmpty("the response should contain test case suggestions");

            foreach (var suggestion in result.Suggestions)
            {
                suggestion.Description.Should().NotBeNullOrWhiteSpace();
                suggestion.TestLevel.Should().NotBeNullOrWhiteSpace();
                suggestion.Reason.Should().NotBeNullOrWhiteSpace();

                suggestion.TestLevel.Should().BeOneOf(
                    "Unit",
                    "Integration",
                    "EndToEnd");
            }
        }
    }
}
