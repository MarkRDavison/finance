namespace mark.davison.finance.web.ui.tests.Pages;

public sealed class ViewAccountPage : BasePage
{
    private readonly string _title;

    private ViewAccountPage(IPage page, AppSettings appSettings, string title) : base(page, appSettings)
    {
        _title = title;
    }

    public static async Task<ViewAccountPage> Goto(IPage page, AppSettings appSettings)
    {
        var titleLocator = page.GetByTestId(DataTestIds.ViewAccountTitle);

        await titleLocator.WaitForAsync();

        var title = await titleLocator.InnerTextAsync();

        var viewAccountPage = new ViewAccountPage(page, appSettings, title);

        return viewAccountPage;
    }

    public ViewAccountPage ExpectAccountName(string name)
    {
        _title.Should().Be(name);

        return this;
    }
}
