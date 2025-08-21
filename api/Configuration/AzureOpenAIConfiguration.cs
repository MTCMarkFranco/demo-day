namespace DemoDay.Api.Configuration;

/// <summary>
/// Configuration settings for Azure OpenAI service integration
/// </summary>
public class AzureOpenAIConfiguration
{
    public const string SectionName = "AzureOpenAI";

    /// <summary>
    /// Azure OpenAI API key for authentication
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Azure OpenAI service endpoint URL
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// Azure OpenAI deployment name (model deployment)
    /// </summary>
    public string DeploymentName { get; set; } = string.Empty;

    /// <summary>
    /// Azure OpenAI API version
    /// </summary>
    public string ApiVersion { get; set; } = string.Empty;

    /// <summary>
    /// Validates that all required configuration values are present
    /// </summary>
    public bool IsValid => 
        !string.IsNullOrWhiteSpace(ApiKey) &&
        !string.IsNullOrWhiteSpace(Endpoint) &&
        !string.IsNullOrWhiteSpace(DeploymentName) &&
        !string.IsNullOrWhiteSpace(ApiVersion);
}
