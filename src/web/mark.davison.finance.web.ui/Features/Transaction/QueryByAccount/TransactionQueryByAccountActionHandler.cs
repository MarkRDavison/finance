namespace mark.davison.finance.web.ui.Features.Transaction.QueryByAccount;

public class TransactionQueryByAccountActionHandler : IActionHandler<TransactionQueryByAccountAction>
{
    private readonly IStateStore _stateStore;

    public TransactionQueryByAccountActionHandler(
        IStateStore stateStore
    )
    {
        _stateStore = stateStore;
    }
    public Task Handle(TransactionQueryByAccountAction action, CancellationToken cancellationToken)
    {
        var existing = _stateStore.GetState<TransactionState>().Instance.Transactions;
        var updated = existing.ToDictionary(_ => _.Id, _ => _);

        foreach (var item in action.Transactions)
        {
            updated[item.Id] = item;
        }

        _stateStore.SetState(new TransactionState(updated.Values));
        return Task.CompletedTask;
    }
}
