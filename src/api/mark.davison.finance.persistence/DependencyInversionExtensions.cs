namespace mark.davison.finance.persistence;

public static class DependencyInversionExtensions
{
    public static IServiceCollection UseFinancePersistence(this IServiceCollection services)
    {
        services.AddTransient(typeof(IEntityDefaulter<>), typeof(GenericFinanceEntityDefaulter<>));
        services.AddTransient<IEntityDefaulter<User>, UserDefaulter>();

        return services;
    }
}
