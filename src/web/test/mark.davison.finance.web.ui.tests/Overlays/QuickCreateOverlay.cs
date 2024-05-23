namespace mark.davison.finance.web.ui.tests.Overlays;

public sealed class QuickCreateOverlay
{
    private readonly IPage _page;
    private readonly AppSettings _appSettings;

    private QuickCreateOverlay(IPage page, AppSettings appSettings)
    {
        _page = page;
        _appSettings = appSettings;
    }

    public static async Task<QuickCreateOverlay> Open(IPage page, AppSettings appSettings)
    {
        var overlay = new QuickCreateOverlay(page, appSettings);

        await page.GetByTestId(DataTestIds.QuickCreate).ClickAsync();

        return overlay;
    }

    public Task<NewAccountPage> CreateAssetAccount() => CreateAccount(AccountType.Asset);

    public async Task<NewAccountPage> CreateAccount(AccountType accountType)
    {
        await _page.GetByText($"New {accountType.ToString().ToLower()} account").ClickAsync();

        var newAccountPage = new NewAccountPage(_page, _appSettings, accountType);

        await _page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions
        {
            Name = $"New {accountType} account"
        }).WaitForAsync();

        return newAccountPage;
    }
}
