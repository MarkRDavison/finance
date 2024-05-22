namespace mark.davison.finance.api;

[UseCQRSServer(typeof(DtosRootType), typeof(CommandsRootType), typeof(QueriesRootType))]
public class Startup
{
    public IConfiguration Configuration { get; }

    public AppSettings AppSettings { get; set; } = null!;

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
            .AddLogging()
            .AddJwtAuth(AppSettings.AUTH)
            .AddAuthorization()
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()
            .AddHttpContextAccessor()
            .AddScoped<ICurrentUserContext, CurrentUserContext>()
            .AddHealthCheckServices<InitializationHostedService>()
            .AddSingleton<IDateService>(new DateService(DateService.DateMode.Utc))
            .AddDatabase<FinanceDbContext>(AppSettings.PRODUCTION_MODE, AppSettings.DATABASE, typeof(PostgresContextFactory), typeof(SqliteContextFactory))
            .AddCoreDbContext<FinanceDbContext>()
            .AddScoped<IFinanceDbContext>(_ => _.GetRequiredService<FinanceDbContext>())
            .AddCQRSServer()
            .AddRedis(AppSettings.REDIS, AppSettings.SECTION, AppSettings.PRODUCTION_MODE)
            .UseFinancePersistence()
            .UseUserApplicationContext()
            .AddCommandCQRS(); // TODO: Remove when IValidateAndProcess implemented

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

        app.UseHttpsRedirection();

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app
            .UseMiddleware<RequestResponseLoggingMiddleware>()
            .UseRouting()
            .UseAuthentication()
            .UseAuthorization()
            .UseMiddleware<PopulateUserContextMiddleware>();

        if (!AppSettings.PRODUCTION_MODE)
        {
            app.UseMiddleware<ValidateUserExistsInDbMiddleware>();
        }

        app
            .UseEndpoints(_ =>
            {
                _
                    .MapHealthChecks()
                    .MapGet<User>()
                    .MapGetById<User>()
                    .MapPost<User>()
                    .MapCQRSEndpoints();
            });

    }

}
