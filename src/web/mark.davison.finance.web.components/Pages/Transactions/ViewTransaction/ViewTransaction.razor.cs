using mark.davison.finance.accounting.constants;

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
                var account = _accountListState.Instance.Accounts.First(__ => __.Id == _.AccountId);

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
                var account = _accountListState.Instance.Accounts.First(__ => __.Id == _.AccountId);

                return destTransactionAccountTypes.Contains(account.AccountTypeId);
            });
        }

        return transactions.First(_ => !_.Source);
    }

    // TODO: Re-use/helperise, ViewAccount.razor.cs
    private string GetAmountText(Guid transactionTypeId, long amount, CurrencyDto currency)
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

        return $"{(negative ? "-" : string.Empty)}{currency.Symbol}{CurrencyRules.FromPersisted(amount).ToString($"N{currency.DecimalPlaces}")}";
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
        return _transactionState.Instance.Transactions
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

                var transactionTypeId = transactions.Select(__ => __.TransactionTypeId).First();

                var source = GetSourceTransaction(transactionTypeId, transactions);

                var sourceAccount = _accountListState.Instance.Accounts.FirstOrDefault(__ => __.Id == source.AccountId);

                if (sourceAccount == null)
                {
                    return (ViewTransactionItem?)null;
                }

                var dest = GetDestinationTransaction(transactionTypeId, source.AccountId, sourceAccount.AccountTypeId, transactions);

                var destAccount = _accountListState.Instance.Accounts.FirstOrDefault(__ => __.Id == dest.AccountId);

                if (destAccount == null)
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
                    Amount = GetAmountText(transactionTypeId, source.Amount, currency),
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
    }
}
