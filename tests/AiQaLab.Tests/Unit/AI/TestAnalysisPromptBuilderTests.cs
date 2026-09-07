using AiQaLab.AI.Models;
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
            var request = new TestAnalysisRequest { Requirement = requirement };
            // Act
            var prompt = promptBuilder.Build(request);
            // Assert
            prompt.Should().Contain(requirement);
        }

        [Fact]
        public void Build_ShouldContainTestAnalysisInstructions()
        {
            // Arrange
            var requirement = "The system shall allow users to reset their password.";
            var promptBuilder = new TestAnalysisPromptBuilder();
            var request = new TestAnalysisRequest { Requirement = requirement };
            // Act
            var prompt = promptBuilder.Build(request);
            // Assert
            prompt.Should().Contain("Analyze the following software requirement and suggest appropriate test cases.");
        }

        [Fact]
        public void Build_WithSourceContext_ShouldIncludeSourceCodeInPrompt()
        {
            // Arrange
            var requirement = "The system shall allow users to reset their password.";
            var sourceCode = new CodeContextItem
            {
                Kind = CodeContextKind.SourceCode,
                Path = "src/PasswordResetService.cs",
                Content = "public class PasswordResetService { /* implementation */ }"
            };
            var promptBuilder = new TestAnalysisPromptBuilder();
            var request = new TestAnalysisRequest
            {
                Requirement = requirement,
                CodeContext = new List<CodeContextItem> { sourceCode }
            };
            // Act
            var prompt = promptBuilder.Build(request);
            // Assert
            prompt.Should().Contain("Relevant source code.");
            prompt.Should().Contain("src/PasswordResetService.cs");
            prompt.Should().Contain(sourceCode.Content);
        }

        [Fact]
        public void Build_WithTestContext_ShouldIncludeExistingTestsInPrompt()
        {
            // Arrange
            var requirement = "The system shall allow users to reset their password.";
            var existingTest = new CodeContextItem
            {
                Kind = CodeContextKind.Test,
                Path = "tests/PasswordResetTests.cs",
                Content = "public void TestPasswordReset() { /* test implementation */ }"
            };
            var promptBuilder = new TestAnalysisPromptBuilder();
            var request = new TestAnalysisRequest
            {
                Requirement = requirement,
                CodeContext = new List<CodeContextItem> { existingTest }
            };
            // Act
            var prompt = promptBuilder.Build(request);
            // Assert
            prompt.Should().Contain("Existing tests.");
            prompt.Should().Contain("tests/PasswordResetTests.cs");
            prompt.Should().Contain(existingTest.Content);
        }

        [Fact]
        public void Build_WithoutCodeContext_ShouldMatchPlainPrompt()
        {
            // Arrange
            var requirement = "The system shall allow users to reset their password.";
            var promptBuilder = new TestAnalysisPromptBuilder();
            var request = new TestAnalysisRequest { Requirement = requirement };
            // Act
            var prompt = promptBuilder.Build(request);
            // Assert
            prompt.Should().NotContain("Relevant source code.");
            prompt.Should().NotContain("Existing tests.");
        }
    }
}
