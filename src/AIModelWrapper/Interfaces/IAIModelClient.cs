// src/AIModelWrapper/Interfaces/IAIModelClient.cs
public interface IAIModelClient
{
    Task<ModelResponse> CompleteAsync(ModelRequest request, CancellationToken cancellationToken = default);
    Task<Stream> CompleteStreamAsync(ModelRequest request, CancellationToken cancellationToken = default);
}

// src/AIModelWrapper/Interfaces/IErrorMapper.cs
public interface IErrorMapper
{
    AIModelException MapError(HttpResponseMessage response, string content);
}
