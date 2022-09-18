namespace mark.davison.finance.web.ui.test.Helpers.State;

public static class AccountListStateHelpers
{
    public static AccountListState CreateAccountListState(params AccountListItemDto[] accountListItems)
    {
        return new AccountListState(accountListItems);
    }

    public static AccountListItemDto CreateAssetAccount() => new AccountListItemDto
    {
        Id = Guid.NewGuid(),
        AccountTypeId = AccountConstants.Asset
    };
    public static AccountListItemDto CreateRevenueAccount() => new AccountListItemDto
    {
        Id = Guid.NewGuid(),
        AccountTypeId = AccountConstants.Revenue
    };
}
