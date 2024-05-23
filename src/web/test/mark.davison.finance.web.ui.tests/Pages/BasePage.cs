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

    public Task<QuickCreateOverlay> OpenQuickCreate() => QuickCreateOverlay.Open(Page, AppSettings);
}
