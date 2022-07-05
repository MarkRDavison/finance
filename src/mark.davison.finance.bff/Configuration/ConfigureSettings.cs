namespace mark.davison.finance.bff.Configuration;

public static class ConfigureSettingsExtensions
{
    public static AppSettings ConfigureSettingsServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var configured = configuration.GetSection(AppSettings.SECTION);

        services.Configure<AppSettings>(configured);
        var appSettings = new AppSettings();
        configured.Bind(appSettings);

        return appSettings;
    }
}
