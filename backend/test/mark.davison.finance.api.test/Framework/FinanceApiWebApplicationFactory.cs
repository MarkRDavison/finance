using System;

namespace mark.davison.finance.api.test.Framework;

public class FinanceApiWebApplicationFactory : WebApplicationFactory<Startup>, IFinanceWebApplicationFactory
{
    public virtual Func<IRepository, Task> SeedDataFunc { get; set; } = _ => Task.CompletedTask;

    public FinanceApiWebApplicationFactory()
    {
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
        services.AddTransient<IFinanceDataSeeder, TestFinanceDataSeeder>(_ =>
            new TestFinanceDataSeeder(
                _.GetRequiredService<IRepository>()
            )
            {
                SeedData = SeedDataFunc
            });
    }
}

