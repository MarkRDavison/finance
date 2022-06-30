namespace mark.davison.finance.web.ui.Features.AccountList;

public partial class AccountListState : State<AccountListState>
{
    public AccountListState(IEnumerable<AccountListItemDto> accounts)
    {
        Accounts = accounts.ToList();
    }

    public IEnumerable<AccountListItemDto> Accounts { get; }

    public override void Initialize()
    {
    }
}
