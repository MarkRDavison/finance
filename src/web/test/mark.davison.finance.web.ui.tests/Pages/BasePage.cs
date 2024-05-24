namespace mark.davison.finance.web.ui.tests.Pages;

public abstract class BasePage
{
    protected readonly IPage Page;
    protected readonly AppSettings AppSettings;

    protected BasePage(IPage page, AppSettings appSettings)
    {
        Page = page;
        AppSettings = appSettings;
    }

    public TPage GoToPage<TPage>() where TPage : BasePage
    {
        throw new NotImplementedException();
    }

    protected async Task<TPage> FillField<TPage>(TPage page, string label, string value) where TPage : BasePage
    {
        await Page.GetByLabel(label).FillAsync(value);
        return page;
    }
    protected async Task<TPage> FillField<TPage>(TPage page, ILocator locator, string label, string value) where TPage : BasePage
    {
        await locator.GetByLabel(label).FillAsync(value);
        return page;
    }
    protected async Task<TPage> FillField<TPage>(TPage page, string label, decimal value) where TPage : BasePage
    {
        return await FillField<TPage>(page, label, value.ToString());
    }
    protected async Task<TPage> FillField<TPage>(TPage page, ILocator locator, string label, decimal value) where TPage : BasePage
    {
        return await FillField<TPage>(page, locator, label, value.ToString());
    }
    protected async Task<TPage> FillField<TPage>(TPage page, string label, DateOnly value) where TPage : BasePage
    {
        await ComponentHelpers.SelectDate(Page, label, value);
        return page;
    }
    protected async Task<TPage> FillField<TPage>(TPage page, ILocator locator, string label, DateOnly value) where TPage : BasePage
    {
        await ComponentHelpers.SelectDate(Page, locator, label, value);
        return page;
    }
    protected async Task<TPage> SelectField<TPage>(TPage page, string label, string value) where TPage : BasePage
    {
        await ComponentHelpers.SelectAsync(Page, label, value);
        return page;
    }
    protected async Task<TPage> SelectField<TPage>(TPage page, ILocator locator, string label, string value) where TPage : BasePage
    {
        await ComponentHelpers.SelectAsync(Page, locator, label, value);
        return page;
    }

    public Task<QuickCreateOverlay> OpenQuickCreate() => QuickCreateOverlay.Open(Page, AppSettings);

}
