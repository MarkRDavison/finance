namespace mark.davison.finance.web.ui.tests.Pages;

public sealed class ViewTransactionPage(IPage page, AppSettings appSettings) : BasePage(page, appSettings)
{
    public static async Task<ViewTransactionPage> Goto(IPage page, AppSettings appSettings)
    {
        await page.GetByTestId(DataTestIds.ViewTransactionTitle).WaitForAsync();

        var viewTransactionPage = new ViewTransactionPage(page, appSettings);

        return viewTransactionPage;
    }

    public async Task<ViewTransactionPage> ExpectGroupDescription(string description)
    {
        var transactionDescription = await Page.GetByTestId(DataTestIds.ViewTransactionDescription).TextContentAsync();

        transactionDescription.Should().Be(description);

        return this;
    }

    public async Task<ViewTransactionPage> ExpectTransactionType(TransactionType type)
    {
        var transactionDescription = await Page.GetByTestId(DataTestIds.ViewTransactionType).TextContentAsync();

        transactionDescription.Should().Be(type.ToString());

        return this;
    }
}
