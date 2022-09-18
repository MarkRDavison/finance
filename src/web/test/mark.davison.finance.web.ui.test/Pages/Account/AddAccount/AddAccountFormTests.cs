namespace mark.davison.finance.web.ui.test.Pages.Account.AddAccount;

[TestClass]
public class AddAccountFormTests : TestBase
{
    private readonly AddAccountFormViewModel _viewModel;

    public AddAccountFormTests()
    {
        _viewModel = new AddAccountFormViewModel();
    }

    [TestMethod]
    public void InitialForm_RendersWithExpectedFields()
    {
        _stateStore.SetState(AccountListStateHelpers.CreateAccountListState());
        _stateStore.SetState(LookupStateHelpers.CreateStandardState());
        _stateStore.SetState(CategoryListStateHelpers.CreateCategoryListState());

        var cut = RenderComponent<AddAccountForm>(_ => _
            .Add(_ => _.ViewModel, _viewModel));

        var inputs = cut.FindAll(".z-form-control");
        Assert.AreEqual(7, inputs.Count());
    }
}
