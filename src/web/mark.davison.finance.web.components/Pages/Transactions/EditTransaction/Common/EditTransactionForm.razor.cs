namespace mark.davison.finance.web.components.Pages.Transactions.EditTransaction.Common;

public partial class EditTransactionForm
{
    [Parameter, EditorRequired]
    public required bool Processing { get; set; }

    [Inject]
    public required IState<StartupState> StartupState { get; set; }

    [Inject]
    public required IState<AccountState> AccountState { get; set; }

    [Inject]
    public required IState<CategoryState> CategoryState { get; set; }

    private IEnumerable<IDropdownItem> _sourceAccountItems
    {
        get
        {
            var sourceAccountTypes = AllowableSourceDestinationAccounts.GetSourceAccountTypes(FormViewModel.TransactionTypeId);

            return AccountState.Value.Accounts
                .Where(_ => _.Active && sourceAccountTypes.Contains(_.AccountTypeId))
                .Select(_ => new DropdownItem
                {
                    Id = _.Id,
                    Name = _.Name
                });
        }
    }

    private IEnumerable<IDropdownItem> _categoryItems => CategoryState.Value.Categories.Select(_ => new DropdownItem
    {
        Id = _.Id,
        Name = _.Name
    });

    private IEnumerable<IDropdownItem> _destinationAccountItems
    {
        get
        {
            var destAccountTypes = AllowableSourceDestinationAccounts.GetDestinationAccountTypes(FormViewModel.TransactionTypeId);

            return AccountState.Value.Accounts
                .Where(_ => _.Active && destAccountTypes.Contains(_.AccountTypeId))
                .Select(_ => new DropdownItem
                {
                    Id = _.Id,
                    Name = _.Name
                });
        }
    }

    public string GetSplitTitle(int index)
    {
        if (FormViewModel.Items.Count <= 1)
        {
            return "Transaction information";
        }

        return $"Split {index + 1}/{FormViewModel.Items.Count}";
    }

    public int GetDecimalPlacesForCurrencyId(Guid? currencyId) => StartupState.Value.Currencies.FirstOrDefault(_ => _.Id == currencyId)?.DecimalPlaces ?? 2;

    public IEnumerable<IDropdownItem> _currencyItems => StartupState.Value.Currencies.Select(_ => new DropdownItem { Id = _.Id, Name = _.Name });

    protected override async Task OnInitializedAsync()
    {
        await EnsureStateLoaded();
    }


    //protected override async Task OnParametersSetAsync()
    //{
    //    await EnsureStateLoaded();
    //}

    private async Task EnsureStateLoaded()
    {
        // TODO: Need to throttle these
        // TODO: Base class to encapsulate loading+activity monitor
        //          - ActivityMonitoredComponentWithState???
        // TODO: Parallel request are unreliable...
        await _stateHelper.FetchAccountList(false);
        await _stateHelper.FetchCategoryList();

        _loading = false;

        await InvokeAsync(StateHasChanged);
    }

    private static string Id(string id, int index) => $"{id}-{index}";

    private bool _loading = true;
}
