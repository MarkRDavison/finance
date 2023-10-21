using mark.davison.finance.web.features.Account.List;
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

    [Parameter]
    public required Guid Id { get; set; }

    protected override async Task OnInitializedAsync()
    {
        _lookupState = GetState<LookupState>();
        _accountListState = GetState<AccountListState>();
        _categoryListState = GetState<CategoryListState>();
        _transactionState = GetState<TransactionState>();

        await EnsureStateLoaded();
    }

    protected override async Task OnParametersSetAsync()
    {
        await EnsureStateLoaded();
    }

    private async Task EnsureStateLoaded()
    {
        // TODO: Move this to state helper???
        var accountTask = Dispatcher.Dispatch(new FetchAccountListAction(false), CancellationToken.None);

        await Task.WhenAll(
            _stateHelper.FetchCategoryList(),
            _stateHelper.FetchAccountInformation(Id),
            accountTask
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
