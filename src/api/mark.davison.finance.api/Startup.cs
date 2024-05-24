using mark.davison.finance.data.helpers.Seeders;

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
            .UseFinancePersistence() // TODO: Include optional defaulters in IFinanceDbContext wrapper????
            .UseUserApplicationContext()
            .AddCommandCQRS()
            .UseDataSeeders();
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
            .UseMiddleware<PopulateUserContextMiddleware>()
            .UseMiddleware<ValidateUserExistsInDbMiddleware>()
            .UseEndpoints(endpoints =>
            {
                endpoints
                    .MapHealthChecks()
                    .MapGet<User>()
                    .MapGetById<User>()
                    .MapPost<User>()
                    .MapCQRSEndpoints();

                if (!AppSettings.PRODUCTION_MODE)
                {
                    endpoints
                        .MapPost("/api/seed", async (HttpContext context) =>
                        {
                            var accountSeeder = context.RequestServices.GetRequiredService<AccountSeeder>();

                            await accountSeeder.CreateStandardAccounts();

                            return Results.Ok();
                        });

                    endpoints
                        .MapPost("/api/reset", async (HttpContext context, CancellationToken cancellationToken) =>
                        {
                            var dbContext = context.RequestServices.GetRequiredService<IFinanceDbContext>();

                            var users = await dbContext
                                .Set<User>()
                                .Where(_ => _.Id != Guid.Empty)
                                .ToListAsync(cancellationToken);
                            await dbContext.DeleteEntitiesAsync(users, cancellationToken);
                            await dbContext.SaveChangesAsync(cancellationToken);

                            return Results.Ok();
                        }).AllowAnonymous();
                }
            });

    }

}
