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

    public async Task Handle(UpdateLookupsAction action, CancellationToken cancellation)
    {
        var response = await _repository.Get<StartupQueryResponse, StartupQueryRequest>(cancellation);
        _stateStore.SetState(new LookupState(
            response.Banks,
            response.AccountTypes,
            response.Currencies,
            response.TransactionTypes
        ));
    }
}
