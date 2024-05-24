﻿namespace mark.davison.finance.web.ui.tests.Overlays;

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

    public async Task<NewAccountPage> CreateAccount(AccountType accountType)
    {
        var heading = $"New {accountType.ToString().ToLower()} account";

        await _page.GetByText(heading).ClickAsync();

        return new NewAccountPage(_page, _appSettings, accountType);
    }

    public async Task<NewTransactionPage> CreateTransaction(TransactionType transactionType)
    {
        var heading = $"New {transactionType.ToString().ToLower()}";

        await _page.GetByText(heading).ClickAsync();

        return new NewTransactionPage(_page, _appSettings, transactionType);
    }
}
