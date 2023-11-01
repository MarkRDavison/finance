namespace mark.davison.finance.web.features.Transaction.QueryById;

public class TransactionQueryByIdActionHandler : IActionHandler<TransactionQueryByIdAction>
{
    private readonly IClientHttpRepository _repository;
    private readonly IStateStore _stateStore;

    public TransactionQueryByIdActionHandler(
        IClientHttpRepository repository,
        IStateStore stateStore)
    {
        _repository = repository;
        _stateStore = stateStore;
    }

    public async Task Handle(TransactionQueryByIdAction action, CancellationToken cancellationToken)
    {
        var updated = _stateStore.GetState<TransactionState>().Instance.Transactions.ToDictionary(_ => _.Id, _ => _);

        var response = await _repository.Get<TransactionByIdQueryResponse, TransactionByIdQueryRequest>(new TransactionByIdQueryRequest
        {
            TransactionGroupId = action.TransactionGroupId
        }, cancellationToken);

        // TODO: Consolidate with TransactionQueryByAccountActionHandler??
        foreach (var item in response.Transactions)
        {
            updated[item.Id] = item;
        }

        _stateStore.SetState(new TransactionState(updated.Values));
    }
}
