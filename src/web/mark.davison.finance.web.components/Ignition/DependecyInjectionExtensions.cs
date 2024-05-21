namespace mark.davison.finance.web.components.Ignition;

public static class DependecyInjectionExtensions
{
    public static IServiceCollection UseFinanceComponents(
        this IServiceCollection services,
        IAuthenticationConfig authConfig)
    {
        services.UseCommonClient(authConfig, typeof(Routes));

        return services;
    }
}
