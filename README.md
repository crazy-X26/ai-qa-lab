# AI QA Lab

A practical project exploring how Large Language Models can augment software testing and QA workflows — as a set of opt-in tools the QA engineer decides whether and how to use, not as an autonomous test generator.

## Vision

Build a flexible set of AI-assisted QA capabilities that can be integrated into different QA workflows, from requirement review and test design to regression analysis and failure triage.

## Design Principles

- **No enforced process.** AiQaLab provides opt-in capabilities that can be integrated into existing QA workflows, regardless of methodology, technology stack, or automation maturity.
- **AI proposes, QA decides.** AI output is treated as a suggestion, not as a source of truth. No automatic merges or test changes without QA review.
- **Provider-agnostic by design.** Every AI-backed feature goes through the `IAIClient` abstraction so it can run against a cloud model or a local/on-prem LLM — the latter matters whenever requirements, code, or test data are sensitive.
- **Grounded in project context.** A suggestion is only worth building when it uses information a generic chat assistant doesn't have — the actual code, its validation rules, and the tests that already exist — not just the raw requirement text. The goal is not to make the model smarter, but to give it the right context at the right point in the QA workflow.
- Existing resources first. AiQaLab should work with existing requirements, test cases, source code, coverage reports, test results, defect information, and other QA artifacts rather than replacing established tools.

## Usage

```bash
dotnet run --project src/AiQaLab.Cli -- analyze-requirement path/to/requirement.txt
```

Reads a plain-text requirement and prints suggested test cases with test level and reasoning. Requires an AI provider API key configured via user secrets (e.g. `AI:Gemini:ApiKey`).

## Project Structure

- `AiQaLab.Core` — shared domain models
- `AiQaLab.AI` — the provider-agnostic AI abstraction and the requirement-analysis pipeline
- `AiQaLab.Cli` — command-line entry point for the AI features
- `AiQaLab.DemoApp` — a small login demo app ("Northwind Logistics") used purely as a system under test
- `AiQaLab.Tool` — reserved for a possible future review UI for AI suggestions (see Planned below)
- `tests/AiQaLab.Tests` — unit tests (mocked AI client) and integration tests against a real provider (tagged `Category=AI`)
- `tests/AiQaLab.PlaywrightTests` — end-to-end tests for the demo app, using stable `data-testid` selectors

## Status

### AI Providers

| Client | Status |
|---|---|
| `GeminiAIClient` | Implemented — calls `gemini-3.1-flash-lite` with structured JSON output |
| `AnthropicAIClient` | Stub — throws `NotImplementedException`, next in line now that the interface no longer assumes a Gemini-specific schema type |
| Local LLM adapter (e.g. Ollama) | Not started — needed for scenarios where requirements, code, or test data can't leave the organization |

### Implemented

- Requirement → test case suggestions (unit / integration / end-to-end), via CLI
- Provider-neutral `IAIClient` abstraction (`System.Text.Json.Nodes`-based schema)
- Playwright end-to-end tests for the demo app

### Planned

- **Local / OpenAI-compatible LLM adapter** — for requirements, code, or test data that can't go to a cloud provider
- **Code-aware test design** — feed the prompt actual code context (signatures, validation rules, existing tests) instead of just the requirement text, so boundary values and equivalence classes are grounded in the real field constraints, and so gaps against existing tests can be flagged
- **Test data proposals** — derive boundary values, equivalence classes, and synthetic sample data from field rules (min/max, regex, DTOs); a natural first use case for the local LLM adapter
- **Playwright test drafts** — turn an approved test case into a Playwright draft that reuses the existing `data-testid` conventions, instead of guessing selectors
- **Failure triage** — on a CI failure, compare the diff, the last commit, and the originating test case to suggest "likely regression" vs. "likely flaky", scoped narrowly rather than as a generic log summarizer
- **Review UI** (`AiQaLab.Tool`) — a lightweight surface for accepting, rejecting, or adjusting AI suggestions, not a freeform chat playground

### Not Pursuing (for now)

- **Freeform prompt playground** — a chat UI would just re-implement what general-purpose assistants and IDE copilots already do
- **Generic log analyzer** — already a commodity feature in IDE copilots and observability tools; see the narrower failure-triage idea above instead
- **Generic bug report generator** — already covered well by test-management add-ons and writing assistants
- **Semantic Kernel** — an extra orchestration layer with no current payoff while a single `IAIClient` interface covers every feature; worth revisiting only if multiple models/tools need to be orchestrated within one call

## Tech Stack

- .NET 9, ASP.NET Core
- `System.Text.Json` / `System.Text.Json.Nodes` for provider-neutral schemas
- Google GenAI SDK (Gemini client)
- Anthropic SDK (client in progress)
- Playwright
- xUnit, FluentAssertions, Moq
