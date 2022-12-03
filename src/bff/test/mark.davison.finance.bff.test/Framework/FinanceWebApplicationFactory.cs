namespace mark.davison.finance.bff.test.Framework;

public class FinanceWebApplicationFactory : WebApplicationFactory<Startup>
{

    public FinanceWebApplicationFactory(Func<Action<AppSettings>> configureSettings)
    {
        ConfigureSettings = configureSettings;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(ConfigureServices);
        builder.ConfigureLogging((WebHostBuilderContext context, ILoggingBuilder loggingBuilder) =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddConsole();
        });
    }

    protected virtual void ConfigureServices(IServiceCollection services)
    {
        MessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage request, CancellationToken token) =>
            {
                HttpResponseMessage response = new HttpResponseMessage();
                if (request.RequestUri!.PathAndQuery.Contains("/auth/realms/markdavison.kiwi/.well-known/openid-configuration"))
                {
                    var content = File.ReadAllTextAsync("openid-configuration.json").Result;
                    response.Content = new StringContent(content);
                }
                else if (request.RequestUri.PathAndQuery.Contains("/auth/realms/markdavison.kiwi/protocol/openid-connect/token"))
                {
                    response.Content = new StringContent(JsonSerializer.Serialize(new AuthTokens
                    {
                        access_token = "access",
                        refresh_token = "refresh"
                    }));
                }
                else if (request.RequestUri.PathAndQuery.Contains("/auth/realms/markdavison.kiwi/protocol/openid-connect/userinfo"))
                {
                    response.Content = new StringContent(JsonSerializer.Serialize(UserProfile));
                }
                else if (request.RequestUri.PathAndQuery.Contains("/api/user") && request.Method == HttpMethod.Post)
                {
                    response.Content = new StringContent(JsonSerializer.Serialize(User));
                }
                else if (request.RequestUri.PathAndQuery.Contains("/api/user?sub="))
                {
                    var query = HttpUtility.ParseQueryString(request.RequestUri.Query);
                    if (Guid.TryParse(query["sub"], out Guid userSub) && userSub == User.Sub)
                    {
                        response.Content = new StringContent(JsonSerializer.Serialize(new User[] { User }));
                    }
                    else
                    {
                        Assert.Fail("Invalid user query: '{0}'", request.RequestUri.PathAndQuery);
                    }
                }
                else
                {
                    Assert.Fail("Unhandled request: '{0}'", request.RequestUri.PathAndQuery);
                }

                return response;
            });
        services.AddHttpClient(ZenoAuthenticationConstants.AuthClientName).ConfigureHttpMessageHandlerBuilder(_ =>
        {
            _.PrimaryHandler = MessageHandlerMock.Object;
        });
        services.Configure<AppSettings>(a =>
        {
            if (ConfigureSettings() != null)
            {
                ConfigureSettings()(a);
            }
        });
    }

    protected Func<Action<AppSettings>> ConfigureSettings { get; set; }
    protected Mock<HttpMessageHandler> MessageHandlerMock { get; set; } = new();

    protected virtual UserProfile UserProfile => new UserProfile
    {
        sub = new Guid("ACC47AFA-F89E-4FED-A346-B75BB5B01737"),
        given_name = "First",
        family_name = "Last",
        email = "first.last@markdavison.kiwi",
        name = "First Last",
        email_verified = true,
        preferred_username = "First"
    };

    protected virtual User User => new User
    {
        Id = new Guid("6282A750-13F8-4C1D-9F13-58EFFDD8BE20"),
        Sub = new Guid("ACC47AFA-F89E-4FED-A346-B75BB5B01737")
    };

}
