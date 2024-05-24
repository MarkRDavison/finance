namespace mark.davison.finance.web.ui.tests.Scenarios.Account;

[TestClass]
public sealed class AddAccountTests : LoggedInTest
{
    [DataRow(AccountType.Asset)]
    [DataRow(AccountType.Expense)]
    [DataRow(AccountType.Revenue)]
    [DataTestMethod]
    public async Task AddAccountWorks(AccountType accountType)
    {
        var accountName = MakeAccountName();
        var accountNumber = MakeAccountNumber();

        var createAccount = await Dashboard
            .OpenQuickCreate()
            .ThenAsync(_ => _.CreateAccount(accountType));

        var newAccount = await createAccount
            .Submit(new NewAccountInfo(
                accountName,
                accountNumber,
                CurrencyConstants.NZD));

        newAccount.ExpectAccountName(accountName);
    }
}
