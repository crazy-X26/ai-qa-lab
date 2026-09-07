using AiQaLab.AI.Models;

namespace AiQaLab.AI.Services.Interfaces
{
    public interface ITestAnalysisService
    {
        Task<TestAnalysisResult> AnalyzeAsync(TestAnalysisRequest request, CancellationToken cancellationToken = default);
    }
}
