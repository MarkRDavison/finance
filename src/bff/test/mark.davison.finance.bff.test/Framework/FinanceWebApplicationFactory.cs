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
        services.Configure<AppSettings>(a =>
        {
            if (ConfigureSettings() != null)
            {
                ConfigureSettings()(a);
            }
        });
    }

    protected Func<Action<AppSettings>> ConfigureSettings { get; set; }


    protected virtual User User => new User
    {
        Id = new Guid("6282A750-13F8-4C1D-9F13-58EFFDD8BE20"),
        Sub = new Guid("ACC47AFA-F89E-4FED-A346-B75BB5B01737")
    };

}
