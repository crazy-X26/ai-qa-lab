using AiQaLab.AI.Models;
using AiQaLab.AI.Services;
using AiQaLab.AI.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using System.CommandLine;

namespace AiQaLab.Cli.Commands
{
    public class AnalyzeRequirementCommand
    {
        private const int MaxCodeContentenChars = 20_000;
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

            var codeOptions = new Option<FileInfo[]>("--code")
            {
                Description = "Paths to a source file to include as code context. Repeatable."
            };

            var testOptions = new Option<FileInfo[]>("--tests")
            {
                Description = "Paths to an existing test file to include as test context. Repeatable."
            };

            var command = new Command(
                "analyze-requirement",
                "Analyze a software requirement and suggest test cases.");

            command.Add(fileArgument);
            command.Add(codeOptions);
            command.Add(testOptions);

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

                var codeFiles = parseResult.GetValue(codeOptions) ?? Array.Empty<FileInfo>();
                var testFiles = parseResult.GetValue(testOptions) ?? Array.Empty<FileInfo>();

                var codeContext = new List<CodeContextItem>();

                foreach (var codeFile in codeFiles)
                {
                    if (!codeFile.Exists)
                    {
                        Console.Error.WriteLine($"Code file not found: {codeFile.FullName}");
                        return CliExitCodes.InvalidInput;
                    }
                    
                    codeContext.Add(new CodeContextItem
                    {
                        Path = codeFile.FullName,
                        Kind = CodeContextKind.SourceCode,
                        Content = await File.ReadAllTextAsync(codeFile.FullName)
                    });
                }

                foreach (var testFile in testFiles)
                {
                    if (!testFile.Exists)
                    {
                        Console.Error.WriteLine($"Test file not found: {testFile.FullName}");
                        return CliExitCodes.InvalidInput;
                    }
                    
                    codeContext.Add(new CodeContextItem
                    {
                        Path = testFile.FullName,
                        Kind = CodeContextKind.Test,
                        Content = await File.ReadAllTextAsync(testFile.FullName)
                    });
                }

                var codeContextCharCount = codeContext.Sum(c => c.Content.Length);

                if(codeContextCharCount > MaxCodeContentenChars)
                {
                    Console.Error.WriteLine($"Code context is too large ({codeContextCharCount} characters, limit {MaxCodeContentenChars}).");
                    return CliExitCodes.InvalidInput;
                }

                var requirement = await File.ReadAllTextAsync(file.FullName);

                var request = new TestAnalysisRequest
                {
                    Requirement = requirement,
                    CodeContext = codeContext
                };

                try
                {
                    var result = await _analysisService.AnalyzeAsync(request, cancellationToken);

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
