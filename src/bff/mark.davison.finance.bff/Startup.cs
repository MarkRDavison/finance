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
            .AddControllers()
            .ConfigureApplicationPartManager(manager =>
            {
                manager.ApplicationParts.Add(new AssemblyPart(typeof(AuthController).Assembly));
            });
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
                .MapControllers();
            endpoints
                .ConfigureCQRSEndpoints(
                    typeof(BffRootType),
                    typeof(CommandsRootType),
                    typeof(QueriesRootType),
                    typeof(DtosRootType));
        });
    }

    public IConfiguration Configuration { get; }
    public AppSettings AppSettings { get; set; } = null!;
}
