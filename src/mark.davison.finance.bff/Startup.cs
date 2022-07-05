﻿using mark.davison.finance.common.server.abstractions.Authentication;
using mark.davison.finance.common.server.abstractions.Repository;
using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;

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

        ProxyOptions = new()
        {
            RouteBase = "/api",
            ProxyAddress = AppSettings.API_ORIGIN
        };

        services
            .AddControllers()
            .PartManager.ApplicationParts.Add(new AssemblyPart(typeof(AuthController).Assembly));

        services.ConfigureHealthCheckServices();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

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
        services.AddScoped<ICurrentUserContext>(_ => new CurrentUserContext());
        services.AddSingleton<IHttpRepository>(_ => new FinanceHttpRepository(AppSettings.API_ORIGIN));

        services
            .AddSession(o =>
            {
                o.Cookie.SameSite = SameSiteMode.None;
                o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                o.Cookie.Name = "finance-session-name";
                o.Cookie.HttpOnly = true;
            });


        services
            .AddDistributedMemoryCache();

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

        services
            .AddHttpClient()
            .AddHttpContextAccessor();

        services.AddProxyServices(ProxyOptions);
        services.UseCQRS(
            typeof(BffRootType),
            typeof(CommandsRootType),
            typeof(QueriesRootType),
            typeof(DtosRootType));
        services.AddCommandCQRS();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
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

    ProxyOptions ProxyOptions { get; set; } = null!;
}