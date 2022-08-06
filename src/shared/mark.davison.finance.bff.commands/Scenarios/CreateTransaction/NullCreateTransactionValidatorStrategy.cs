namespace mark.davison.finance.bff.commands.Scenarios.CreateTransaction;

public class NullCreateTransactionValidatorStrategy : ICreateTransactionValidatorStrategy
{
    public Task ValidateTranasction(CreateTransactionDto transaction, CreateTransactionResponse response, ICreateTransctionValidationContext context)
    {
        return Task.FromResult(context);
    }

    public Task ValidateTransactionGroup(CreateTransactionRequest request, CreateTransactionResponse response, ICreateTransctionValidationContext context)
    {
        return Task.FromResult(context);
    }
}
