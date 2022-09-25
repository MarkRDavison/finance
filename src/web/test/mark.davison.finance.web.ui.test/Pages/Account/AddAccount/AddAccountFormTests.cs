namespace mark.davison.finance.web.ui.test.Pages.Account.EditAccount;

[TestClass]
public class EditAccountFormTests : TestBase
{
    private readonly EditAccountFormViewModel _viewModel;

    public EditAccountFormTests()
    {
        _viewModel = new EditAccountFormViewModel();
    }

    [TestMethod]
    public void InitialForm_RendersWithExpectedFields()
    {
        _stateStore.SetState(AccountListStateHelpers.CreateAccountListState());
        _stateStore.SetState(LookupStateHelpers.CreateStandardState());
        _stateStore.SetState(CategoryListStateHelpers.CreateCategoryListState());

        var cut = RenderComponent<EditAccountForm>(_ => _
            .Add(_ => _.ViewModel, _viewModel));

        var inputs = cut.FindAll(".z-form-control");
        Assert.AreEqual(7, inputs.Count());
    }
}
