using DemoDay.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using System.Text;

namespace DemoDay.Api.Controllers;

/// <summary>
/// Controller for AI Agent streaming endpoints
/// Provides character-by-character streaming of AI responses with weather integration
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AgentController : ControllerBase
{
    private readonly IAIAgentService _aiAgentService;
    private readonly ILogger<AgentController> _logger;

    public AgentController(
        IAIAgentService aiAgentService,
        ILogger<AgentController> logger)
    {
        _aiAgentService = aiAgentService ?? throw new ArgumentNullException(nameof(aiAgentService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Streams an AI agent response character by character
    /// </summary>
    /// <param name="query">The user's question or prompt for the AI agent (max 2000 characters)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Streaming response with Markdown-formatted content</returns>
    /// <response code="200">Returns a streaming response with AI-generated content</response>
    /// <response code="400">Missing or invalid query parameter</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("stream")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> StreamResponse(
        [FromQuery(Name = "query")] string query,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            _logger.LogWarning("Empty or missing 'query' parameter for streaming request");
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid Request",
                Detail = "'query' parameter is required and cannot be empty.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        try
        {
            _logger.LogInformation("Starting streaming response for message: {Message}", query);

            // Set appropriate headers for streaming
            Response.ContentType = "text/plain; charset=utf-8";
            Response.Headers["Cache-Control"] = "no-cache";
            Response.Headers["Connection"] = "keep-alive";
            
            // Disable response buffering to enable true streaming
            Response.Headers["X-Accel-Buffering"] = "no";

            var responseStream = Response.Body;
            var written = 0;

            await foreach (var character in _aiAgentService.StreamResponseAsync(query, cancellationToken))
            {
                // Convert character to bytes and write to response stream
                var bytes = Encoding.UTF8.GetBytes(character.ToString());
                await responseStream.WriteAsync(bytes, cancellationToken);
                await responseStream.FlushAsync(cancellationToken);
                
                written++;
                
                // Log progress every 100 characters for monitoring
                if (written % 100 == 0)
                {
                    _logger.LogDebug("Streamed {Characters} characters", written);
                }
            }

            _logger.LogInformation("Completed streaming response. Total characters: {Total}", written);
            return new EmptyResult();
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Streaming request was cancelled by client");
            return new EmptyResult();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument in streaming request");
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid Request",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during streaming response");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = "An unexpected error occurred while processing your request.",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }

    /// <summary>
    /// Health check endpoint for the AI agent service
    /// </summary>
    /// <returns>Service status information</returns>
    /// <response code="200">Service is healthy</response>
    [HttpGet("health")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public IActionResult Health()
    {
        return Ok(new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow,
            service = "AI Agent Service",
            version = "1.0.0"
        });
    }

    /// <summary>
    /// Get available capabilities and features of the AI agent
    /// </summary>
    /// <returns>Agent capabilities information</returns>
    /// <response code="200">Returns agent capabilities</response>
    [HttpGet("capabilities")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public IActionResult GetCapabilities()
    {
        return Ok(new
        {
            streaming = true,
            weatherIntegration = true,
            markdownOutput = true,
            apiType = "query-parameter",
            maxQueryLength = 2000,
            endpoint = "GET /api/agent/stream?query={your-question}",
            supportedFormats = new[] { "markdown", "text" },
            weatherFeatures = new[]
            {
                "Current weather conditions",
                "Temperature and feels-like temperature", 
                "Humidity and pressure",
                "Wind speed and direction",
                "Automatic location extraction",
                "Weather keyword detection"
            },
            kernelFunctions = new[]
            {
                "Weather-GetWeather: Get current weather for a location",
                "Weather-ContainsWeatherQuery: Check if message contains weather queries",
                "Weather-ExtractLocation: Extract location from user message"
            }
        });
    }
}
