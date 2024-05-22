namespace mark.davison.finance.web.components.test.Pages.Accounts.EditAccount;

[TestClass]
public class EditAccountFormTests : BunitTestContext
{
    [TestMethod]
    public void ExpectedControlsAreRendered()
    {
        var cut = RenderComponent<EditAccountForm>(_ => _
            .Add(p => p.FormViewModel, new EditAccountFormViewModel()));

        var name = cut.Find("#edit-account-form-name");
        Assert.IsNotNull(name);

        var accountNumber = cut.Find("#edit-account-form-account-number");
        Assert.IsNotNull(accountNumber);

        var accountType = cut.Find("#edit-account-form-account-type");
        Assert.IsNotNull(accountType);

        var currency = cut.Find("#edit-account-form-currency");
        Assert.IsNotNull(currency);

        var openingBalance = cut.Find("#edit-account-form-opening-balance");
        Assert.IsNotNull(openingBalance);

        var virtualBalance = cut.Find("#edit-account-form-virtual-balance");
        Assert.IsNotNull(virtualBalance);

        var openingBalanceDate = cut.Find("#edit-account-form-opening-balance-date");
        Assert.IsNotNull(openingBalanceDate);
    }

    [TestMethod]
    public void ExpectedControlsAreRendered_WhenAccountTypeHidden()
    {
        var cut = RenderComponent<EditAccountForm>(_ => _
            .Add(p => p.FormViewModel, new EditAccountFormViewModel
            {
                HideAccountType = true
            }));

        var name = cut.Find("#edit-account-form-name");
        Assert.IsNotNull(name);

        var accountNumber = cut.Find("#edit-account-form-account-number");
        Assert.IsNotNull(accountNumber);

        var accountType = cut.FindAll("#edit-account-form-account-type");
        Assert.IsFalse(accountType.Any());

        var currency = cut.Find("#edit-account-form-currency");
        Assert.IsNotNull(currency);

        var openingBalance = cut.Find("#edit-account-form-opening-balance");
        Assert.IsNotNull(openingBalance);

        var virtualBalance = cut.Find("#edit-account-form-virtual-balance");
        Assert.IsNotNull(virtualBalance);

        var openingBalanceDate = cut.Find("#edit-account-form-opening-balance-date");
        Assert.IsNotNull(openingBalanceDate);
    }
}
