using System.Text;
using System.Text.Json;

namespace mark.davison.finance.bff.test.Framework;

public class BffIntegrationTestBase
{
    public BffIntegrationTestBase()
    {
        Factory = new FinanceWebApplicationFactory(() => ConfigureSettings);
        Client = Factory.CreateClient();
    }

    public void Dispose()
    {
        Client?.Dispose();
        Factory?.Dispose();
    }

    protected Task<HttpResponseMessage> PostAsync(string uri, object data)
    {
        return CallAsync(HttpMethod.Post, uri, data);
    }

    protected Task<HttpResponseMessage> DeleteAsync(string uri)
    {
        return CallAsync(HttpMethod.Delete, uri, null);
    }

    protected async Task<HttpResponseMessage> CallAsync(HttpMethod httpMethod, string uri, object? data)
    {
        var message = new HttpRequestMessage
        {
            Method = httpMethod,
            RequestUri = new Uri(uri, UriKind.Relative),
            Headers =
            {
                //{ HttpRequestHeader.Authorization.ToString(), $"Bearer { MockJwtTokens.GenerateJwtToken(claims ?? DefaultClaims) }" },
                //{ AuthConstants.Token.Sub, Sub.ToString() }
            },
            Content = data == null ? null : new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json")
        };

        return await Client.SendAsync(message);

    }

    protected async Task<T> ReadAsAsync<T>(HttpResponseMessage response)
    {
        string res = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(res)!;
    }

    protected async Task<T> PostAsAsync<T>(string uri, T data, bool requireSuccess = false)
    {
        var response = await PostAsync(uri, data!);
        if (requireSuccess)
        {
            response.EnsureSuccessStatusCode();
        }
        return await ReadAsAsync<T>(response);
    }

    protected async Task<HttpResponseMessage> GetAsync(string uri, bool requireSuccess = false)
    {
        var response = await CallAsync(HttpMethod.Get, uri, null);
        if (requireSuccess)
        {
            response.EnsureSuccessStatusCode();
        }
        return response;
    }

    protected async Task<T> GetAsync<T>(string uri, bool requireSuccess = false)
    {
        var response = await CallAsync(HttpMethod.Get, uri, null);
        if (requireSuccess)
        {
            response.EnsureSuccessStatusCode();
        }
        return await ReadAsAsync<T>(response);
    }

    protected async Task<bool> DeleteAsync<T>(string uri, bool requireSuccess = false)
    {
        var response = await CallAsync(HttpMethod.Delete, uri, null);
        if (requireSuccess)
        {
            response.EnsureSuccessStatusCode();
        }
        return response.IsSuccessStatusCode;
    }

    protected async Task<T> PatchAsync<T>(string uri, T data, bool requireSuccess = false)
    {
        var response = await CallAsync(HttpMethod.Patch, uri, data);
        if (requireSuccess)
        {
            response.EnsureSuccessStatusCode();
        }
        return await ReadAsAsync<T>(response);
    }

    protected FinanceWebApplicationFactory Factory { get; }
    protected HttpClient Client { get; }

    protected Action<AppSettings> ConfigureSettings { get; set; } = a => { };
}
