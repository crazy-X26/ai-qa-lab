using AiQaLab.AI.Services;
using AiQaLab.AI.Services.Interfaces;
using AiQaLab.Cli.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.CommandLine;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddUserSecrets<Program>();

using var cancellationTokenSource = new CancellationTokenSource();

Console.CancelKeyPress += (_, EventArgs) =>
{
    EventArgs.Cancel = true;
    cancellationTokenSource.Cancel();
};

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
builder.Services.AddTransient<AnalyzeRequirementCommand>();

using var host = builder.Build();
var analyzeRequirementCommand = host.Services.GetRequiredService<AnalyzeRequirementCommand>();

var rootCommand = new RootCommand("AiQaLab - AI-powered QA tools");
rootCommand.Add(analyzeRequirementCommand.Build());

return await rootCommand.Parse(args).InvokeAsync();