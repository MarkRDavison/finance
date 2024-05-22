using mark.davison.finance.models.dtos.Commands.CreateTransaction;

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

        var actionResponse = new FetchTransactionsByGroupResponse
        {
            ActionId = action.ActionId,
            Errors = commandResponse.Errors,
            Warnings = commandResponse.Warnings,
            Value = commandResponse.Value
        };

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

        var actionResponse = new FetchTransactionsByAccountResponse
        {
            ActionId = action.ActionId,
            Errors = commandResponse.Errors,
            Warnings = commandResponse.Warnings,
            Value = commandResponse.Value
        };

        // TODO: Framework to dispatch general ***something went wrong***

        dispatcher.Dispatch(actionResponse);
    }

    [EffectMethod]
    public async Task HandleCreateTransactionAction(CreateTransactionAction action, IDispatcher dispatcher)
    {
        var commandRequest = new CreateTransactionRequest
        {
            Description = action.Request.Description,
            Transactions = action.Request.Transactions,
            TransactionTypeId = action.Request.TransactionTypeId
        };

        var commandResponse = await _repository.Post<CreateTransactionResponse, CreateTransactionRequest>(commandRequest, CancellationToken.None);

        Console.WriteLine("CreateTransactionResponse received");

        var actionResponse = new CreateTransactionActionResponse
        {
            ActionId = action.ActionId,
            Warnings = commandResponse.Warnings,
            Errors = commandResponse.Errors,
            Group = commandResponse.Group,
            Journals = commandResponse.Journals,
            Transactions = commandResponse.Transactions
        };

        // TODO: Framework to dispatch general ***something went wrong***
        Console.WriteLine("CreateTransactionActionResponse dispatched");
        Console.WriteLine("CreateTransactionActionResponse   ActionId: {0}", action.ActionId);
        Console.WriteLine("CreateTransactionActionResponse ResponseId: {0}", actionResponse.ActionId);

        dispatcher.Dispatch(actionResponse);
    }
}
