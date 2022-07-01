namespace mark.davison.finance.web.ui.Features.Account.Add;

public class UpdateAccountListItemsAction : IAction<UpdateAccountListItemsAction>
{

    public UpdateAccountListItemsAction(IEnumerable<AccountListItemDto> items)
    {
        Items = items;
    }

    public IEnumerable<AccountListItemDto> Items { get; }
}
