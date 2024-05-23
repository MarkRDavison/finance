namespace mark.davison.finance.web.ui.tests.Pages;

public class BasePage
{
    protected readonly IPage Page;
    protected readonly AppSettings AppSettings;

    public BasePage(IPage page, AppSettings appSettings)
    {
        Page = page;
        AppSettings = appSettings;
    }
}
