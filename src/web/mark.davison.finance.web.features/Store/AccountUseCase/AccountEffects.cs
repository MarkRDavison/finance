using mark.davison.finance.models.dtos.Commands.UpsertAccount;

namespace mark.davison.finance.web.features.Store.AccountUseCase;

public sealed class AccountEffects
{
    private readonly IClientHttpRepository _repository;

    public AccountEffects(IClientHttpRepository repository)
    {
        _repository = repository;
    }

    [EffectMethod]
    public async Task HandleFetchAccountsActionAsync(FetchAccountsAction action, IDispatcher dispatcher)
    {
        // TODO: Rename query req/res
        var queryResponse = await _repository.Get<AccountListQueryResponse, AccountListQueryRequest>(CancellationToken.None);

        // TODO: Helper to remove the action id and value from 2 more lines
        var actionResponse = new FetchAccountsActionResponse
        {
            Errors = queryResponse.Errors,
            Warnings = queryResponse.Warnings,
            ActionId = action.ActionId,
            Value = queryResponse.Value
        };

        // TODO: Framework to dispatch general ***something went wrong***

        dispatcher.Dispatch(actionResponse);
    }

    [EffectMethod]
    public async Task HandleCreateAccountActionAsync(CreateAccountAction action, IDispatcher dispatcher)
    {
        var commandRequest = new UpsertAccountCommandRequest
        {
            UpsertAccountDto = action.UpsertAccountDto
        };

        var commandResponse = await _repository.Post<UpsertAccountCommandResponse, UpsertAccountCommandRequest>(commandRequest, CancellationToken.None);

        var actionResponse = new CreateAccountActionResponse
        {
            Errors = commandResponse.Errors,
            Warnings = commandResponse.Warnings,
            ActionId = action.ActionId,
            Value = commandResponse.Value
        };

        // TODO: Framework to dispatch general ***something went wrong***

        dispatcher.Dispatch(actionResponse);
    }
}
