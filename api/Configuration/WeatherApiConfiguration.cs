namespace DemoDay.Api.Configuration;

/// <summary>
/// Configuration for Weather API settings
/// </summary>
public class WeatherApiConfiguration
{
    public const string SectionName = "WeatherApi";

    /// <summary>
    /// OpenWeatherMap API key
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Base URL for the weather API
    /// </summary>
    public string BaseUrl { get; set; } = "https://api.openweathermap.org/data/2.5";

    /// <summary>
    /// Validates that all required configuration values are present
    /// </summary>
    public bool IsValid => !string.IsNullOrWhiteSpace(ApiKey);
}
