namespace mark.davison.finance.web.features.Transaction.Create;

public class TransactionCreateCommandHandler : ICommandHandler<TransactionCreateCommand, TransactionCreateCommandResponse>
{
    private readonly IClientHttpRepository _repository;

    public TransactionCreateCommandHandler(
        IClientHttpRepository repository)
    {
        _repository = repository;
    }
    public async Task<TransactionCreateCommandResponse> Handle(TransactionCreateCommand command, CancellationToken cancellation)
    {
        var request = new CreateTransactionRequest
        {
            Description = command.Description,
            TransactionTypeId = command.TransactionTypeId,
            Transactions = command.CreateTransactionDtos
        };

        var response = await _repository.Post<CreateTransactionResponse, CreateTransactionRequest>(request, cancellation);

        return new TransactionCreateCommandResponse
        {
            Success = response.Success
        };
    }
}
