using AiQaLab.AI.Services.Interfaces;
using System.CommandLine;

namespace AiQaLab_Cli.Commands
{
    public class AnalyzeRequirementCommand
    {
        private readonly ITestAnalysisService _analysisService;

        public AnalyzeRequirementCommand(ITestAnalysisService analysisService)
        {
            _analysisService = analysisService;
        }

        public Command Create()
        {
            var fileArgument = new Argument<FileInfo>("file")
            {
                Description = "Path to the requirement file."
            };

            var command = new Command(
                "analyze-requirement",
                "Analyze a software requirement and suggest test cases.");

            command.Add(fileArgument);

            command.SetAction(async parseResult =>
            {
                var file = parseResult.GetValue(fileArgument);

                if (file == null)
                {
                    Console.Error.WriteLine($"Requirement file is required.");
                    return;
                }

                if (!file.Exists)
                {
                    Console.Error.WriteLine($"Requirement not found: {file.FullName}");
                    return;
                }

                var requirement = await File.ReadAllTextAsync(file.FullName);

                var result = await _analysisService.AnalyzeAsync(requirement);

                foreach (var suggestion in result.Suggestions)
                {
                    Console.WriteLine($"[{suggestion.TestLevel}]");
                    Console.WriteLine(suggestion.Description);
                    Console.WriteLine($"Reason: {suggestion.Reason}");
                    Console.WriteLine();
                }
            });

            return command;
        }
    }
}
