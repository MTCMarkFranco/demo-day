using DemoDay.Api.Configuration;
using DemoDay.Api.Services;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add configuration for user secrets and environment variables
builder.Configuration.AddUserSecrets<Program>();

// Configure options from configuration
builder.Services.Configure<AzureOpenAIConfiguration>(
    builder.Configuration.GetSection(AzureOpenAIConfiguration.SectionName));

builder.Services.Configure<WeatherApiConfiguration>(
    builder.Configuration.GetSection(WeatherApiConfiguration.SectionName));

// Add HTTP client for weather API calls
builder.Services.AddHttpClient();

// Register weather tool for dependency injection
builder.Services.AddScoped<WeatherTool>();

// Register simplified AI agent service
builder.Services.AddScoped<IAIAgentService, AIAgentService>();

// Add controllers
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = false;
    });

// Configure API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Demo Day AI Agent API",
        Version = "v1",
        Description = "A streaming AI agent API with weather integration using Microsoft Semantic Kernel and Azure OpenAI",
        Contact = new OpenApiContact
        {
            Name = "Demo Day Team",
            Email = "demo@example.com"
        }
    });

    // Include XML comments for better API documentation
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Add examples for requests
    // c.EnableAnnotations();
});

// Add CORS for frontend integration
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Add health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Demo Day AI Agent API v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
        c.DisplayRequestDuration();
        c.EnableTryItOutByDefault();
    });
}

app.UseHttpsRedirection();
app.UseCors();

// Map controllers
app.MapControllers();

// Add health check endpoint
app.MapHealthChecks("/health");

// Add a simple info endpoint
app.MapGet("/info", () => new
{
    application = "Demo Day AI Agent API",
    version = "1.0.0",
    environment = app.Environment.EnvironmentName,
    timestamp = DateTime.UtcNow,
    features = new[]
    {
        "Streaming AI responses",
        "Weather integration",
        "Markdown formatting",
        "Character-by-character streaming",
        "Azure OpenAI with Semantic Kernel",
        "OpenWeatherMap integration"
    }
})
.WithName("GetApplicationInfo")
.WithOpenApi()
.WithTags("Information");

app.Run();

// Make the Program class accessible for testing
public partial class Program { }
