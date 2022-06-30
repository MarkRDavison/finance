using static mark.davison.finance.common.server.abstractions.Authentication.ZenoAuthenticationConstants;

namespace mark.davison.finance.common.server.Middleware;

public class ProxyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ProxyOptions _options;

    public ProxyMiddleware(
        RequestDelegate next,
        IHttpClientFactory httpClientFactory,
        ProxyOptions options
    )
    {
        _next = next;
        _httpClientFactory = httpClientFactory;
        _options = options;
    }
    public async Task Invoke(HttpContext context)
    {
        // TODO: This token may have expired
        var accessToken = context.Session.Get(SessionNames.AccessToken);

        using var httpClient = _httpClientFactory.CreateClient(_options.RouteBase);
        var message = new HttpRequestMessage
        {
            Method = new HttpMethod(context.Request.Method),
            RequestUri = new Uri(context.Request.Path, UriKind.Relative),
            Headers =
            {
                { HttpRequestHeader.Authorization.ToString(), "Bearer " + accessToken },
            }
        };

        try
        {
            using (var responseMessage = await httpClient.SendAsync(message, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted))
            {
                context.Response.StatusCode = (int)responseMessage.StatusCode;

                CopyFromTargetResponseHeaders(context, responseMessage);

                await ProcessResponseContent(context, responseMessage);

                return;
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    private void CopyFromTargetResponseHeaders(HttpContext context, HttpResponseMessage responseMessage)
    {
        foreach (var header in responseMessage.Headers)
        {
            context.Response.Headers[header.Key] = header.Value.ToArray();
        }
        //set each one of these headers
        foreach (var header in responseMessage.Content.Headers)
        {
            context.Response.Headers[header.Key] = header.Value.ToArray();
        }
        context.Response.Headers.Remove("transfer-encoding");
    }
    private async Task ProcessResponseContent(HttpContext context, HttpResponseMessage responseMessage)
    {
        var content = await responseMessage.Content.ReadAsByteArrayAsync();

        if (IsContentOfType(responseMessage, "text/html") || IsContentOfType(responseMessage, "text/javascript"))
        {
            var stringContent = Encoding.UTF8.GetString(content);
            await context.Response.WriteAsync(stringContent, Encoding.UTF8);
        }
        else
        {
            await context.Response.Body.WriteAsync(content);
        }
    }

    private bool IsContentOfType(HttpResponseMessage responseMessage, string type)
    {
        var result = false;

        if (responseMessage.Content?.Headers?.ContentType != null)
        {
            result = responseMessage.Content.Headers.ContentType.MediaType == type;
        }

        return result;
    }
}
