$ErrorActionPreference = "Stop"

dotnet publish `
    .\src\AiQaLab.Cli\AiQaLab.Cli.csproj `
    -c Release `
    -r win-x64 `
    --self-contained false