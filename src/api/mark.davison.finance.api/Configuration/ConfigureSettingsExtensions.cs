﻿namespace mark.davison.finance.api.Configuration;

public static class ConfigureSettingsExtensions
{
    public static AppSettings ConfigureSettingsServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var appSettings = new AppSettings();
        var configured = configuration.GetSection(AppSettings.SECTION);

        services.Configure<AppSettings>(configured);
        configured.Bind(appSettings);

        return appSettings;
    }
}
