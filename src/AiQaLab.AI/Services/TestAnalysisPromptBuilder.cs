using AiQaLab.AI.Services.Interfaces;

namespace AiQaLab.AI.Services
{
    public class TestAnalysisPromptBuilder : ITestAnalysisPromptBuilder
    {
        public string Build(string requirement)
        {
            return $"""
                You are a software quality assurance assistant.

                Analyze the following software requirement and suggest appropriate test cases.

                Consider the appropriate test level for each suggestion:
                - Unit
                - Integration
                - EndToEnd

                Avoid suggesting redundant tests at multiple levels unless the higher-level
                test provides meaningful additional coverage.

                Requirement:
                {requirement}
                """;
        }
    }
}
