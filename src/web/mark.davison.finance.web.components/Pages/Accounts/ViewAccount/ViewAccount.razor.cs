using mark.davison.finance.accounting.rules;
using mark.davison.finance.accounting.rules.Account;
using mark.davison.finance.web.components.CommonCandidates.Components.CommandMenu;
using mark.davison.finance.web.components.CommonCandidates.Components.Link;
using mark.davison.finance.web.features.Category;
using mark.davison.finance.web.features.Lookup;
using mark.davison.finance.web.features.Transaction;

namespace mark.davison.finance.web.components.Pages.Accounts.ViewAccount;

public partial class ViewAccount
{
    private IStateInstance<LookupState> _lookupState { get; set; } = default!;
    private IStateInstance<AccountListState> _accountListState { get; set; } = default!;
    private IStateInstance<CategoryListState> _categoryListState { get; set; } = default!;
    private IStateInstance<TransactionState> _transactionState { get; set; } = default!;
    private AccountListItemDto? _currentAccount => _accountListState.Instance.Accounts.FirstOrDefault(_ => _.Id == Id);

    private MudDataGrid<ViewAccountGridRow>? MudDataGrid { get; set; }

    private List<CommandMenuItem> commandMenuItems { get; set; } = new();

    [Parameter]
    public required Guid Id { get; set; }

    protected override async Task OnInitializedAsync()
    {
        _lookupState = GetState<LookupState>();
        _accountListState = GetState<AccountListState>();
        _categoryListState = GetState<CategoryListState>();
        _transactionState = GetState<TransactionState>();

        commandMenuItems = new()
        {
            new CommandMenuItem{ Text = "Edit", Id = "EDIT" },
            new CommandMenuItem{ Text = "Delete", Id = "DELETE" }
        };

        await EnsureStateLoaded();

        if (MudDataGrid != null)
        {
            await MudDataGrid.SetSortAsync(
                nameof(ViewAccountGridRow.Date),
                SortDirection.Descending,
                _ => _.Date);
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        await EnsureStateLoaded();
    }

    private async Task EnsureStateLoaded()
    {
        await Task.WhenAll(
            _stateHelper.FetchAccountList(false),
            _stateHelper.FetchCategoryList(),
            _stateHelper.FetchAccountInformation(Id)
        );
    }

    //private void OpenEditAccountModal()
    //{
    //    if (_currentAccount != null)
    //    {
    //        _editViewModel = new EditAccountViewModel(Dispatcher)
    //        {
    //            EditAccountFormViewModel = new EditAccountFormViewModel
    //            {
    //                Id = _currentAccount.Id,
    //                AccountNumber = _currentAccount.AccountNumber,
    //                AccountTypeId = _currentAccount.AccountTypeId,
    //                CurrencyId = _currentAccount.CurrencyId,
    //                Name = _currentAccount.Name,
    //                VirtualBalance = CurrencyRules.FromPersisted(_currentAccount.VirtualBalance.GetValueOrDefault()),
    //                OpeningBalance = CurrencyRules.FromPersisted(_currentAccount.OpeningBalance.GetValueOrDefault()),
    //                OpeningBalanceDate = _currentAccount.OpeningBalanceDate ?? default
    //            }
    //        };
    //        EditAccountModalOpen = true;
    //    }
    //}

    private IEnumerable<ViewAccountGridRow> GenerateRows(Guid accountId)
    {
        List<ViewAccountGridRow> items = new();

        foreach (var tGroup in _transactionState.Instance.Transactions.GroupBy(_ => _.TransactionGroupId))
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
                        Text = "SOME RANDOM SPLIT: " + splitDescription
                    }
                });
            }

            foreach (var tbjs in transactionsByJournal)
            {
                if (tbjs.Count() != 2)
                {
                    throw new InvalidDataException("Not 2 transactions under a journal");
                }

                var sourceTransaction = tbjs.First(_ => _.Source);
                var destinationTransaction = tbjs.First(_ => !_.Source);

                var sourceAccount = _accountListState.Instance.Accounts.FirstOrDefault(_ => _.Id == sourceTransaction.AccountId); ;
                var destAccount = _accountListState.Instance.Accounts.FirstOrDefault(_ => _.Id == destinationTransaction.AccountId); ;

                var thisAccountTransaction = sourceAccount == null ? destinationTransaction : (sourceAccount.Id == accountId ? sourceTransaction : destinationTransaction);

                var transactionType = _lookupState.Instance.TransactionTypes.First(_ => _.Id == thisAccountTransaction.TransactionTypeId);

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
                    Text = thisAccountTransaction.CategoryId == null ? string.Empty : _categoryListState.Instance.Categories.FirstOrDefault(_ => _.Id == thisAccountTransaction.CategoryId)?.Name ?? string.Empty,
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
                    Amount = CurrencyRules.FromPersisted(thisAccountTransaction.Amount),
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

        if (_.Amount != null)
        {
            if (_.Amount < 0.0M)
            {
                style += "color: #e47365; ";
            }
            else
            {
                style += "color: #00ad5d; ";
            }
        }

        return style;
    };

    private IEnumerable<AccountTransactionItemViewModel> Generate(Guid accountId)
    {
        List<AccountTransactionItemViewModel> items = new();

        foreach (var tGroup in _transactionState.Instance.Transactions.GroupBy(_ => _.TransactionGroupId))
        {
            AccountTransactionItemViewModel item = new();
            if (tGroup.Count() % 2 != 0)
            {
                throw new InvalidDataException();
            }
            item.SplitDescription = tGroup.FirstOrDefault()?.SplitTransactionDescription;
            item.IsSplit = !string.IsNullOrEmpty(item.SplitDescription);
            item.TransactionGroupId = tGroup.FirstOrDefault()?.TransactionGroupId ?? Guid.Empty;

            foreach (var transactionGroup in tGroup.GroupBy(_ => _.TransactionJournalId))
            {
                var splitItem = new AccountTransactionItemTransactionViewModel { };

                splitItem.SourceTransaction = transactionGroup.First(_ => _.Amount < 0);
                splitItem.DestinationTransaction = transactionGroup.First(_ => _.Amount > 0);

                if (splitItem.SourceTransaction.AccountId == accountId ||
                    splitItem.DestinationTransaction.AccountId == accountId)
                {
                    item.Transactions.Add(splitItem);
                }

            }

            items.Add(item);
        }

        return items;
    }

}
