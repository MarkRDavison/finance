namespace mark.davison.finance.web.ui.tests.Pages;

public sealed class NewAccountPage : BasePage
{
    private readonly AccountType _accountType;

    private const string NameLabel = "Name";
    private const string AccountNumberLabel = "Account number";
    private const string CurrencyLabel = "Currency";
    private const string CreateAccountButton = "Create account";

    public NewAccountPage(
        IPage page,
        AppSettings appSettings,
        AccountType accountType
    ) : base(
        page,
        appSettings)
    {
        _accountType = accountType;
    }

    public Task<NewAccountPage> FillName(string value) => FillField(NameLabel, value);
    public Task<NewAccountPage> FillAccountNumber(string value) => FillField(AccountNumberLabel, value);
    public Task<NewAccountPage> FillCurrency(string value) => SelectField(CurrencyLabel, value);

    private async Task<NewAccountPage> FillField(string label, string value)
    {
        await Page.GetByLabel(label).FillAsync(value);
        return this;
    }

    private async Task<NewAccountPage> SelectField(string label, string value)
    {
        await ComponentHelpers.SelectAsync(Page, label, value);
        return this;
    }

    public async Task<ViewAccountPage> Submit()
    {
        await Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions
        {
            Name = CreateAccountButton
        }).ClickAsync();

        return await ViewAccountPage.Goto(Page, AppSettings);
    }
}
