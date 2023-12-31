﻿using mark.davison.finance.bff.commands.Scenarios.CreateCategory;

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
        services.AddTransient<ICreateTagCommandValidator, CreateTagCommandValidator>();
        services.AddTransient<ICommandValidator<CreateTransactionRequest, CreateTransactionResponse>, CreateTransactionCommandValidator>();
        services.AddTransient<ICommandProcessor<CreateTransactionRequest, CreateTransactionResponse>, CreateTransactionCommandProcessor>();
        services.AddTransient<ICreateTransactionValidatorStrategyFactory, CreateTransactionValidatorStrategyFactory>();
        services.AddTransient<ICreateTransctionValidationContext, CreateTransctionValidationContext>();


        services.AddTransient<ICommandValidator<CreateCategoryCommandRequest, CreateCategoryCommandResponse>, CreateCategoryCommandValidator>();
        services.AddTransient<ICommandProcessor<CreateCategoryCommandRequest, CreateCategoryCommandResponse>, CreateCategoryCommandProcessor>();

    }
}

