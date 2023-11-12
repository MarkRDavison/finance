using mark.davison.finance.accounting.rules.Account;
using mark.davison.finance.web.features.Category;

namespace mark.davison.finance.web.components.Pages.Transactions.EditTransaction.Common;

public partial class EditTransactionForm
{

    private IEnumerable<IDropdownItem> _sourceAccountItems
    {
        get
        {
            var sourceAccountTypes = AllowableSourceDestinationAccounts.GetSourceAccountTypes(FormViewModel.TransactionTypeId);

            return FormViewModel.AccountState.Instance.Accounts
                .Where(_ => _.Active && sourceAccountTypes.Contains(_.AccountTypeId))
                .Select(_ => new DropdownItem
                {
                    Id = _.Id,
                    Name = _.Name
                });
        }
    }

    private IEnumerable<IDropdownItem> _categoryItems => FormViewModel.CategoryState.Instance.Categories.Select(_ => new DropdownItem
    {
        Id = _.Id,
        Name = _.Name
    });

    private IEnumerable<IDropdownItem> _destinationAccountItems
    {
        get
        {
            var destAccountTypes = AllowableSourceDestinationAccounts.GetDestinationAccountTypes(FormViewModel.TransactionTypeId);

            return FormViewModel.AccountState.Instance.Accounts
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

    public int GetDecimalPlacesForCurrencyId(Guid? currencyId) => FormViewModel.LookupState.Instance.Currencies.FirstOrDefault(_ => _.Id == currencyId)?.DecimalPlaces ?? 2;

    public IEnumerable<IDropdownItem> _currencyItems => FormViewModel.LookupState.Instance.Currencies.Select(_ => new DropdownItem { Id = _.Id, Name = _.Name });

    protected override async Task OnInitializedAsync()
    {
        FormViewModel.LookupState = GetState<LookupState>();
        FormViewModel.AccountState = GetState<AccountListState>();
        FormViewModel.CategoryState = GetState<CategoryListState>();

        await EnsureStateLoaded();
    }

    protected override async Task OnParametersSetAsync()
    {
        await EnsureStateLoaded();
    }

    private async Task EnsureStateLoaded()
    {
        await Task.WhenAll(
            _stateHelper.FetchAccountList(false),
            _stateHelper.FetchCategoryList()
        );
    }

    private static string Id(string id, int index) => $"{id}-{index}";

    [Parameter, EditorRequired]
    public bool Processing { get; set; }
}
