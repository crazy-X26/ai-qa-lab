using AiQaLab.AI.Models;

namespace AiQaLab.AI.Services.Interfaces
{
    public interface ITestAnalysisPromptBuilder
    {
        string Build(TestAnalysisRequest request);
    }
}
