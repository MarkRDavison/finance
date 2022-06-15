namespace mark.davison.finance.common.client.Authentication;
public class ApiClient
{
    private HttpClient _client;

    public ApiClient(HttpClient client)
    {
        _client = client;
    }

    public HttpClient Client => _client;
}
