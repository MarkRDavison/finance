namespace mark.davison.finance.web.features.Account.Add;

public class UpdateAccountListItemsActionHandler : IActionHandler<UpdateAccountListItemsAction>
{
    private readonly IStateStore _stateStore;

    public UpdateAccountListItemsActionHandler(
        IStateStore stateStore
    )
    {
        _stateStore = stateStore;
    }

    public Task Handle(UpdateAccountListItemsAction action, CancellationToken cancellationToken)
    {
        var existing = _stateStore.GetState<AccountListState>().Instance.Accounts;
        var updated = existing.ToDictionary(_ => _.Id, _ => _);

        foreach (var item in action.Items)
        {
            updated[item.Id] = item;
        }

        _stateStore.SetState(new AccountListState(updated.Values));
        return Task.CompletedTask;
    }
}
