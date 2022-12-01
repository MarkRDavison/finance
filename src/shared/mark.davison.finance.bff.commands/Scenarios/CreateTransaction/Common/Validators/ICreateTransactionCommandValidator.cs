namespace mark.davison.finance.bff.commands.Scenarios.CreateTransaction.Validators;

public interface ICreateTransactionCommandValidator
{
    Task<CreateTransactionCommandResponse> Validate(CreateTransactionCommandRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken);
}
