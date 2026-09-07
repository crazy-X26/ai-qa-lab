using AiQaLab.AI.Models;
using AiQaLab.AI.Services.Interfaces;
using System.Text;

namespace AiQaLab.AI.Services
{
    public class TestAnalysisPromptBuilder : ITestAnalysisPromptBuilder
    {
        public string Build(TestAnalysisRequest request)
        {
            var sb = new StringBuilder();

            sb.AppendLine("You are a software quality assurance assistant.");
            sb.AppendLine();
            sb.AppendLine("Analyze the following software requirement and suggest appropriate test cases.");
            sb.AppendLine();
            sb.AppendLine("Consider the appropriate test level for each suggestion:");
            sb.AppendLine("- Unit");
            sb.AppendLine("- Integration");
            sb.AppendLine("- EndToEnd");
            sb.AppendLine();
            sb.AppendLine("Avoid suggesting redundant tests at multiple levels unless the higher-level");
            sb.AppendLine("test provides meaningful additional coverage.");
            sb.AppendLine();
            sb.AppendLine("Requirement:");
            sb.AppendLine(request.Requirement);

            var sourceItems = request.CodeContext.Where(item => item.Kind == CodeContextKind.SourceCode).ToList();
            var testItems = request.CodeContext.Where(item => item.Kind == CodeContextKind.Test).ToList();

            if(sourceItems.Any())
            {
                sb.AppendLine();
                sb.AppendLine("Relevant source code. Ground your boundary values and equivalence classes in the");
                sb.AppendLine("actual validation rules below (attributes, ranges, regex) instead of assuming generic ones.");
                foreach (var item in sourceItems)
                {
                    sb.AppendLine($"-- {item.Path} --");
                    sb.AppendLine(item.Content);
                }
            }

            if(testItems.Any())
            {
                sb.AppendLine();
                sb.AppendLine("Existing tests. Do not repeat cases already covered here. Focus new suggestions");
                sb.AppendLine("on gaps against this existing coverage and edge cases.");
                foreach (var item in testItems)
                {
                    sb.AppendLine($"-- {item.Path} --");
                    sb.AppendLine(item.Content);
                }
            }

            sb.AppendLine();
            sb.AppendLine("Requirement:");
            sb.AppendLine(request.Requirement);

            return sb.ToString();
        }
    }
}
