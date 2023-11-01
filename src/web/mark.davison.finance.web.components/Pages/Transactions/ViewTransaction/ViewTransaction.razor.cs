namespace mark.davison.finance.web.components.Pages.Transactions.ViewTransaction;

public partial class ViewTransaction
{
    private IStateInstance<TransactionState> _transactionState { get; set; } = default!;
    private IStateInstance<AccountListState> _accountListState { get; set; } = default!;
    private IStateInstance<LookupState> _lookupState { get; set; } = default!;
    private IStateInstance<CategoryListState> _categoryListState { get; set; } = default!;

    private IEnumerable<ViewTransactionItem> _items => ToCardItems();

    [Parameter]
    public required Guid Id { get; set; }

    protected override async Task OnInitializedAsync()
    {
        _transactionState = GetState<TransactionState>();
        _accountListState = GetState<AccountListState>();
        _lookupState = GetState<LookupState>();
        _categoryListState = GetState<CategoryListState>();

        await EnsureStateLoaded();
    }

    protected override async Task OnParametersSetAsync()
    {
        await EnsureStateLoaded();
    }

    private async Task EnsureStateLoaded()
    {
        await Task.WhenAll(
            _stateHelper.FetchTransactionInformation(Id),
            _stateHelper.FetchAccountList(false),
            _stateHelper.FetchCategoryList()
        );
    }

    private IEnumerable<ViewTransactionItem> ToCardItems()
    {
        return _transactionState.Instance.Transactions
            .Where(_ => _.TransactionGroupId == Id)
            .GroupBy(_ => _.TransactionJournalId)
            .Select(_ =>
            {
                var source = _.First(__ => __.Source);
                var dest = _.First(__ => !__.Source);

                var sourceAccount = _accountListState.Instance.Accounts.FirstOrDefault(__ => __.Id == source.AccountId);
                var destAccount = _accountListState.Instance.Accounts.FirstOrDefault(__ => __.Id == dest.AccountId);

                if (sourceAccount == null || destAccount == null)
                {
                    return (ViewTransactionItem?)null;
                }

                var currency = _lookupState.Instance.Currencies.FirstOrDefault(__ => __.Id == sourceAccount.CurrencyId);

                if (currency == null)
                {
                    return (ViewTransactionItem?)null;
                }

                var category = _categoryListState.Instance.Categories.FirstOrDefault(__ => __.Id == source.CategoryId);

                return new ViewTransactionItem
                {
                    Description = source.Description,
                    SourceAccount = new LinkInfo
                    {
                        Text = sourceAccount.Name,
                        Href = RouteHelpers.Account(sourceAccount.Id)
                    },
                    DestinationAccount = new LinkInfo
                    {
                        Text = destAccount.Name,
                        Href = RouteHelpers.Account(destAccount.Id)
                    },
                    Amount = $"{currency.Symbol}{CurrencyRules.FromPersisted(source.Amount).ToString($"N{currency.DecimalPlaces}")}",
                    ForeignAmount = "",
                    Category = category == null ? null : new LinkInfo
                    {
                        Text = category.Name,
                        Href = RouteHelpers.Category(category.Id)
                    },
                    // Transfer == blue
                    AmountStyle = source.Amount < 0 ? "color: #e47365; " : "color: #00ad5d; "
                };
            })
            .Where(_ => _ != null)
            .Cast<ViewTransactionItem>();
    }
}
