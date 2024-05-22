namespace mark.davison.finance.web.components.Pages.Transactions.ViewTransaction;

public partial class ViewTransaction
{
    [Inject, EditorRequired]
    public required IState<StartupState> StartupState { get; set; }
    [Inject, EditorRequired]
    public required IState<AccountState> AccountListState { get; set; }
    [Inject, EditorRequired]
    public required IState<CategoryState> CategoryListState { get; set; }
    [Inject, EditorRequired]
    public required IState<TransactionState> TransactionState { get; set; }

    private IEnumerable<ViewTransactionItem> _items => ToCardItems();

    private DateOnly? _transactionDate;
    private string? _transactionDescription;
    private string? _transactionType;
    private string? _totalAmount;
    private string? _totalAmountStyle;
    private List<LinkInfo> _sourceAccounts = new();

    [Parameter]
    public required Guid Id { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await EnsureStateLoaded();
    }


    // TODO: Throttle
    //protected override async Task OnParametersSetAsync()
    //{
    //    await EnsureStateLoaded();
    //}

    private async Task EnsureStateLoaded()
    {
        await Task.WhenAll(
            _stateHelper.FetchTransactionInformation(Id),
            _stateHelper.FetchAccountList(false),
            _stateHelper.FetchCategoryList()
        );
    }

    // TODO: To helper
    private TransactionDto GetSourceTransaction(Guid transactionTypeId, List<TransactionDto> transactions)
    {
        if (transactionTypeId == TransactionConstants.Transfer)
        {
            return transactions.First(_ => _.Source);
        }
        else if (
            transactionTypeId == TransactionConstants.Deposit ||
            transactionTypeId == TransactionConstants.Withdrawal
        )
        {
            var sourceTransactionAccountTypes = AllowableSourceDestinationAccounts.GetSourceAccountTypes(transactionTypeId);

            return transactions.First(_ =>
            {
                // TODO: Helper on account list state to go from id -> accountType???
                // TODO: Loading this page directly crashes here, state not loaded???
                var account = AccountListState.Value.Accounts.First(__ => __.Id == _.AccountId);

                return sourceTransactionAccountTypes.Contains(account.AccountTypeId);
            });
        }

        return transactions.First(_ => _.Source);
    }

    // TODO: To helper
    private TransactionDto GetDestinationTransaction(Guid transactionTypeId, Guid sourceAccountId, Guid sourceAccountTypeId, List<TransactionDto> transactions)
    {
        if (transactionTypeId == TransactionConstants.Transfer)
        {
            return transactions.First(_ => !_.Source);
        }
        else if (
            transactionTypeId == TransactionConstants.Deposit ||
            transactionTypeId == TransactionConstants.Withdrawal
        )
        {
            var destTransactionAccountTypes = AllowableSourceDestinationAccounts.GetDestinationAccountTypesForSource(transactionTypeId, sourceAccountTypeId);

            return transactions.First(_ =>
            {
                if (_.AccountId == sourceAccountId)
                {
                    return false;
                }

                // TODO: Helper on account list state to go from id -> accountType???
                var account = AccountListState.Value.Accounts.First(__ => __.Id == _.AccountId);

                return destTransactionAccountTypes.Contains(account.AccountTypeId);
            });
        }

        return transactions.First(_ => !_.Source);
    }

    // TODO: Re-use/helperise, ViewAccount.razor.cs
    private string GetAmountText(Guid transactionTypeId, long amount, CurrencyDto? currency)
    {

        if (transactionTypeId == TransactionConstants.Transfer)
        {
            amount = Math.Abs(amount);
        }
        else if (transactionTypeId == TransactionConstants.Deposit)
        {
            amount = Math.Abs(amount);
        }

        bool negative = amount < 0;

        amount = Math.Abs(amount);

        return $"{(negative ? "-" : string.Empty)}{currency?.Symbol}{CurrencyRules.FromPersisted(amount).ToString($"N{currency?.DecimalPlaces ?? 2}")}";
    }

    private string GetAmountColour(Guid transactionTypeId, long amount)
    {
        if (transactionTypeId == TransactionConstants.Transfer)
        {
            return "color: #47b2f5; "; // blue
        }

        if (transactionTypeId == TransactionConstants.Deposit)
        {
            if (amount < 0)
            {

                return "color: #00ad5d; "; // green
            }
            else
            {
                return "color: #e47365; "; // red
            }
        }

        if (transactionTypeId == TransactionConstants.Withdrawal)
        {
            if (amount < 0)
            {

                return "color: #e47365; "; // red
            }
            else
            {
                return "color: #00ad5d; "; // green
            }
        }

        if (amount < 0)
        {

            return "color: #e47365; "; // red
        }
        else
        {
            return "color: #00ad5d; "; // green
        }
    }

    private IEnumerable<ViewTransactionItem> ToCardItems()
    {
        CurrencyDto? currency = null;
        Guid transactionTypeId = Guid.Empty;
        _sourceAccounts.Clear();

        var items = TransactionState.Value.Transactions
            .Where(_ => _.TransactionGroupId == Id)
            .GroupBy(_ => _.TransactionJournalId)
            .Select(_ =>
            {
                // TODO: NEXT: CURRENT: MEPLEASE:
                //  
                // Source and dest are not determined for this display using the Source property
                //
                // Instead it is based on the transaction type
                //  - Transfer: is a special case, uses blue and its neither a positive or a negative
                //  - Deposit: source is the revenue account, i.e. from work account -> (GREEN) -> saving account
                //  - Withdrawal: source is the asset account, i.e. from checking -> (RED) -> supermarket account
                //
                // Need to make a utility for getting the styles on the numbers, css/c#???
                //
                var transactions = _.ToList();

                transactionTypeId = transactions.Select(__ => __.TransactionTypeId).First();

                var source = GetSourceTransaction(transactionTypeId, transactions);

                var sourceAccount = AccountListState.Value.Accounts.FirstOrDefault(__ => __.Id == source.AccountId);

                if (sourceAccount == null)
                {
                    return (ViewTransactionItem?)null;
                }

                var dest = GetDestinationTransaction(transactionTypeId, source.AccountId, sourceAccount.AccountTypeId, transactions);

                var destAccount = AccountListState.Value.Accounts.FirstOrDefault(__ => __.Id == dest.AccountId);

                if (destAccount == null)
                {
                    return (ViewTransactionItem?)null;
                }

                currency = StartupState.Value.Currencies.FirstOrDefault(__ => __.Id == sourceAccount.CurrencyId);

                if (currency == null)
                {
                    return (ViewTransactionItem?)null;
                }

                var category = CategoryListState.Value.Categories.FirstOrDefault(__ => __.Id == source.CategoryId);

                var transactionType = StartupState.Value.TransactionTypes.First(__ => __.Id == transactionTypeId);

                _transactionType = transactionType.Type;
                _transactionDescription = _.First().SplitTransactionDescription;
                _transactionDate = _.First().Date;

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
                    Amount = GetAmountText(transactionTypeId, source.Amount, currency),
                    AmountValue = source.Amount,
                    ForeignAmount = "",
                    Category = category == null ? null : new LinkInfo
                    {
                        Text = category.Name,
                        Href = RouteHelpers.Category(category.Id)
                    },
                    AmountStyle = GetAmountColour(transactionTypeId, source.Amount)
                };
            })
            .Where(_ => _ != null)
            .Cast<ViewTransactionItem>();

        var totalAmount = items.Sum(_ => _.AmountValue);
        _totalAmount = GetAmountText(transactionTypeId, totalAmount, currency);
        _totalAmountStyle = GetAmountColour(transactionTypeId, totalAmount);
        _sourceAccounts.AddRange(items.Select(_ => _.SourceAccount).DistinctBy(_ => _.Href));

        return items;
    }
}
