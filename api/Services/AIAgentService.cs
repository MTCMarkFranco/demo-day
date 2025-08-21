using DemoDay.Api.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

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

/// <summary>
/// Simple weather tool for Semantic Kernel
/// </summary>
public class WeatherTool
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WeatherTool> _logger;
    private readonly WeatherApiConfiguration _config;

    public WeatherTool(
        HttpClient httpClient, 
        ILogger<WeatherTool> logger,
        IOptions<WeatherApiConfiguration> config)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _config = config.Value ?? throw new ArgumentNullException(nameof(config));

        if (!_config.IsValid)
        {
            throw new InvalidOperationException("WeatherApi configuration is invalid. ApiKey is required.");
        }
    }

    [KernelFunction("GetWeatherData")]
    [Description("Get current weather data for a specific city on a given date")]
    public async Task<string> GetWeatherData(
        [Description("The date to get weather for (YYYY-MM-DD format)")] string dateTime,
        [Description("The city name to get weather for")] string city,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting weather for {City} on {Date}", city, dateTime);

        try
        {
            // For current weather (OpenWeatherMap doesn't support historical dates without paid plan)
            var url = $"{_config.BaseUrl}/weather?q={city}&appid={_config.ApiKey}&units=metric";
            
            var response = await _httpClient.GetAsync(url, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Weather API returned {StatusCode} for {City}", response.StatusCode, city);
                return $"Unable to get weather data for {city}. Please check the city name.";
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var weatherData = JsonSerializer.Deserialize<WeatherApiResponse>(json);

            if (weatherData?.Main == null)
            {
                return $"No weather data available for {city}.";
            }

            var result = $"Weather in {weatherData.Name}:\n" +
                        $"Temperature: {weatherData.Main.Temp:F1}°C (feels like {weatherData.Main.FeelsLike:F1}°C)\n" +
                        $"Condition: {weatherData.Weather?.FirstOrDefault()?.Description ?? "Unknown"}\n" +
                        $"Humidity: {weatherData.Main.Humidity}%";

            if (weatherData.Wind != null)
            {
                result += $"\nWind: {weatherData.Wind.Speed:F1} m/s";
            }

            _logger.LogInformation("Weather data retrieved for {City}", city);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting weather for {City}", city);
            return $"Sorry, I couldn't get weather information for {city} right now.";
        }
    }

    // Simplified weather data models
    private class WeatherApiResponse
    {
        [JsonPropertyName("main")]
        public MainData? Main { get; set; }

        [JsonPropertyName("weather")]
        public WeatherInfo[]? Weather { get; set; }

        [JsonPropertyName("wind")]
        public WindInfo? Wind { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }

    private class MainData
    {
        [JsonPropertyName("temp")]
        public double Temp { get; set; }

        [JsonPropertyName("feels_like")]
        public double FeelsLike { get; set; }

        [JsonPropertyName("humidity")]
        public int Humidity { get; set; }
    }

    private class WeatherInfo
    {
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
    }

    private class WindInfo
    {
        [JsonPropertyName("speed")]
        public double Speed { get; set; }
    }
}
