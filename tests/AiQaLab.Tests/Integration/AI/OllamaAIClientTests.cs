using AiQaLab.AI.Models;
using AiQaLab.AI.Schemas;
using AiQaLab.AI.Services;
using FluentAssertions;
using System.Text.Json;

namespace AiQaLab.Tests.Integration.AI
{
    public class OllamaAIClientTests
    {
        [Trait("Category", "AI")]
        [Fact]
        public async Task SendStructuredAsync_WithTestRequirement_ReturnsValidTestAnalysisResult()
        {
            // Arrange
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:11434"),
                Timeout = TimeSpan.FromMinutes(3)
            };

            var client = new OllamaAIClient(httpClient, "qwen3:4b");

            var schema = TestAnalysisSchema.CreateSchema();

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
