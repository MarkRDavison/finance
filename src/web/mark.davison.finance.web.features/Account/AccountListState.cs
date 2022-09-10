namespace mark.davison.finance.web.features.Account;

public class AccountListState : IState
{
    public AccountListState() : this(Enumerable.Empty<AccountListItemDto>())
    {
    }

    public AccountListState(IEnumerable<AccountListItemDto> accounts)
    {
        Accounts = accounts.ToList();
        LastModified = DateTime.Now;
    }

    public IEnumerable<AccountListItemDto> Accounts { get; private set; }
    public DateTime LastModified { get; private set; }

    public void Initialise()
    {
        Accounts = Enumerable.Empty<AccountListItemDto>();
        LastModified = default;
    }
}
