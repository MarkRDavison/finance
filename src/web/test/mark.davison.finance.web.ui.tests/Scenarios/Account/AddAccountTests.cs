namespace mark.davison.finance.web.ui.tests.Scenarios.Account;

[TestClass]
public sealed class AddAccountTests : LoggedInTest
{
    [TestMethod]
    public async Task AddAssertAccountWorks()
    {
        var accountName = MakeAccountName();
        var accountNumber = MakeAccountNumber();

        var createAccount = await Dashboard
            .OpenQuickCreate()
            .ThenAsync(_ => _.CreateAssetAccount());

        var newAccount = await createAccount
            .FillName(accountName)
            .ThenAsync(_ => _.FillAccountNumber(accountNumber))
            .ThenAsync(_ => _.FillCurrency(CurrencyConstants.NZD))
            .ThenAsync(_ => _.Submit());

        newAccount.ExpectAccountName(accountName);
    }
}
