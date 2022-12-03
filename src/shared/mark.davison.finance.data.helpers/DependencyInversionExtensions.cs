namespace mark.davison.finance.data.helpers;

public static class DependencyInversionExtensions
{
    public static void UseDataSeeders(this IServiceCollection services)
    {
        services.AddTransient<AccountSeeder>();
        services.AddTransient<TransactionSeeder>();
    }
}
