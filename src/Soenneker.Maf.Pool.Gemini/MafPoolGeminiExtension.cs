using System.Threading;
using System.Threading.Tasks;
using GeminiDotnet;
using GeminiDotnet.Extensions.AI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Soenneker.Maf.Dtos.Options;
using Soenneker.Maf.Pool.Abstract;

namespace Soenneker.Maf.Pool.Gemini;

/// <summary>
/// Provides Gemini-specific registration extensions for <see cref="IMafPool"/>, enabling integration with Google Gemini via Microsoft Agent Framework.
/// </summary>
public static class MafPoolGeminiExtension
{
    /// <summary>
    /// Registers a Gemini model in the agent pool with optional rate/token limits.
    /// </summary>
    public static ValueTask AddGemini(this IMafPool pool, string poolId, string key, string modelId, string apiKey, string? endpoint = null,
        int? rps = null, int? rpm = null, int? rpd = null, int? tokensPerDay = null, string? instructions = null,
        CancellationToken cancellationToken = default)
    {
        var options = new MafOptions
        {
            ModelId = modelId,
            Endpoint = endpoint,
            ApiKey = apiKey,
            RequestsPerSecond = rps,
            RequestsPerMinute = rpm,
            RequestsPerDay = rpd,
            TokensPerDay = tokensPerDay,
            AgentFactory = (opts, _) =>
            {
                var geminiOptions = new GeminiClientOptions
                {
                    ApiKey = opts.ApiKey!,
                    ModelId = opts.ModelId!
                };
                IChatClient chatClient = new GeminiChatClient(geminiOptions);
                AIAgent agent = chatClient.AsAIAgent(instructions: instructions ?? "You are a helpful assistant.", name: opts.ModelId);
                return new ValueTask<AIAgent>(agent);
            }
        };

        return pool.Add(poolId, key, options, cancellationToken);
    }

    /// <summary>
    /// Unregisters a Gemini model from the agent pool and removes the associated cache entry.
    /// </summary>
    /// <returns>True if the entry existed and was removed; false if it was not present.</returns>
    public static ValueTask<bool> RemoveGemini(this IMafPool pool, string poolId, string key, CancellationToken cancellationToken = default)
    {
        return pool.Remove(poolId, key, cancellationToken);
    }
}
