namespace mark.davison.finance.web.features.Account.List;

public class FetchAccountListActionHandler : IActionHandler<FetchAccountListAction>
{
    private readonly IClientHttpRepository _repository;
    private readonly IStateStore _stateStore;

    public FetchAccountListActionHandler(
        IClientHttpRepository repository,
        IStateStore stateStore)
    {
        _repository = repository;
        _stateStore = stateStore;
    }

    public async Task Handle(FetchAccountListAction action, CancellationToken cancellationToken)
    {
        var response = await _repository.Get<AccountListQueryResponse, AccountListQueryRequest>(new AccountListQueryRequest
        {
            ShowActive = action.ShowActive
        });

        _stateStore.SetState(new AccountListState(response.Accounts));
    }
}
