namespace mark.davison.finance.web.ui.tests.Pages;

public sealed record NewTransactionInfo(
    string Name,
    string SourceAccount,
    string DestinationAccount,
    decimal Amount,
    DateOnly Date);

public sealed class NewTransactionPage : BasePage
{
    private readonly TransactionType _transactionType;

    private const string CreateTransactionButton = "Create transaction";
    private const string AddSplitButton = "Add another split";
    private const string NameLabel = "Name";
    private const string SourceAccountLabel = "Source account";
    private const string DestinationAccountLabel = "Destination account";
    private const string AmountLabel = "Amount";
    private const string DateLabel = "Date";
    private const string CategoryLabel = "Category";

    public NewTransactionPage(
        IPage page,
        AppSettings appSettings,
        TransactionType transactionType
    ) : base(
        page,
        appSettings)
    {
        _transactionType = transactionType;
    }

    public async Task<NewTransactionPage> AddSplit()
    {
        await Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions
        {
            Name = AddSplitButton
        }).ClickAsync();

        return this;
    }
    public async Task<ViewTransactionPage> Submit(NewTransactionInfo transactionInfo)
    {
        return await Submit(string.Empty, [transactionInfo]);
    }
    public async Task<ViewTransactionPage> Submit(string splitDescription, IEnumerable<NewTransactionInfo> transactionInfo)
    {
        var transactionCount = transactionInfo.Count();
        foreach (var (info, i) in transactionInfo.Zip(Enumerable.Range(0, transactionCount)))
        {
            var locator = Page.Locator($"#edit-transaction-form-{i}");

            await FillField(this, locator, NameLabel, info.Name);
            await SelectField(this, locator, SourceAccountLabel, info.SourceAccount);
            await SelectField(this, locator, DestinationAccountLabel, info.DestinationAccount);
            await FillField(this, locator, AmountLabel, info.Amount);
            if (i == 0)
            {
                await FillField(this, locator, DateLabel, info.Date);
            }

            if (transactionCount > 1 && i < transactionCount - 1)
            {
                await Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions
                {
                    Name = AddSplitButton
                }).ClickAsync();
            }
        }

        if (transactionInfo.Count() > 1)
        {
            if (string.IsNullOrEmpty(splitDescription))
            {
                throw new InvalidOperationException("Split description must be specified");
            }

            await Page
                .GetByTestId(DataTestIds.EditTransactionSplitDescription)
                .FillAsync(splitDescription);
        }

        return await Submit();
    }

    public async Task<ViewTransactionPage> Submit()
    {
        await Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions
        {
            Name = CreateTransactionButton
        }).ClickAsync();

        return await ViewTransactionPage.Goto(Page, AppSettings);
    }
}
