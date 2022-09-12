namespace mark.davison.finance.web.ui.test.Pages.Categories.AddCategory;

[TestClass]
public class AddCategoryFromTests : TestBase
{
    private readonly AddCategoryFormViewModel _viewModel;

    public AddCategoryFromTests()
    {
        _viewModel = new AddCategoryFormViewModel();
    }

    [TestMethod]
    public void InitialForm_RendersWithExpectedFields()
    {
        _stateStore.SetState(CategoryListStateHelpers.CreateCategoryListState());

        var cut = RenderComponent<AddCategoryForm>(_ => _
            .Add(_ => _.ViewModel, _viewModel));

        var inputs = cut.FindAll(".z-form-control");
        Assert.AreEqual(1, inputs.Count());
    }

    [TestMethod]
    public void UpdatingName_UpdatesViewModel()
    {
        var nameText = "some name";
        _stateStore.SetState(CategoryListStateHelpers.CreateCategoryListState());

        var cut = RenderComponent<AddCategoryForm>(_ => _
            .Add(_ => _.ViewModel, _viewModel));

        cut.SetTextInputValueByLabel("Name", nameText);
        Assert.AreEqual(nameText, _viewModel.Name);
    }
}
