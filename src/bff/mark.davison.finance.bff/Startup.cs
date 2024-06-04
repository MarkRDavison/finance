namespace mark.davison.finance.bff;

public class Startup
{
    public IConfiguration Configuration { get; }
    public AppSettings AppSettings { get; set; } = default!;

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        AppSettings = services.ConfigureSettingsServices<AppSettings>(Configuration);
        if (AppSettings == null) { throw new InvalidOperationException(); }

        Console.WriteLine(AppSettings.DumpAppSettings(AppSettings.PRODUCTION_MODE));

        services
            .AddCors()
            .UseCookieOidcAuth(
                AppSettings.AUTH,
                AppSettings.CLAIMS,
                _ =>
                {
                    if (!AppSettings.PRODUCTION_MODE)
                    {
                        _.OnUserCreated = async (IServiceProvider services, User user, string access_token) =>
                        {
                            var client = services.GetRequiredService<IHttpClientFactory>().CreateClient("INIT");

                            client.BaseAddress = new Uri(AppSettings.API_ORIGIN);
                            client.DefaultRequestHeaders.Add(HeaderNames.Authorization, $"Bearer {access_token}");

                            await client.PostAsync("/api/seed", null);
                        };
                    }
                },
                AppSettings.API_ORIGIN)
            .AddHealthCheckServices()
            .AddAuthorization()
            .AddEndpointsApiExplorer()
            .AddSwaggerGen();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseCors(builder =>
            builder
                .SetIsOriginAllowedToAllowWildcardSubdomains()
                .SetIsOriginAllowed(_ => true) // TODO: Config driven
                .AllowAnyMethod()
                .AllowCredentials()
                .AllowAnyHeader());

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app
            .UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor
            })
            .UseMiddleware<RequestResponseLoggingMiddleware>()
            .UseRouting()
            .UseAuthentication()
            .UseMiddleware<CheckAccessTokenValidityMiddleware>()
            .UseAuthorization()
            .UseEndpoints(endpoints =>
            {
                endpoints
                    .UseApiProxy(AppSettings.API_ORIGIN)
                    .UseAuthEndpoints(AppSettings.WEB_ORIGIN)
                    .MapHealthChecks();
            });
    }
}
