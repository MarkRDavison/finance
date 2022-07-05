namespace mark.davison.finance.bff.commands;

public static class DependencyInversionExtensions
{
    public static void AddCommandCQRS(this IServiceCollection services)
    {
        // TODO: Make validators like TResponse Validate<TRequest, TResponse>(TRequest request) etc
        services.AddTransient<ICreateAccountCommandValidator, CreateAccountCommandValidator>();
    }
}

