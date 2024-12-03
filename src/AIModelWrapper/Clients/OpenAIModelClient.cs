// src/AIModelWrapper/Clients/OpenAIModelClient.cs
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Polly;

public class OpenAIModelClient : IAIModelClient
{
    private readonly HttpClient _httpClient;
    private readonly IAsyncPolicy<ModelResponse> _resiliencePolicy;
    private readonly IErrorMapper _errorMapper;
    private readonly ModelConfiguration _config;

    public OpenAIModelClient(
        HttpClient httpClient,
        IAsyncPolicy<ModelResponse> resiliencePolicy,
        IErrorMapper errorMapper,
        ModelConfiguration config)
    {
        _httpClient = httpClient;
        _resiliencePolicy = resiliencePolicy;
        _errorMapper = errorMapper;
        _config = config;

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", config.ApiKey);
    }

    public async Task<ModelResponse> CompleteAsync(
        ModelRequest request,
        CancellationToken cancellationToken = default)
    {
        return await _resiliencePolicy.ExecuteAsync(async () =>
        {
            var openAIRequest = new
            {
                prompt = request.Prompt,
                max_tokens = request.MaxTokens,
                temperature = request.Temperature,
                model = "gpt-4"  // or get from config
            };

            var response = await _httpClient.PostAsJsonAsync(
                "completions",
                openAIRequest,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                throw _errorMapper.MapError(response, content);
            }

            var result = await response.Content.ReadFromJsonAsync<OpenAIResponse>(
                cancellationToken: cancellationToken);

            return new ModelResponse
            {
                Text = result?.Choices?.FirstOrDefault()?.Text ?? string.Empty,
                ModelName = result?.Model ?? string.Empty,
                TokensUsed = result?.Usage?.TotalTokens ?? 0,
                ModelSpecificData = new Dictionary<string, object>
                {
                    ["finish_reason"] = result?.Choices?.FirstOrDefault()?.FinishReason ?? string.Empty
                }
            };
        });
    }

    public async Task<Stream> CompleteStreamAsync(
        ModelRequest request,
        CancellationToken cancellationToken = default)
    {
        // Implementation for streaming
        throw new NotImplementedException();
    }
}

// OpenAI response models
public class OpenAIResponse
{
    [JsonPropertyName("choices")]
    public List<Choice>? Choices { get; set; }

    [JsonPropertyName("model")]
    public string? Model { get; set; }

    [JsonPropertyName("usage")]
    public Usage? Usage { get; set; }
}

public class Choice
{
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; set; }
}

public class Usage
{
    [JsonPropertyName("total_tokens")]
    public int TotalTokens { get; set; }
}
