using mark.davison.common.CQRS;

namespace mark.davison.finance.bff;

public class Startup
{
    const string KeycloakRealmToWellKnown = "/.well-known/openid-configuration";

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var AppSettings = services.ConfigureSettingsServices(Configuration);
        if (AppSettings == null) { throw new InvalidOperationException(); }

        services
            .AddControllers();
        services.ConfigureHealthCheckServices();

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .SetIsOriginAllowed((host) => true)
                    .AllowAnyHeader());
        });

        services.AddAuthentication(ZenoAuthenticationConstants.ZenoAuthenticationScheme);
        services.AddAuthorization();
        services.AddTransient<ICustomZenoAuthenticationActions, FinanceCustomZenoAuthenticationActions>();
        services.AddHttpClient("PROXY");
        services.UseFinanceBff(AppSettings);

        services
            .AddSession(o =>
            {
                o.Cookie.SameSite = SameSiteMode.None;
                o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                o.Cookie.Name = "finance-session-name";
                o.Cookie.HttpOnly = true;
            });

        if (string.IsNullOrEmpty(AppSettings.REDIS_PASSWORD) ||
            string.IsNullOrEmpty(AppSettings.REDIS_HOST))
        {
            services
                .AddDistributedMemoryCache();
        }
        else
        {
            var config = new ConfigurationOptions
            {
                EndPoints = { AppSettings.REDIS_HOST + ":" + AppSettings.REDIS_PORT },
                Password = AppSettings.REDIS_PASSWORD
            };
            IConnectionMultiplexer redis = ConnectionMultiplexer.Connect(config);
            services.AddStackExchangeRedisCache(_ =>
            {
                _.InstanceName = "finance_" + (AppSettings.PRODUCTION_MODE ? "prod_" : "dev_");
                _.Configuration = redis.Configuration;
            });
            services.AddDataProtection().PersistKeysToStackExchangeRedis(redis, "DataProtectionKeys");
            services.AddSingleton(redis);
        }

        services.AddZenoAuthentication(_ =>
        {
            if (string.IsNullOrEmpty(AppSettings.AUTHORITY))
            {
                throw new InvalidOperationException();
            }
            _.Scope = AppSettings.SCOPE;
            _.WebOrigin = AppSettings.WEB_ORIGIN;
            _.BffOrigin = AppSettings.BFF_ORIGIN;
            _.ClientId = AppSettings.CLIENT_ID;
            _.ClientSecret = AppSettings.CLIENT_SECRET;
            _.OpenIdConnectWellKnownUri = AppSettings.AUTHORITY + KeycloakRealmToWellKnown;
        });

    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSession();

        app.UseMiddleware<RequestResponseLoggingMiddleware>();

        app.UseCors("CorsPolicy");

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware<HydrateAuthenticationFromSessionMiddleware>();

        app.UseEndpoints(endpoints =>
        {
            endpoints
                .MapHealthChecks();

            endpoints
                .UseAuthenticationEndpoints();

            MapProxyCQRSGet(endpoints, "/api/startup-query");
            MapProxyCQRSGet(endpoints, "/api/account-list-query");
            MapProxyCQRSGet(endpoints, "/api/category-list-query");
            MapProxyCQRSGet(endpoints, "/api/tag-list-query");
        });
    }

    static void MapProxyCQRSGet(IEndpointRouteBuilder endpoints, string path)
    {
        endpoints.MapGet(
            path,
            async (HttpContext context, IOptions<AppSettings> options, IHttpClientFactory httpClientFactory, ICurrentUserContext currentUserContext, CancellationToken cancellationToken) =>
            {
                if (string.IsNullOrEmpty(currentUserContext.Token))
                {
                    return Results.Unauthorized();
                }

                var client = httpClientFactory.CreateClient("PROXY");

                var headers = HeaderParameters.Auth(currentUserContext.Token, currentUserContext.CurrentUser);

                var request = new HttpRequestMessage(HttpMethod.Get, $"{options.Value.API_ORIGIN.TrimEnd('/')}{path}");

                foreach (var k in headers)
                {
                    request.Headers.Add(k.Key, k.Value);
                }

                var response = await client.SendAsync(request, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return Results.Text(content);
                }

                return Results.BadRequest(new Response
                {
                    Errors = new() { $"Error: {response.StatusCode}" }
                });
            });
    }

    public IConfiguration Configuration { get; }
    public AppSettings AppSettings { get; set; } = null!;
}
