namespace mark.davison.finance.bff.commands.Scenarios.CreateTransaction.Transfer;

public class CreateTransferTransactionValidatorStrategy : ICreateTransactionValidatorStrategy
{
    public Task ValidateTranasction(CreateTransactionDto transaction, CreateTransactionResponse response, ICreateTransctionValidationContext context)
    {
        throw new NotImplementedException();
    }

    public Task ValidateTransactionGroup(CreateTransactionRequest request, CreateTransactionResponse response, ICreateTransctionValidationContext context)
    {
        throw new NotImplementedException();
    }
}
