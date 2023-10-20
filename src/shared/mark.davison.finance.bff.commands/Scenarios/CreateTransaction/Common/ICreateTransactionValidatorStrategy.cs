namespace mark.davison.finance.bff.commands.Scenarios.CreateTransaction.Common;

public interface ICreateTransactionValidatorStrategy
{
    Task ValidateTransactionGroup(CreateTransactionRequest request, CreateTransactionResponse response, ICreateTransctionValidationContext validationContext);
    Task ValidateTranasction(CreateTransactionDto transaction, CreateTransactionResponse response, ICreateTransctionValidationContext validationContext);
}
