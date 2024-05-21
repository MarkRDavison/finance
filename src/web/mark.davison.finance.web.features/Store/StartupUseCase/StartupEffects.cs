namespace mark.davison.finance.web.features.Store.StartupUseCase;

public sealed class StartupEffects
{
    private readonly IClientHttpRepository _repository;

    public StartupEffects(IClientHttpRepository repository)
    {
        _repository = repository;
    }

    [EffectMethod]
    public async Task HandleFetchStartupActionAsync(FetchStartupAction action, IDispatcher dispatcher)
    {
        var queryResponse = await _repository.Get<StartupQueryResponse, StartupQueryRequest>(CancellationToken.None);

        var actionResponse = FetchStartupActionResponse.From(queryResponse);
        actionResponse.ActionId = action.ActionId;
        actionResponse.Value = queryResponse.Value;

        // TODO: Framework to dispatch general ***something went wrong***

        dispatcher.Dispatch(actionResponse);
    }
}
