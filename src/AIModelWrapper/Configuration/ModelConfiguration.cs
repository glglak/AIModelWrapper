// src/AIModelWrapper/Configuration/ModelConfiguration.cs
public record ModelConfiguration
{
    public string ApiKey { get; init; } = string.Empty;
    public string? BaseUrl { get; init; }
    public int TimeoutSeconds { get; init; } = 30;
    public int MaxRetries { get; init; } = 3;
    public Dictionary<string, string> DefaultHeaders { get; init; } = new();
}

// src/AIModelWrapper/Models/ModelRequest.cs
public class ModelRequest
{
    public string Prompt { get; set; } = string.Empty;
    public double Temperature { get; set; } = 0.7;
    public int MaxTokens { get; set; } = 2000;
    public Dictionary<string, object> ModelSpecificParams { get; set; } = new();
}

// src/AIModelWrapper/Models/ModelResponse.cs
public class ModelResponse
{
    public string Text { get; set; } = string.Empty;
    public string ModelName { get; set; } = string.Empty;
    public double TokensUsed { get; set; }
    public Dictionary<string, object> ModelSpecificData { get; set; } = new();
}
