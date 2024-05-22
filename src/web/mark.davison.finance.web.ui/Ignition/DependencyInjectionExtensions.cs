namespace mark.davison.finance.web.ui.Ignition;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseFinanceWeb(this IServiceCollection services, IAuthenticationConfig authConfig)
    {
        services
            .UseFinanceComponents(authConfig)
            .UseFluxorState(_ => { }, typeof(Program), typeof(FeaturesRootType))
            .UseClientRepository(WebConstants.ApiClientName, WebConstants.LocalBffRoot);

        return services;
    }
}
