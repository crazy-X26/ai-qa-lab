using AiQaLab.AI.Services;
using AiQaLab.AI.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using System.CommandLine;

namespace AiQaLab.Cli.Commands
{
    public class AnalyzeRequirementCommand
    {
        private readonly ITestAnalysisService _analysisService;

        public AnalyzeRequirementCommand(ITestAnalysisService analysisService)
        {
            _analysisService = analysisService;
        }

        public Command Build()
        {
            var fileArgument = new Argument<FileInfo>("file")
            {
                Description = "Path to the requirement file."
            };

            var command = new Command(
                "analyze-requirement",
                "Analyze a software requirement and suggest test cases.");

            command.Add(fileArgument);

            command.SetAction(async (parseResult, cancellationToken) =>
            {
                var file = parseResult.GetValue(fileArgument);

                if (file == null)
                {
                    Console.Error.WriteLine("Requirement file is required.");
                    return CliExitCodes.InvalidInput;
                }

                if (!file.Exists)
                {
                    Console.Error.WriteLine($"Requirement not found: {file.FullName}");
                    return CliExitCodes.InvalidInput;
                }

                var requirement = await File.ReadAllTextAsync(file.FullName);

                try
                {
                    var result = await _analysisService.AnalyzeAsync(requirement, cancellationToken);

                    foreach (var suggestion in result.Suggestions)
                    {
                        Console.WriteLine($"[{suggestion.TestLevel}]");
                        Console.WriteLine(suggestion.Description);
                        Console.WriteLine($"Reason: {suggestion.Reason}");
                        Console.WriteLine();
                    }
                    return CliExitCodes.Success;
                }
                catch (OperationCanceledException)
                {
                    Console.Error.WriteLine("Analysis cancelled.");
                    return CliExitCodes.Cancelled;
                }
                catch (TestAnalysisException ex)
                {
                    Console.Error.WriteLine($"AI analysis failed: {ex.Message}");
                    return CliExitCodes.GeneralError;
                }
            });

            return command;
            
        }
    }
}
