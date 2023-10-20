namespace mark.davison.finance.bff.commands.Scenarios.CreateTransaction.Common;

public class CreateTransactionCommandHandler : ValidateAndProcessCommandHandler<CreateTransactionRequest, CreateTransactionResponse>
{
    public CreateTransactionCommandHandler(
        ICommandProcessor<CreateTransactionRequest, CreateTransactionResponse> processor,
        ICommandValidator<CreateTransactionRequest, CreateTransactionResponse> validator
    ) : base(
        processor,
        validator
    )
    {
    }
}
