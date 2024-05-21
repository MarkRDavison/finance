namespace mark.davison.finance.bff.Ignition;

public static class DependencyInjectionExtensions
{

    public static IServiceCollection UseFinanceBff(this IServiceCollection services, AppSettings appSettings)
    {
        return services.UseFinanceBff(appSettings, null);
    }

    public static IServiceCollection UseFinanceBff(this IServiceCollection services, AppSettings appSettings, Func<HttpClient>? client)
    {
        services.AddScoped<ICurrentUserContext, CurrentUserContext>();
        services.AddSingleton<IDateService>(new DateService(DateService.DateMode.Utc));

        //services.AddSingleton<IHttpRepository>(_ =>
        //{
        //    var options = SerializationHelpers.CreateStandardSerializationOptions();
        //    if (client == null)
        //    {
        //        return new FinanceHttpRepository(appSettings.API_ORIGIN, options);
        //    }
        //    return new FinanceHttpRepository(appSettings.API_ORIGIN, client(), options);
        //});

        services
            .AddHttpClient()
            .AddHttpContextAccessor();

        return services;
    }

}
