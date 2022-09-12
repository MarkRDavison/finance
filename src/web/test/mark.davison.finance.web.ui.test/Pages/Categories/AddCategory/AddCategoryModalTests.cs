namespace mark.davison.finance.web.ui.test.Pages.Categories.AddCategory;

[TestClass]
public class AddCategoryModalTests : TestBase
{
    private readonly AddCategoryModalViewModal _viewModel;

    public AddCategoryModalTests()
    {
        _viewModel = new(_dispatcher.Object);
    }
    protected override void SetupTest()
    {
        base.SetupTest();
        Services.Add(new ServiceDescriptor(typeof(AddCategoryModalViewModal), _viewModel));
    }

    [TestMethod]
    public void ModalRenders_WhenOpened()
    {
        var cut = RenderComponent<AddCategoryModal>(_ => _
            .Add(_ => _.IsOpen, true));

        var buttons = cut.FindAll(".z-btn");
        Assert.AreEqual(2, buttons.Count);
        Assert.IsNotNull(buttons.FirstOrDefault(_ => _.TextContent == "Save"));
        Assert.IsNotNull(buttons.FirstOrDefault(_ => _.TextContent == "Cancel"));

        var title = cut.Find("h1");
        Assert.IsNotNull(title);
        Assert.AreEqual("Add Category", title.TextContent);

        var form = cut.FindAll("#AddCategoryForm");
        Assert.IsNotNull(form);
    }
}
