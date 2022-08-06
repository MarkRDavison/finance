namespace mark.davison.finance.bff.commands.Scenarios.CreateTransaction;

public interface ICreateTransactionValidatorStrategy
{
    Task ValidateTransactionGroup(CreateTransactionRequest request, CreateTransactionResponse response, ICreateTransctionValidationContext context);
    Task ValidateTranasction(CreateTransactionDto transaction, CreateTransactionResponse response, ICreateTransctionValidationContext context);
}
