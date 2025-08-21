using DemoDay.Api.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using System.Runtime.CompilerServices;

namespace DemoDay.Api.Services;

/// <summary>
/// Simplified AI agent service using Semantic Kernel with weather tool
/// </summary>
public interface IAIAgentService
{
    /// <summary>
    /// Streams an AI agent response with weather tool capability
    /// </summary>
    /// <param name="message">User question</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Async enumerable of characters for streaming response</returns>
    IAsyncEnumerable<char> StreamResponseAsync(string message, CancellationToken cancellationToken = default);
}

public class AIAgentService : IAIAgentService
{
    private readonly Kernel _kernel;
    private readonly ILogger<AIAgentService> _logger;

    public AIAgentService(
        IOptions<AzureOpenAIConfiguration> config, 
        ILogger<AIAgentService> logger,
        WeatherTool weatherTool)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        var azureConfig = config.Value ?? throw new ArgumentNullException(nameof(config));

        if (!azureConfig.IsValid)
        {
            throw new InvalidOperationException("AzureOpenAI configuration is invalid.");
        }

        _kernel = CreateKernel(azureConfig, weatherTool);
    }

    public async IAsyncEnumerable<char> StreamResponseAsync(
        string message, 
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message cannot be empty", nameof(message));

        _logger.LogInformation("Processing AI request: {Message}", message);

        var executionSettings = new AzureOpenAIPromptExecutionSettings
        {
            MaxTokens = 2000,
            Temperature = 0.7,
            TopP = 0.9,
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };

        var prompt = $"You are a helpful AI assistant. Please respond to: {message}";

        IAsyncEnumerable<StreamingKernelContent>? streamingResult = null;
        Exception? setupException = null;
        
        try
        {
            streamingResult = _kernel.InvokePromptStreamingAsync(
                prompt, 
                new KernelArguments(executionSettings),
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            setupException = ex;
        }

        if (setupException != null)
        {
            _logger.LogError(setupException, "Error setting up AI streaming");
            var errorMsg = "Sorry, I encountered an error. Please try again.";
            foreach (var character in errorMsg)
            {
                yield return character;
            }
            yield break;
        }

        if (streamingResult == null)
            yield break;

        await foreach (var content in streamingResult.WithCancellation(cancellationToken))
        {
            var text = content.ToString();
            if (!string.IsNullOrEmpty(text))
            {
                foreach (var character in text)
                {
                    yield return character;
                }
            }
        }

        _logger.LogInformation("AI response completed");
    }

    private Kernel CreateKernel(AzureOpenAIConfiguration config, WeatherTool weatherTool)
    {
        var builder = Kernel.CreateBuilder();

        builder.AddAzureOpenAIChatCompletion(
            deploymentName: config.DeploymentName,
            endpoint: config.Endpoint,
            apiKey: config.ApiKey,
            apiVersion: config.ApiVersion);

        // Add weather tool as a kernel plugin
        builder.Plugins.AddFromObject(weatherTool, "Weather");

        var kernel = builder.Build();
        _logger.LogInformation("Semantic Kernel initialized with weather tool");
        return kernel;
    }
}

