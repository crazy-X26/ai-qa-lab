using AiQaLab.AI.Services;
using FluentAssertions;

namespace AiQaLab.Tests.Unit.AI
{
    public class TestAnalysisServiceTests
    {
        private readonly TestAnalysisService _testAnalysisService;
        public TestAnalysisServiceTests()
        {
            _testAnalysisService = new TestAnalysisService();
        }
        
        [Fact]
        public async Task AnalyzeAsync_ReturnsTestSuggestions()
        {
            // Arrange
            var requirement = "Users can log in with valid credentials.";

            // Act
            var result = await _testAnalysisService.AnalyzeAsync(requirement);

            // Assert
            result.Should().NotBeNull();
            result.Suggestions.Should().NotBeEmpty();
        }
    }
}
