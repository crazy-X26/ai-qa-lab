# AI QA Lab

A practical project exploring how Large Language Models can augment software testing and QA workflows — as a set of opt-in capabilities the QA engineer decides whether and how to use, not as an autonomous test generator.

## Vision

Build a flexible set of AI-assisted QA capabilities that can be integrated into different QA workflows, from requirement review and test design to regression analysis and failure triage.

AiQaLab is intended to complement existing QA processes and tools rather than replace them. Projects may differ in methodology, technology stack, automation maturity, and available QA artifacts.

## Design Principles

* **No enforced process.** AiQaLab provides opt-in capabilities that can be integrated into existing QA workflows, regardless of methodology, technology stack, or automation maturity.
* **AI proposes, QA decides.** AI output is treated as a suggestion, not as a source of truth. No automatic merges or test changes without QA review.
* **Provider-agnostic by design.** AI-backed capabilities use the `IAIClient` abstraction so they can run against different cloud or local/on-premise LLM providers. This is particularly important when requirements, source code, test data, or other QA artifacts must not leave the organization.
* **Grounded in project context.** AI suggestions should be based on relevant project information such as requirements, source code, and existing tests rather than only on generic prompts. The goal is not to make the model smarter, but to provide it with the right context at the right point in the QA workflow.
* **Existing resources first.** AiQaLab should work with existing requirements, test cases, source code, coverage reports, test results, defect information, and other QA artifacts rather than replacing established tools.

## Current Capability

The current vertical slice focuses on **AI-assisted test analysis**.

A requirement can be analyzed together with optional, QA-selected project context such as:

* relevant source code
* existing tests

The context is incorporated into the prompt so that suggested tests can be grounded in the actual implementation.

For example, source code can provide concrete validation rules such as:

* minimum and maximum values
* regular expressions
* required fields
* conditional validation
* implementation-specific behavior

Existing tests can be provided to help identify gaps instead of suggesting cases that are already covered.

The resulting suggestions contain:

* test description
* suggested test level (`Unit`, `Integration`, `EndToEnd`)
* reasoning

The AI output is returned as structured JSON and deserialized into the application's domain model.

## AI Providers

AiQaLab uses a provider-neutral `IAIClient` abstraction.

| Client              | Status                                                                        |
| ------------------- | ----------------------------------------------------------------------------- |
| `GeminiAIClient`    | Implemented — Gemini with structured JSON output                              |
| `OllamaAIClient`    | Implemented — local LLM with structured JSON output                           |
| `AnthropicAIClient` | In progress — provider abstraction implemented, API calls not yet implemented |

The current local integration uses Ollama with the `qwen3:4b` model.

The Ollama integration demonstrates that the same QA analysis flow can be executed against a locally running LLM without changing the consuming services.

Local inference is particularly relevant for scenarios where sensitive requirements, source code, test data, or other project information cannot be sent to a cloud provider.

## Usage

The current CLI exposes the requirement analysis workflow:

```bash
dotnet run --project src/AiQaLab.Cli -- analyze-requirement path/to/requirement.txt
```

The workflow reads a plain-text requirement and produces structured test case suggestions.

For the Gemini provider, the API key is configured via user secrets:

```text
AI:Gemini:ApiKey
```

Local Ollama inference requires a running Ollama instance with the configured model available locally.

## Project Structure

* `AiQaLab.Core` — shared domain models
* `AiQaLab.AI` — provider-neutral AI abstraction and QA analysis logic
* `AiQaLab.Cli` — command-line entry point for AI-assisted QA capabilities
* `AiQaLab.DemoApp` — small ASP.NET Core application ("Northwind Logistics") used as a system under test
* `AiQaLab.Tool` — reserved for a possible future review UI for AI suggestions
* `tests/AiQaLab.Tests` — unit and AI integration tests
* `tests/AiQaLab.PlaywrightTests` — Playwright end-to-end tests for the demo application

## Status

### Implemented

* Requirement → test case suggestions via CLI
* Structured `TestAnalysisResult` output
* Provider-neutral `IAIClient` abstraction
* Provider-neutral JSON Schema using `System.Text.Json.Nodes.JsonNode`
* Gemini AI client
* Local Ollama AI client
* Structured output with Ollama
* Unit tests for AI clients and analysis logic
* Integration tests against real AI providers
* QA-selected source code and existing test context can be included in test analysis
* Playwright end-to-end tests for the demo application

### In Progress

* Anthropic AI client
* Further refinement of code-aware test analysis
* Evaluation of how project context should be selected and provided to AiQaLab

### Planned

* **Code-aware test design** — improve analysis of source code, validation rules, and existing tests to identify meaningful test cases and potential coverage gaps
* **Test data proposals** — derive boundary values, equivalence classes, and synthetic sample data from actual implementation constraints
* **Regression impact analysis** — use requirements, source changes, existing tests, and other project artifacts to identify potentially affected behavior
* **Playwright test drafts** — turn approved test cases into Playwright drafts that reuse existing project conventions such as `data-testid` selectors
* **Failure triage** — compare CI failures with recent changes and the originating test case to suggest likely regressions versus likely flaky failures
* **Review UI** — provide a lightweight surface for reviewing, accepting, rejecting, or adjusting AI suggestions rather than providing a generic chat interface

### Not Pursuing (for now)

* **Freeform prompt playground** — general-purpose assistants and IDE copilots already provide this capability well. AiQaLab focuses on integrating AI into QA workflows and project context instead.
* **Generic log analyzer** — already a commodity capability in IDE copilots and observability tools; AiQaLab focuses instead on narrower QA-specific failure triage.
* **Generic bug report generator** — already covered well by test-management integrations and writing assistants.
* **Semantic Kernel** — currently adds an unnecessary orchestration layer while a provider-neutral `IAIClient` abstraction is sufficient. This may be revisited if future workflows require multi-model or tool orchestration.

## Tech Stack

* .NET 9
* ASP.NET Core
* `System.Text.Json` / `System.Text.Json.Nodes`
* Google GenAI SDK
* Anthropic SDK
* Ollama
* Playwright
* xUnit
* FluentAssertions
* Moq
