namespace mark.davison.finance.web.features.Store.AccountUseCase;

public sealed class FetchAccountsAction : BaseAction
{
    public bool ShowActive { get; }
}

public sealed class FetchAccountsActionResponse : BaseActionResponse<List<AccountListItemDto>>
{

}

public sealed class CreateAccountAction : BaseAction
{
    public UpsertAccountDto UpsertAccountDto { get; set; } = new();
}

public sealed class CreateAccountActionResponse : BaseActionResponse<AccountListItemDto>
{

}