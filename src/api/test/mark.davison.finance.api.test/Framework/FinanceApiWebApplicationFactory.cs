namespace mark.davison.finance.api.test.Framework;

public class FinanceApiWebApplicationFactory : WebApplicationFactory<Startup>, ICommonWebApplicationFactory<AppSettings>
{
    public virtual Func<IRepository, Task> SeedDataFunc { get; set; } = _ => Task.CompletedTask;

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
        services.AddTransient<IFinanceDataSeeder, TestFinanceDataSeeder>(_ =>
            new TestFinanceDataSeeder(
                _.GetRequiredService<IRepository>(),
                _.GetRequiredService<IOptions<AppSettings>>()
            )
            {
                SeedData = SeedDataFunc
            });
    }
}

