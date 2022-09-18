namespace mark.davison.finance.web.features.Lookup;

public class UpdateLookupsActionHandler : IActionHandler<UpdateLookupsAction>
{

    private readonly IClientHttpRepository _repository;
    private readonly IStateStore _stateStore;

    public UpdateLookupsActionHandler(
        IClientHttpRepository repository,
        IStateStore stateStore)
    {
        _repository = repository;
        _stateStore = stateStore;
    }

    public async Task Handle(UpdateLookupsAction action, CancellationToken cancellationToken)
    {
        var response = await _repository.Get<StartupQueryResponse, StartupQueryRequest>(cancellationToken);
        _stateStore.SetState(new LookupState(
            response.AccountTypes,
            response.Currencies,
            response.TransactionTypes
        ));
    }
}
