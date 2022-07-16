namespace mark.davison.finance.web.features.Account;

public class AccountListState : IState
{
    public AccountListState()
    {
        Accounts = Enumerable.Empty<AccountListItemDto>();
    }
    public AccountListState(IEnumerable<AccountListItemDto> accounts)
    {
        Accounts = accounts.ToList();
    }

    public IEnumerable<AccountListItemDto> Accounts { get; private set; }

    public void Initialise()
    {
        Accounts = Enumerable.Empty<AccountListItemDto>();
    }
}
