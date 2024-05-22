namespace mark.davison.finance.shared.commands;

[ExcludeFromCodeCoverage]
public static class DependencyInversionExtensions
{
    public static void AddCommandCQRS(this IServiceCollection services)
    {
        services.AddTransient<ICreateTransactionValidatorStrategyFactory, CreateTransactionValidatorStrategyFactory>();
        services.AddTransient<ICreateTransctionValidationContext, CreateTransctionValidationContext>();
    }
}

