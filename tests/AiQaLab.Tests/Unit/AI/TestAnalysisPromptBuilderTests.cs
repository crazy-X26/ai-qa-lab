using AiQaLab.AI.Services;
using FluentAssertions;

namespace AiQaLab.Tests.Unit.AI
{
    public class TestAnalysisPromptBuilderTests
    {
        [Fact]
        public void Build_ShouldIncludeRequirementInPrompt()
        {
            // Arrange
            var requirement = "The system shall allow users to reset their password.";
            var promptBuilder = new TestAnalysisPromptBuilder();
            // Act
            var prompt = promptBuilder.Build(requirement);
            // Assert
            prompt.Should().Contain(requirement);
        }

        [Fact]
        public void Build_ShouldContainTestAnalysisInstructions()
        {
            // Arrange
            var requirement = "The system shall allow users to reset their password.";
            var promptBuilder = new TestAnalysisPromptBuilder();
            // Act
            var prompt = promptBuilder.Build(requirement);
            // Assert
            prompt.Should().Contain("Analyze the following software requirement and suggest appropriate test cases.");
        }
    }
}
