namespace mark.davison.finance.web.features.Transaction.Create;

public class TransactionCreateCommandHandler : ICommandHandler<TransactionCreateCommand, TransactionCreateCommandResponse>
{
    private readonly IClientHttpRepository _repository;

    public TransactionCreateCommandHandler(
        IClientHttpRepository repository)
    {
        _repository = repository;
    }
    public Task<TransactionCreateCommandResponse> Handle(TransactionCreateCommand command, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }
}
