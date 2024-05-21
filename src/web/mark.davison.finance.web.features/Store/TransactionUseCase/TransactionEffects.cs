namespace mark.davison.finance.web.features.Store.TransactionUseCase;

public sealed class TransactionEffects
{
    private readonly IClientHttpRepository _repository;

    public TransactionEffects(IClientHttpRepository repository)
    {
        _repository = repository;
    }

    [EffectMethod]
    public async Task HandleFetchTransactionsByGroupAsync(FetchTransactionsByGroup action, IDispatcher dispatcher)
    {
        var commandRequest = new TransactionByIdQueryRequest
        {
            TransactionGroupId = action.TransactionGroupId
        };

        var commandResponse = await _repository.Get<TransactionByIdQueryResponse, TransactionByIdQueryRequest>(commandRequest, CancellationToken.None);

        var actionResponse = FetchTransactionsByGroupResponse.From(commandResponse);
        actionResponse.ActionId = action.ActionId;
        actionResponse.Value = commandResponse.Value;

        // TODO: Framework to dispatch general ***something went wrong***

        dispatcher.Dispatch(actionResponse);
    }

    [EffectMethod]
    public async Task HandleFetchTransactionsByAccountAsync(FetchTransactionsByAccount action, IDispatcher dispatcher)
    {
        var commandRequest = new TransactionByAccountQueryRequest
        {
            AccountId = action.AccountId
        };

        var commandResponse = await _repository.Get<TransactionByAccountQueryResponse, TransactionByAccountQueryRequest>(commandRequest, CancellationToken.None);

        var actionResponse = FetchTransactionsByAccountResponse.From(commandResponse);
        actionResponse.ActionId = action.ActionId;
        actionResponse.Value = commandResponse.Value;

        // TODO: Framework to dispatch general ***something went wrong***

        dispatcher.Dispatch(actionResponse);
    }
}
