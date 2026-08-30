[![](https://img.shields.io/nuget/v/soenneker.maf.pool.gemini.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.maf.pool.gemini/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.maf.pool.gemini/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.maf.pool.gemini/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.maf.pool.gemini.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.maf.pool.gemini/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.maf.pool.gemini/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.maf.pool.gemini/actions/workflows/codeql.yml)

# Soenneker.Maf.Pool.Gemini

Provides Gemini-specific registration extensions for `IMafPool`, enabling integration with Google Gemini via Microsoft Agent Framework.

## Install

```bash
dotnet add package Soenneker.Maf.Pool.Gemini
```

## Usage

```csharp
using Soenneker.Maf.Pool.Gemini;
using Soenneker.Maf.Pool.Abstract;

await pool.AddGemini(
    poolId: "chat",
    key: "gemini-primary",
    modelId: "gemini-2.5-flash",
    apiKey: configuration["GEMINI_API_KEY"]!,
    rpm: 60,
    instructions: "Answer concisely.",
    cancellationToken: cancellationToken);

(AIAgent? agent, IMafPoolEntry? entry) =
    await pool.GetAvailable("chat", cancellationToken);
```

`pool` is an `IMafPool` registered by `Soenneker.Maf.Pool`. Pass `endpoint` only when targeting a compatible non-default Gemini endpoint.

## What you get

- `MafPoolGeminiExtension` — Provides Gemini-specific registration extensions for `IMafPool`, enabling integration with Google Gemini via Microsoft Agent Framework.

## API at a glance

| API | What it does | Result / important behavior |
| --- | --- | --- |
| `MafPoolGeminiExtension.AddGemini(pool, poolId, key, modelId, apiKey, endpoint, rps, rpm, rpd, tokensPerDay, instructions, cancellationToken)` | Registers a Gemini model in the agent pool with optional rate/token limits. | A task that completes when the gemini addition is complete. |
| `MafPoolGeminiExtension.RemoveGemini(pool, poolId, key, cancellationToken)` | Unregisters a Gemini model from the agent pool and removes the associated cache entry. | True if the entry existed and was removed; false if it was not present. |

## Practical notes

- The agent is created lazily and reused until its entry is removed.
- Store the API key in a secret provider; the pool retains it in the entry options while the entry is registered.
- Omitted instructions default to `You are a helpful assistant.`
- Checkout consumes one request from the configured quota. `tokensPerDay` is not reconciled against actual provider token usage.
