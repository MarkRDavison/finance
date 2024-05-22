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

        var actionResponse = new FetchStartupActionResponse
        {
            Errors = queryResponse.Errors,
            Warnings = queryResponse.Warnings,
            ActionId = action.ActionId,
            Value = queryResponse.Value
        };

        // TODO: Framework to dispatch general ***something went wrong***

        dispatcher.Dispatch(actionResponse);
    }
}
