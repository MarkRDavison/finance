namespace mark.davison.finance.common.client.Repository;

public class ClientHttpRepository : IClientHttpRepository
{
    private readonly string _remoteEndpoint;
    private HttpClient _httpClient;

    public ClientHttpRepository(string remoteEndpoint, IHttpClientFactory _httpClientFactory)
    {
        _remoteEndpoint = remoteEndpoint;
        _httpClient = _httpClientFactory.CreateClient("API");
        _httpClient.BaseAddress = new Uri(_remoteEndpoint);
    }

    public async Task<TResponse> Get<TResponse, TRequest>(TRequest request)
        where TRequest : class, ICommand<TRequest, TResponse>, new()
        where TResponse : class, new()
    {
        // TODO: Source Generator-ify this
        var attribute = typeof(TRequest).CustomAttributes.FirstOrDefault(_ => _.AttributeType == typeof(GetRequestAttribute));
        if (attribute == null) { throw new InvalidOperationException("Cannot perform Get against request without GetRequestAttribute"); }

        var path = attribute.NamedArguments.First(_ => _.MemberName == nameof(GetRequestAttribute.Path));
        var pathValue = path.TypedValue.Value as string;


        var requestMessage = new HttpRequestMessage(
            HttpMethod.Get,
            $"/api/{pathValue}");
        using var response = await _httpClient.SendAsync(requestMessage);
        var body = await response.Content.ReadAsStringAsync();
        var obj = JsonSerializer.Deserialize<TResponse>(body, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        return obj ?? new TResponse();
    }

    public async Task<TResponse> Post<TResponse, TRequest>(TRequest request)
        where TRequest : class, ICommand<TRequest, TResponse>, new()
        where TResponse : class, new()
    {
        // TODO: Source Generator-ify this
        var attribute = typeof(TRequest).CustomAttributes.FirstOrDefault(_ => _.AttributeType == typeof(PostRequestAttribute));
        if (attribute == null) { throw new InvalidOperationException("Cannot perform Post against request without PostRequestAttribute"); }

        var path = attribute.NamedArguments.First(_ => _.MemberName == nameof(PostRequestAttribute.Path));
        var pathValue = path.TypedValue.Value as string;


        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var requestMessage = new HttpRequestMessage(
            HttpMethod.Post,
            $"/api/{pathValue}")
        {
            Content = content
        };
        using var response = await _httpClient.SendAsync(requestMessage);
        var body = await response.Content.ReadAsStringAsync();
        var obj = JsonSerializer.Deserialize<TResponse>(body, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        return obj ?? new TResponse();
    }
}
