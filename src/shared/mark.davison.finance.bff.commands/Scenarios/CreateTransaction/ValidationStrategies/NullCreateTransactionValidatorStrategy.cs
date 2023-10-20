namespace mark.davison.finance.bff.commands.Scenarios.CreateTransaction.ValidationStrategies;

public class NullCreateTransactionValidatorStrategy : ICreateTransactionValidatorStrategy
{
    public Task ValidateTranasction(CreateTransactionDto transaction, CreateTransactionResponse response, ICreateTransctionValidationContext validationContext)
    {
        return Task.CompletedTask;
    }

    public Task ValidateTransactionGroup(CreateTransactionRequest request, CreateTransactionResponse response, ICreateTransctionValidationContext validationContext)
    {
        return Task.CompletedTask;
    }
}

