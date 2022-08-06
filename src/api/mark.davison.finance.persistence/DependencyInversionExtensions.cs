namespace mark.davison.finance.persistence;

public static class DependencyInversionExtensions
{
    public static IServiceCollection UseFinancePersistence(this IServiceCollection services)
    {
        services.AddTransient(typeof(IEntityDefaulter<>), typeof(GenericFinanceEntityDefaulter<>));

        return services;
    }
}
