namespace mark.davison.finance.shared.commands;

[ExcludeFromCodeCoverage]
public static class DependencyInversionExtensions
{
    public static IServiceCollection AddCommandCQRS(this IServiceCollection services)
    {
        services.AddTransient<ICreateTransactionValidatorStrategyFactory, CreateTransactionValidatorStrategyFactory>();
        services.AddTransient<ICreateTransctionValidationContext, CreateTransctionValidationContext>();

        return services;
    }
}

