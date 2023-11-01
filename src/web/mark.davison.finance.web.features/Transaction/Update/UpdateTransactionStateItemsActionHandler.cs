namespace mark.davison.finance.web.features.Transaction.Update;

public class UpdateTransactionStateItemsActionHandler : IActionHandler<UpdateTransactionStateItemsAction>
{
    private readonly IStateStore _stateStore;

    public UpdateTransactionStateItemsActionHandler(
        IStateStore stateStore
    )
    {
        _stateStore = stateStore;
    }

    public Task Handle(UpdateTransactionStateItemsAction action, CancellationToken cancellation)
    {
        var existing = _stateStore.GetState<TransactionState>().Instance.Transactions;
        var updated = existing.ToDictionary(_ => _.Id, _ => _);

        foreach (var item in action.Items)
        {
            updated[item.Id] = item;
        }

        _stateStore.SetState(new TransactionState(updated.Values));
        return Task.CompletedTask;
    }
}
