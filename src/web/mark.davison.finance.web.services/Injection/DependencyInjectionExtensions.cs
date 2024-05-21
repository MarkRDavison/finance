namespace mark.davison.finance.web.services.Injection;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseFinanceWebServices(this IServiceCollection services)
    {
        services.AddSingleton<IAppContextService, AppContextService>();
        services.AddSingleton<IDateService>(new DateService(DateService.DateMode.Local));

        return services;
    }
}
