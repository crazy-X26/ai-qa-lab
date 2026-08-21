using AiQaLab.AI.Models;
using AiQaLab.AI.Schemas;
using AiQaLab.AI.Services.Interfaces;
using System.Text.Json;

namespace AiQaLab.AI.Services
{
    public class TestAnalysisService : ITestAnalysisService
    {
        private readonly IAIClient _aiClient;
        private readonly ITestAnalysisPromptBuilder _promptBuilder;

        public TestAnalysisService(IAIClient aiClient, ITestAnalysisPromptBuilder promptBuilder)
        {
            _aiClient = aiClient;
            _promptBuilder = promptBuilder;
        }

        public async Task<TestAnalysisResult> AnalyzeAsync(string requirement, CancellationToken cancellationToken = default)
        {
            var prompt = _promptBuilder.Build(requirement);

            try
            {
                var response = await _aiClient.SendStructuredAsync(
                    prompt,
                    TestAnalysisSchema.Create(),
                    cancellationToken);

                var result = JsonSerializer.Deserialize<TestAnalysisResult>(
                    response,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (result is null)
                {
                    throw new TestAnalysisException(
                        "The AI returned an empty test analysis result.");
                }

                if (result.Suggestions.Count == 0)
                {
                    throw new TestAnalysisException(
                        "The AI returned no test case suggestions.");
                }

                return result;
            }
            catch (JsonException ex)
            {
                throw new TestAnalysisException(
                    "The AI returned an invalid test analysis result.",
                    ex);
            }
        }
    }
}
