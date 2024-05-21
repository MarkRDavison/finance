namespace mark.davison.finance.web.features.Store.DashboardUseCase;

public sealed class DashboardEffects
{
    private readonly IClientHttpRepository _repository;

    public DashboardEffects(IClientHttpRepository repository)
    {
        _repository = repository;
    }

    [EffectMethod]
    public async Task HandleFetchDashboardSummaryActionAsync(FetchDashboardSummaryAction action, IDispatcher dispatcher)
    {
        var commandRequest = new AccountDashboardSummaryQueryRequest
        {
            AccountTypeId = action.AccountTypeId
        };

        var commandResponse = await _repository.Get<AccountDashboardSummaryQueryResponse, AccountDashboardSummaryQueryRequest>(commandRequest, CancellationToken.None);

        var actionResponse = FetchDashboardSummaryActionResponse.From(commandResponse);
        actionResponse.ActionId = action.ActionId;
        actionResponse.Value = commandResponse.Value;

        // TODO: Framework to dispatch general ***something went wrong***

        dispatcher.Dispatch(actionResponse);
    }
}
