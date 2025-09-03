# Demo Day AI Agent API - Agentic Rebuild Prompt

Create a complete .NET 8 Web API project that implements a streaming AI agent with weather integration capabilities. Use the following specifications to rebuild the entire project from scratch.

## Project Overview
Build a streaming AI agent API that provides character-by-character responses using Microsoft Semantic Kernel with Azure OpenAI integration and weather tool capabilities via OpenWeatherMap API.

## CLI Commands to Execute

```bash
# Create new Web API project
dotnet new webapi -n DemoDay.Api --framework net8.0 --use-controllers true
cd DemoDay.Api

# Initialize user secrets for secure development configuration
dotnet user-secrets init

# Add required NuGet packages
dotnet add package Microsoft.SemanticKernel --version 1.63.0
dotnet add package Microsoft.Extensions.Http --version 9.0.8
dotnet add package Microsoft.AspNetCore.OpenApi --version 8.0.19
dotnet add package Swashbuckle.AspNetCore --version 6.6.2

# Clean up default files
rm Controllers/WeatherForecastController.cs
rm WeatherForecast.cs
```

## Project Structure Requirements

```
DemoDay.Api/
├── Program.cs                              # Main entry point with service configuration
├── DemoDay.Api.csproj                      # Project file with dependencies
├── DemoDay.Api.http                        # HTTP test file for manual testing
├── appsettings.json                        # Base configuration (no secrets)
├── appsettings.Development.json             # Development-specific settings
├── Configuration/
│   ├── AzureOpenAIConfiguration.cs         # Azure OpenAI config model
│   └── WeatherApiConfiguration.cs          # Weather API config model
├── Controllers/
│   └── AgentController.cs                  # Main API controller with streaming endpoint
└── Services/
    ├── AIAgentService.cs                   # Semantic Kernel AI service implementation
    └── WeatherService.cs                   # Weather tool for Semantic Kernel
```

## Core Implementation Requirements

### 1. Configuration Classes

**File: Configuration/AzureOpenAIConfiguration.cs**
- Create configuration class with properties: ApiKey, Endpoint, DeploymentName, ApiVersion
- Include IsValid property for validation
- Use const string SectionName = "AzureOpenAI"

**File: Configuration/WeatherApiConfiguration.cs**
- Create configuration class with properties: ApiKey, BaseUrl (default: "https://api.openweathermap.org/data/2.5")
- Include IsValid property for validation
- Use const string SectionName = "WeatherApi"

### 2. Services Implementation

**File: Services/AIAgentService.cs**
- Implement IAIAgentService interface with StreamResponseAsync method
- Method signature: `IAsyncEnumerable<char> StreamResponseAsync(string message, CancellationToken cancellationToken = default)`
- Use Microsoft Semantic Kernel with Azure OpenAI chat completion
- Configure kernel with weather tool as plugin
- Implement character-by-character streaming using `IAsyncEnumerable<char>`
- Use AzureOpenAIPromptExecutionSettings with MaxTokens=2000, Temperature=0.7, TopP=0.9
- Enable FunctionChoiceBehavior.Auto() for automatic tool usage
- Include comprehensive error handling and logging

**File: Services/WeatherService.cs**
- Create WeatherTool class for Semantic Kernel integration
- Implement `[KernelFunction("GetWeatherData")]` method
- Method parameters: dateTime (string), city (string), CancellationToken
- Use HttpClient to call OpenWeatherMap API
- Return formatted weather data as string (temperature, condition, humidity, wind)
- Include JSON deserialization models for API response
- Handle API errors gracefully with user-friendly messages

### 3. Controller Implementation

**File: Controllers/AgentController.cs**
- Create AgentController with three endpoints:
  1. `GET /api/agent/stream?query={query}` - Main streaming endpoint
  2. `GET /api/agent/health` - Health check
  3. `GET /api/agent/capabilities` - Agent capabilities info
- Streaming endpoint must:
  - Accept query parameter (max 2000 characters)
  - Set appropriate headers: ContentType="text/plain; charset=utf-8", Cache-Control="no-cache", Connection="keep-alive"
  - Stream characters using UTF-8 encoding
  - Handle cancellation and errors gracefully
  - Return EmptyResult() after streaming completes
- Include comprehensive XML documentation comments
- Use proper HTTP status codes and ProblemDetails for errors

### 4. Program.cs Configuration

**Requirements:**
- Configure logging (Console + Debug providers)
- Add User Secrets support for development
- Configure Options pattern for both configuration classes
- Register HttpClient for weather API calls
- Register AIAgentService and WeatherTool with dependency injection
- Add Controllers with API behavior options
- Configure Swagger/OpenAPI with comprehensive documentation
- Add CORS support (AllowAnyOrigin for development)
- Add health checks
- Configure Swagger UI at root path ("/") in development
- Add `/health` endpoint mapping
- Add `/info` endpoint with application metadata and features list
- Make Program class partial for testing accessibility

### 5. Configuration Files

**File: appsettings.json**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.SemanticKernel": "Information",
      "DemoDay.Api": "Information"
    }
  },
  "AllowedHosts": "*",
  "AzureOpenAI": {
    "ApiKey": "",
    "Endpoint": "",
    "DeploymentName": "",
    "ApiVersion": ""
  },
  "WeatherApi": {
    "ApiKey": "",
    "BaseUrl": "https://api.openweathermap.org/data/2.5",
    "TimeoutSeconds": 30,
    "MaxRetries": 3,
    "RequestsPerMinute": 60
  }
}
```

**File: appsettings.Development.json**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.SemanticKernel": "Debug",
      "DemoDay.Api": "Debug"
    }
  }
}
```

### 6. Project File Configuration

**File: DemoDay.Api.csproj**
- Target framework: net8.0
- Enable nullable reference types
- Enable implicit usings
- Include UserSecretsId for development
- Add PackageReference for all required packages with specific versions
- Add InternalsVisibleTo attribute for testing

### 7. HTTP Test File

**File: DemoDay.Api.http**
Create comprehensive test scenarios:
- Health check endpoints
- Application info endpoint
- Agent capabilities endpoint
- Weather-enabled queries
- Non-weather queries
- Use variable `@api_base = https://localhost:7139`

## Technical Specifications

### Streaming Implementation
- Use `IAsyncEnumerable<char>` for true character-by-character streaming
- Implement proper HTTP headers for streaming responses
- Handle client disconnections gracefully via CancellationToken
- Log streaming progress every 100 characters

### Semantic Kernel Integration
- Use Azure OpenAI chat completion connector
- Register weather tool as kernel plugin with name "Weather"
- Enable automatic function calling with FunctionChoiceBehavior.Auto()
- Implement proper error handling for AI service failures

### Weather Tool Features
- KernelFunction decorated method for Semantic Kernel
- Accept date and city parameters with proper descriptions
- Call OpenWeatherMap API with metric units
- Format response as readable text with temperature, conditions, humidity, wind
- Handle API rate limiting and error responses

### Security & Configuration
- Use .NET User Secrets for development (never commit API keys)
- Support environment variables for production deployment
- Validate configuration on startup with IsValid properties
- Implement comprehensive error handling without exposing internals

### API Documentation
- Complete Swagger/OpenAPI documentation
- XML comments on all public methods and controllers
- Response type attributes with proper status codes
- Example requests in HTTP test file

### Logging & Monitoring
- Structured logging throughout the application
- Different log levels for development vs production
- Progress logging for streaming operations
- Error logging with context preservation

## Development Workflow Commands

```bash
# Set development secrets (replace with actual values)
dotnet user-secrets set "AzureOpenAI:ApiKey" "your-azure-openai-api-key"
dotnet user-secrets set "AzureOpenAI:Endpoint" "https://your-resource.openai.azure.com/"
dotnet user-secrets set "AzureOpenAI:DeploymentName" "gpt-4.1"
dotnet user-secrets set "AzureOpenAI:ApiVersion" "2024-12-01-preview"
dotnet user-secrets set "WeatherApi:ApiKey" "your-openweathermap-api-key"

# Build and run
dotnet build
dotnet run

# Run with watch mode for development
dotnet watch run
```

## Expected Endpoints After Implementation

1. **GET /api/agent/stream?query={message}** - Stream AI response character-by-character
2. **GET /api/agent/health** - Service health check
3. **GET /api/agent/capabilities** - Agent features and capabilities
4. **GET /health** - Application health check
5. **GET /info** - Application information and features
6. **GET /** - Swagger UI documentation (development only)

## Success Criteria

✅ Project builds without errors or warnings  
✅ All endpoints respond correctly  
✅ Streaming works character-by-character  
✅ Weather integration functions via Semantic Kernel  
✅ Configuration is secure (no hardcoded secrets)  
✅ Swagger documentation is comprehensive  
✅ Error handling is robust  
✅ Logging provides adequate visibility  
✅ HTTP test file works for all scenarios  

## Notes for Agentic Implementation

- Follow the exact file structure and naming conventions specified
- Implement all classes with the exact method signatures provided
- Use the specified NuGet package versions for consistency
- Ensure proper async/await patterns throughout
- Include all required using statements
- Follow .NET 8 conventions and best practices
- Test each component as you build it using the HTTP test file
- Verify streaming works by testing character-by-character output
- Ensure weather tool integration works through Semantic Kernel function calling

This prompt provides complete specifications to rebuild the Demo Day AI Agent API project from scratch using CLI commands and modern .NET 8 development practices.
