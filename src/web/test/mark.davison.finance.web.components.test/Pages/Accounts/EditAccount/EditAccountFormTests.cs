using mark.davison.finance.web.components.Pages.Accounts.EditAccount;
using mark.davison.finance.web.features.Lookup;

namespace mark.davison.finance.web.components.test.Pages.Accounts.EditAccount;

[TestClass]
public class EditAccountFormTests : BunitTestContext
{
    [TestMethod]
    public async Task POC_BUNIT_TEST()
    {
        SetState(new LookupState());

        var cut = RenderComponent<EditAccountForm>(_ => _
            .Add(__ => __.FormViewModel, new EditAccountFormViewModel()));

        await Task.CompletedTask;

        Assert.Fail();
    }
}
