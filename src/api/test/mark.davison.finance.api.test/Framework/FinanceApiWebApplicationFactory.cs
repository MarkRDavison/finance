namespace mark.davison.finance.api.test.Framework;

public class TestFinanceHttpRepository : HttpRepository
{
    public TestFinanceHttpRepository(string baseUri, JsonSerializerOptions options) : base(baseUri, new HttpClient(), options)
    {

    }
    public TestFinanceHttpRepository(string baseUri, HttpClient client, JsonSerializerOptions options) : base(baseUri, client, options)
    {

    }
}

public class FinanceApiWebApplicationFactory : WebApplicationFactory<Startup>, ICommonWebApplicationFactory<AppSettings>
{
    public IServiceProvider ServiceProvider => base.Services;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, conf) => conf
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.integration.json"));
        builder.ConfigureTestServices(ConfigureServices);
        builder.ConfigureLogging((WebHostBuilderContext context, ILoggingBuilder loggingBuilder) =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddConsole();
        });
    }

    protected virtual void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<IFinanceDataSeeder, FinanceDataSeeder>(_ => // TODO: Remove lambda???
            new FinanceDataSeeder(
                _.GetRequiredService<IServiceScopeFactory>(),
                _.GetRequiredService<IOptions<AppSettings>>()
            ));
        services.UseDataSeeders();

        services
            .AddHttpClient()
            .AddHttpContextAccessor();

        services.UseCQRSServer();
        services.AddCommandCQRS();
        services.AddScoped<ICurrentUserContext, CurrentUserContext>(_ =>
        {
            var context = new CurrentUserContext();
            if (ModifyCurrentUserContext != null) { ModifyCurrentUserContext(_, context); }
            return context;
        });
    }

    public Action<IServiceProvider, CurrentUserContext>? ModifyCurrentUserContext { get; set; }
}

