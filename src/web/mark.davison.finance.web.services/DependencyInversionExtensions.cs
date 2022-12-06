namespace mark.davison.finance.web.services;

public static class DependencyInversionExtensions
{
    public static void UseFinanceWebServices(this IServiceCollection services)
    {
        services.AddSingleton<IAppContextService, AppContextService>();
        services.AddSingleton<IDateService>(new DateService(DateService.DateMode.Local));
    }
}
