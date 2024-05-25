namespace mark.davison.finance.shared.utilities.Ignition;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddSharedServices(this IServiceCollection services)
    {
        services.AddScoped<IFinanceUserContext, FinanceUserContext>();

        return services;
    }
}
