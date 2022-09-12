using mark.davison.finance.models.dtos.Commands.CreateTransaction;

namespace mark.davison.finance.bff.commands.Scenarios.CreateTransaction.Validators;

public interface ICreateTransactionCommandValidator
{
    Task<CreateTransactionResponse> Validate(CreateTransactionRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellation);
}
