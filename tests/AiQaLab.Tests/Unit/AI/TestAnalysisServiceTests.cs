using AiQaLab.AI.Models;
using AiQaLab.AI.Services;
using AiQaLab.AI.Services.Interfaces;
using FluentAssertions;
using Google.GenAI.Types;
using Moq;
using System.Text.Json;

namespace AiQaLab.Tests.Unit.AI
{
    public class TestAnalysisServiceTests
    {
        private readonly Mock<IAIClient> _mockAIClient = new();
        private readonly Mock<ITestAnalysisPromptBuilder> _mockPromptBuilder = new();

        [Fact]
        public async Task AnalyzeAsync_ReturnsTestSuggestions()
        {
            // Arrange
            var requirement = "Users can log in with valid credentials.";

            var jsonResponse = JsonSerializer.Serialize(new TestAnalysisResult
            {
                Suggestions = [
                    new TestCaseSuggestion
                    { Description = "Description", TestLevel = "TestLevel", Reason = "Reason"
                    }
                ]
            });

            _mockPromptBuilder
            .Setup(x => x.Build(requirement))
            .Returns("Prompt");

            _mockAIClient
            .Setup(x => x.SendStructuredAsync(
                It.IsAny<string>(),
                It.IsAny<Schema>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(jsonResponse);

            var testAnalysisService = new TestAnalysisService(_mockAIClient.Object, _mockPromptBuilder.Object);


            // Act
            var result = await testAnalysisService.AnalyzeAsync(requirement);

            // Assert
            result.Should().NotBeNull();
            result.Suggestions.Should().NotBeEmpty();
        }

        [Fact]
        public async Task AnalyzeAsync_ReturnsExpectedTestSuggestion()
        {
            // Arrange
            var requirement = "Users can log in with valid credentials.";

            var expectedResult = new TestAnalysisResult
            {
                Suggestions =
                [
                    new TestCaseSuggestion
                    {
                        Description = "User can log in with valid credentials.",
                        TestLevel = "EndToEnd",
                        Reason = "Verifies the complete login flow."
                    }
                ]
            };

            var jsonResponse = JsonSerializer.Serialize(expectedResult);
            var expectedPrompt = "Generated test analysis prompt";

            _mockPromptBuilder
            .Setup(x => x.Build(requirement))
            .Returns(expectedPrompt);

            _mockAIClient
            .Setup(x => x.SendStructuredAsync(
                It.IsAny<string>(),
                It.IsAny<Schema>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(jsonResponse);

            var testAnalysisService = new TestAnalysisService(_mockAIClient.Object, _mockPromptBuilder.Object);

            // Act
            var result = await testAnalysisService.AnalyzeAsync(requirement);

            // Assert
            result.Should().NotBeNull();
            result.Suggestions.Should().NotBeEmpty();

            var suggestion = result.Suggestions[0];

            suggestion.Description.Should().Be("User can log in with valid credentials.");

            suggestion.TestLevel.Should().Be("EndToEnd");

            suggestion.Reason.Should().Be("Verifies the complete login flow.");

            _mockPromptBuilder.Verify(x => x.Build(requirement), Times.Once);

            _mockAIClient.Verify(x => x.SendStructuredAsync(
                expectedPrompt,
                It.IsAny<Schema>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
        }

        [Fact]
        public async Task AnalyzeAsync_WithInvalidAIResponse_ThrowsTestAnalysisException()
        {
            // Arrange
            var requirement = "Users can log in with valid credentials.";

            _mockPromptBuilder
            .Setup(x => x.Build(requirement))

            .Returns("Prompt");
            _mockAIClient
                .Setup(x => x.SendStructuredAsync(
                    It.IsAny<string>(),
                    It.IsAny<Schema>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync("This is not valid JSON");

            var service = new TestAnalysisService(_mockAIClient.Object, _mockPromptBuilder.Object);

            // Act
            var act = () => service.AnalyzeAsync(requirement);

            // Assert
            await act.Should()
                .ThrowAsync<TestAnalysisException>()
                .WithMessage("The AI returned an invalid test analysis result.");
        }

        [Fact]
        public async Task AnalyzeAsync_WithNoSuggestions_ThrowsTestAnalysisException()
        {
            // Arrange
            var requirement = "Users can log in with valid credentials.";

            var jsonResponse = JsonSerializer.Serialize(new TestAnalysisResult { Suggestions = [] });

            _mockPromptBuilder
            .Setup(x => x.Build(requirement))
            .Returns("Prompt");
            _mockAIClient
                .Setup(x => x.SendStructuredAsync(
                    It.IsAny<string>(),
                    It.IsAny<Schema>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(jsonResponse);

            var service = new TestAnalysisService(_mockAIClient.Object, _mockPromptBuilder.Object);

            // Act
            var act = () => service.AnalyzeAsync(requirement);

            // Assert
            await act.Should()
                .ThrowAsync<TestAnalysisException>()
                .WithMessage("The AI returned no test case suggestions.");
        }

        [Fact]
        public async Task AnalyzeAsync_WithEmptyAIResponse_ThrowsTestAnalysisException()
        {
            // Arrange
            var requirement = "Users can log in with valid credentials.";

            _mockPromptBuilder
            .Setup(x => x.Build(requirement))
            .Returns("Prompt");
            _mockAIClient
                .Setup(x => x.SendStructuredAsync(
                    It.IsAny<string>(),
                    It.IsAny<Schema>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync("null");

            var service = new TestAnalysisService(_mockAIClient.Object, _mockPromptBuilder.Object);

            // Act
            var act = () => service.AnalyzeAsync(requirement);

            // Assert
            await act.Should()
                .ThrowAsync<TestAnalysisException>()
                .WithMessage("The AI returned an empty test analysis result.");
        }
    }
}
