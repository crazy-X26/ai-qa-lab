using AiQaLab.AI.Services;
using AiQaLab.AI.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddSingleton<IAIClient>(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();

    var apiKey = configuration["AI:Gemini:ApiKey"];

    if (string.IsNullOrWhiteSpace(apiKey))
    {
        throw new InvalidOperationException(
            "Gemini API key is not configured.");
    }

    return new GeminiAIClient(apiKey);
});

builder.Services.AddSingleton<ITestAnalysisPromptBuilder, TestAnalysisPromptBuilder>();
builder.Services.AddSingleton<ITestAnalysisService, TestAnalysisService>();

using var host = builder.Build();

var testAnalysisService = host.Services.GetRequiredService<ITestAnalysisService>();

Console.WriteLine("AiQaLab CLI configured successfully.");

if (args.Length != 1)
{
    Console.Error.WriteLine("Usage: aIQaLab <requirement-file>");
    return;
}

var requirementFile = args[0];

if (!File.Exists(requirementFile))
{
    Console.Error.WriteLine($"Requirement file not found {requirementFile}");
    return;
}

var requirement = await File.ReadAllTextAsync(requirementFile);

var result = await testAnalysisService.AnalyzeAsync(requirement);

Console.WriteLine();
Console.WriteLine("Test suggestions:");
Console.WriteLine();

foreach (var suggestion in result.Suggestions)
{
    Console.WriteLine($"[{suggestion.TestLevel}]");
    Console.WriteLine(suggestion.Description);
    Console.WriteLine($"Reason: {suggestion.Reason}");
    Console.WriteLine();
}