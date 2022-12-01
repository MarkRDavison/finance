using mark.davison.finance.models.dtos.Commands.CreateTransaction;

namespace mark.davison.finance.bff.commands.Scenarios.CreateTransaction;

public class NullCreateTransactionValidatorStrategy : ICreateTransactionValidatorStrategy
{
    public Task ValidateTranasction(CreateTransactionDto transaction, CreateTransactionCommandResponse response, ICreateTransctionValidationContext context)
    {
        return Task.FromResult(context);
    }

    public Task ValidateTransactionGroup(CreateTransactionCommandRequest request, CreateTransactionCommandResponse response, ICreateTransctionValidationContext context)
    {
        return Task.FromResult(context);
    }
}
