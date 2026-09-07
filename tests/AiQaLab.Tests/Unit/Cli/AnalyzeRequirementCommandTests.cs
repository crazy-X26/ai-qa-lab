using AiQaLab.AI.Models;
using AiQaLab.AI.Services.Interfaces;
using AiQaLab.Cli;
using AiQaLab.Cli.Commands;
using FluentAssertions;
using Moq;

namespace AiQaLab.Tests.Unit.Cli
{
    public class AnalyzeRequirementCommandTests : IDisposable
    {
        private readonly Mock<ITestAnalysisService> _mockAlalysisService = new();
        private readonly List<String> _tempFiles = new();

        [Fact]
        public async Task Build_WithCodeAndTestsOptions_ShouldResolvecodeContextItems()
        {
            // Arrange
            var requirementFile = CreateTempFile("The system shall allow users to reset their password.");
            var sourceCodeFile = CreateTempFile("public class PasswordResetService { /* implementation */ }");
            var testCodeFile = CreateTempFile("public class PasswordResetServiceTests { /* test implementation */ }");

            TestAnalysisRequest? capturedRequest = null;

            _mockAlalysisService.Setup(x => x.AnalyzeAsync(It.IsAny<TestAnalysisRequest>(), It.IsAny<CancellationToken>()))
                .Callback<TestAnalysisRequest, CancellationToken>((request, _) => capturedRequest = request)
                .ReturnsAsync(new TestAnalysisResult
                {
                    Suggestions = [new TestCaseSuggestion { 
                        Description = "Tests the password reset functionality.",
                        TestLevel = "Unit",
                        Reason = "Ensures that the password reset service behaves as expected."
                    }]
                });

            var command = new AnalyzeRequirementCommand(_mockAlalysisService.Object).Build();

            // Act
            var exitCode = await command.Parse([
                requirementFile,
                "--code", sourceCodeFile,
                "--tests", testCodeFile
                ]).InvokeAsync();

            //Assert
            exitCode.Should().Be(CliExitCodes.Success);
            capturedRequest.Should().NotBeNull();
            capturedRequest!.CodeContext.Should().HaveCount(2);
            capturedRequest.CodeContext.Should()
                .ContainSingle(item => item.Path == sourceCodeFile 
                && item.Kind == CodeContextKind.SourceCode
                && item.Content == "public class PasswordResetService { /* implementation */ }");
            capturedRequest.CodeContext.Should()
                .ContainSingle(item => item.Path == testCodeFile
                && item.Kind == CodeContextKind.Test
                && item.Content == "public class PasswordResetServiceTests { /* test implementation */ }");
        }

        [Fact]
        public async Task Build_WithMissingCodeFiles_ShouldReturnInvalidInputExitCode()
        {
            // Arrange
            var requirementFile = CreateTempFile("The system shall allow users to reset their password.");
            var missingSourceCodeFile = Path.Combine(Path.GetTempPath(), $"aiqalab-missing-source-{Guid.NewGuid()}.cs");

            var command = new AnalyzeRequirementCommand(_mockAlalysisService.Object).Build();

            // Act
            var exitCode = await command.Parse([
                requirementFile, "--code", missingSourceCodeFile])
                .InvokeAsync();

            // Assert
            exitCode.Should().Be(CliExitCodes.InvalidInput);
            _mockAlalysisService.Verify(x => x.AnalyzeAsync(It.IsAny<TestAnalysisRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        private string CreateTempFile(string content)
        {
            var tempFilePath = Path.Combine(Path.GetTempPath(), $"aiqalab-test-{Guid.NewGuid()}.tmp");
            File.WriteAllText(tempFilePath, content);
            _tempFiles.Add(tempFilePath);
            return tempFilePath;
        }

        public void Dispose()
        {
            foreach (var tempFile in _tempFiles)
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }
    }
}
