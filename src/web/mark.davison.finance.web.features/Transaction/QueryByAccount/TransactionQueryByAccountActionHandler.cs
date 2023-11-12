namespace mark.davison.finance.web.features.Transaction.QueryByAccount;

public class TransactionQueryByAccountActionHandler : IActionHandler<TransactionQueryByAccountAction>
{
    private readonly IClientHttpRepository _repository;
    private readonly IStateStore _stateStore;

    public TransactionQueryByAccountActionHandler(
        IClientHttpRepository repository,
        IStateStore stateStore)
    {
        _repository = repository;
        _stateStore = stateStore;
    }

    public async Task Handle(TransactionQueryByAccountAction action, CancellationToken cancellationToken)
    {
        var updated = _stateStore.GetState<TransactionState>().Instance.Transactions.ToDictionary(_ => _.Id, _ => _);

        var response = await _repository.Get<TransactionByAccountQueryResponse, TransactionByAccountQueryRequest>(new TransactionByAccountQueryRequest
        {
            AccountId = action.AccountId
        }, cancellationToken);

        // TODO: Consolidate with TransactionQueryByIdActionHandler??
        foreach (var item in response.Transactions)
        {
            updated[item.Id] = item;
        }

        _stateStore.SetState(new TransactionState(updated.Values));
    }
}
