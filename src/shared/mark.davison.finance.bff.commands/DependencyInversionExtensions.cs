namespace mark.davison.finance.bff.commands;

[ExcludeFromCodeCoverage]
public static class DependencyInversionExtensions
{
    public static void AddCommandCQRS(this IServiceCollection services)
    {
        // TODO: Make validators like TResponse Validate<TRequest, TResponse>(TRequest request) etc
        //       and auto register them
        services.AddTransient<IUpsertAccountCommandValidator, UpsertAccountCommandValidator>();
        services.AddTransient<IUpsertAccountCommandProcessor, UpsertAccountCommandProcessor>();
        services.AddTransient<ICreateCategoryCommandValidator, CreateCategoryCommandValidator>();
        services.AddTransient<ICreateTransactionCommandValidator, CreateTransactionCommandValidator>();
        services.AddTransient<ICreateTransactionCommandProcessor, CreateTransactionCommandProcessor>();
        services.AddTransient<ICreateTransactionValidatorStrategyFactory, CreateTransactionValidatorStrategyFactory>();
        services.AddTransient<ICreateTransctionValidationContext, CreateTransctionValidationContext>();

    }
}

