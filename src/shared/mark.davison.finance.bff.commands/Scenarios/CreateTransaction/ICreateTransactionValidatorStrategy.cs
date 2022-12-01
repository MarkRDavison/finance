using mark.davison.finance.models.dtos.Commands.CreateTransaction;

namespace mark.davison.finance.bff.commands.Scenarios.CreateTransaction;

public interface ICreateTransactionValidatorStrategy
{
    Task ValidateTransactionGroup(CreateTransactionCommandRequest request, CreateTransactionCommandResponse response, ICreateTransctionValidationContext context);
    Task ValidateTranasction(CreateTransactionDto transaction, CreateTransactionCommandResponse response, ICreateTransctionValidationContext context);
}
