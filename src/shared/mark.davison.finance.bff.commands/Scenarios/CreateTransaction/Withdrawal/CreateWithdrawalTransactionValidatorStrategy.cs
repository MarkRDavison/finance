namespace mark.davison.finance.bff.commands.Scenarios.CreateTransaction.Withdrawal;

public class CreateWithdrawalTransactionValidatorStrategy : ICreateTransactionValidatorStrategy
{
    public Task ValidateTranasction(CreateTransactionDto transaction, CreateTransactionResponse response, ICreateTransctionValidationContext context)
    {
        throw new NotImplementedException();
    }

    public Task ValidateTransactionGroup(CreateTransactionRequest request, CreateTransactionResponse response, ICreateTransctionValidationContext context)
    {
        return Task.FromResult(context);
    }
}
