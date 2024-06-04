using mark.davison.finance.web.services.AppContext;

namespace mark.davison.finance.web.features.Store.StartupUseCase;

public sealed class StartupEffects
{
    private readonly IClientHttpRepository _repository;
    private readonly IAppContextService _appContextService;

    public StartupEffects(
        IClientHttpRepository repository,
        IAppContextService appContextService)
    {
        _repository = repository;
        _appContextService = appContextService;
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

        if (actionResponse.SuccessWithValue)
        {
            _appContextService.UpdateRange(actionResponse.Value.UserContext.StartRange, actionResponse.Value.UserContext.EndRange);
        }

        // TODO: Framework to dispatch general ***something went wrong***

        dispatcher.Dispatch(actionResponse);
    }
}
