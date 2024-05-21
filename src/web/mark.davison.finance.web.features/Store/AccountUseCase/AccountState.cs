namespace mark.davison.finance.web.features.Store.AccountUseCase;

[FeatureState]
public sealed class AccountState
{
    public AccountState() : this([])
    {
    }

    public AccountState(IEnumerable<AccountListItemDto> accounts)
    {
        Accounts = new(accounts.ToList());
    }

    public ReadOnlyCollection<AccountListItemDto> Accounts { get; }
}
