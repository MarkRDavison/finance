namespace mark.davison.finance.web.ui.test.Helpers.State;

public static class AccountListStateHelpers
{
    public static AccountListState CreateAccountListState(params AccountListItemDto[] accountListItems)
    {
        return new AccountListState(accountListItems);
    }
}
