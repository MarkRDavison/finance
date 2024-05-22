namespace mark.davison.finance.web.components.Pages.Accounts.ViewAccount;

public partial class ViewAccount
{
    [Inject, EditorRequired]
    public required IState<StartupState> StartupState { get; set; }
    [Inject, EditorRequired]
    public required IState<AccountState> AccountListState { get; set; }
    [Inject, EditorRequired]
    public required IState<CategoryState> CategoryListState { get; set; }
    [Inject, EditorRequired]
    public required IState<TransactionState> TransactionState { get; set; }
    private AccountDto? _currentAccount => AccountListState.Value.Accounts.FirstOrDefault(_ => _.Id == Id);

    private MudDataGrid<ViewAccountGridRow>? MudDataGrid { get; set; }

    private List<CommandMenuItem> commandMenuItems { get; set; } = new();

    [Parameter]
    public required Guid Id { get; set; }

    protected override async Task OnInitializedAsync()
    {
        commandMenuItems = new()
        {
            new CommandMenuItem{ Text = "Edit", Id = "EDIT" },
            new CommandMenuItem{ Text = "Delete", Id = "DELETE" }
        };

        await EnsureStateLoaded();

        if (MudDataGrid != null)
        {
            // TODO: Sorting with multi line entries
            // TODO: This is slow and jumps after initial render, sort data before binding?
            //await MudDataGrid.SetSortAsync(
            //    nameof(ViewAccountGridRow.Date),
            //    SortDirection.Descending,
            //    _ => _.Date);
        }
    }


    // TODO: Throttle
    //protected override async Task OnParametersSetAsync()
    //{
    //    await EnsureStateLoaded();
    //}

    private async Task EnsureStateLoaded()
    {
        await Task.WhenAll(
            _stateHelper.FetchAccountList(false),
            _stateHelper.FetchCategoryList(),
            _stateHelper.FetchAccountInformation(Id)
        );
    }

    private IEnumerable<ViewAccountGridRow> GenerateRows(Guid accountId)
    {
        List<ViewAccountGridRow> items = new();

        foreach (var tGroup in TransactionState.Value.Transactions
            .GroupBy(_ => _.TransactionGroupId)
            .Where(_ => _appContext.RangeStart <= _.First().Date || _appContext.RangeEnd <= _.First().Date)
            .OrderByDescending(_ => _.First().Date)) // TODO: Sorting with multi line entries
        {
            var splitDescription = tGroup.First().SplitTransactionDescription ?? string.Empty;

            var transactionsByJournal = tGroup.GroupBy(_ => _.TransactionJournalId);

            // TODO: Helper method on transaction state to get data in useful format by account/category/tag etc etc
            if (!transactionsByJournal.Any(_ => _.Any(__ => __.AccountId == accountId)))
            {
                continue;
            }

            if (string.IsNullOrEmpty(splitDescription) && transactionsByJournal.Count() > 1)
            {
                throw new InvalidDataException("No split description but more than 1 journals worth of transactions");
            }

            bool isSplit = !string.IsNullOrEmpty(splitDescription);

            if (!string.IsNullOrEmpty(splitDescription))
            {
                items.Add(new ViewAccountGridRow
                {
                    IsSplit = true,
                    Description = new()
                    {
                        Href = RouteHelpers.Transaction(tGroup.Key),
                        Text = splitDescription
                    },
                    Amount = CurrencyRules.FromPersisted(transactionsByJournal
                        // Dont sum splits for other accounts for the total
                        .Where(_ => _.Any(__ => __.AccountId == Id))
                        .Sum(_ => _.First(_ => _.Source).Amount)) // TODO: BETTER
                });
            }

            foreach (var tbjs in transactionsByJournal)
            {
                // TODO: When navigating here directly, if there are splits this account is
                // not directly involved in, they are not loaded
                if (tbjs.Count() != 2)
                {
                    throw new InvalidDataException("Not 2 transactions under a journal");
                }

                var sourceTransaction = tbjs.First(_ => _.Source);
                var destinationTransaction = tbjs.First(_ => !_.Source);

                var sourceAccount = AccountListState.Value.Accounts.FirstOrDefault(_ => _.Id == sourceTransaction.AccountId); ;
                var destAccount = AccountListState.Value.Accounts.FirstOrDefault(_ => _.Id == destinationTransaction.AccountId); ;

                var thisAccountTransaction = sourceAccount == null ? destinationTransaction : (sourceAccount.Id == accountId ? sourceTransaction : destinationTransaction);

                var transactionType = StartupState.Value.TransactionTypes.First(_ => _.Id == thisAccountTransaction.TransactionTypeId);

                var sourceAccountLinkInfo = new LinkInfo
                {
                    Text = sourceAccount?.Name ?? BuiltinAccountNames.GetBuiltinAccountName(sourceTransaction.AccountId),
                    Href = sourceAccount == null ? string.Empty : RouteHelpers.Account(sourceAccount.Id)
                };

                var destAccountLinkInfo = new LinkInfo
                {
                    Text = destAccount?.Name ?? BuiltinAccountNames.GetBuiltinAccountName(destinationTransaction.AccountId),
                    Href = destAccount == null ? string.Empty : RouteHelpers.Account(destAccount.Id)
                };

                var categoryLinkInfo = new LinkInfo
                {
                    Text = thisAccountTransaction.CategoryId == null ? string.Empty : CategoryListState.Value.Categories.FirstOrDefault(_ => _.Id == thisAccountTransaction.CategoryId)?.Name ?? string.Empty,
                    Href = thisAccountTransaction.CategoryId == null ? string.Empty : RouteHelpers.Category(thisAccountTransaction.CategoryId.Value)
                };

                items.Add(new ViewAccountGridRow
                {
                    IsSplit = false,
                    IsSubTransaction = isSplit,
                    Description = new()
                    {
                        Text = thisAccountTransaction.Description,
                        Href = RouteHelpers.Transaction(tGroup.Key)
                    },
                    Amount = CurrencyRules.FromPersisted(thisAccountTransaction.Amount), // TODO: Re-use from ViewTransaction.razor.cs GetAmountText? maybe helper classes etc
                    Date = thisAccountTransaction.Date,
                    TransactionGroupId = tGroup.Key,
                    TransactionType = transactionType.Type,
                    Category = categoryLinkInfo,
                    SourceAccount = sourceAccountLinkInfo,
                    DestinationAccount = destAccountLinkInfo
                });
            }
        }

        return items;
    }

    private Func<ViewAccountGridRow, string> _amountCellStyle => _ =>
    {
        string style = "";

        if (_.TransactionType == "Transfer") // TODO: Id???
        {
            style += "color: #47b2f5; ";
        }
        else if (_.Amount != null)
        {
            if (_.Amount < 0.0M)
            {
                // TODO: Common utility
                style += "color: #e47365; ";
            }
            else
            {
                style += "color: #00ad5d; ";
            }
        }

        return style;
    };

    private string RowClassFunc(ViewAccountGridRow row, int index)
    {
        // This is the style that sets the borders
        // mud-table-cell > border-bottom: 1px solid etc...
        return " border-style: none;"; // border style based on whether it is a split, the last of a day etc
    }

}
