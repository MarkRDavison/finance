namespace mark.davison.finance.web.components.Pages.Accounts;

public partial class AccountsList
{
    private IStateInstance<AccountListState> _accountListState { get; set; } = default!;
    private IStateInstance<LookupState> _lookupState { get; set; } = default!;
    private IEnumerable<AccountListItemViewModel> _items => _accountListState.Instance.Accounts.Where(_ => Type == null || _.AccountTypeId == Type).Select(AccountListStateToViewModel);

    private string _title => Type == null ? "Accounts" : (_lookupState.Instance.AccountTypes.FirstOrDefault(_ => _.Id == Type)?.Type + " accounts");

    private static AccountListItemViewModel AccountListStateToViewModel(AccountListItemDto dto)
    {
        return new AccountListItemViewModel
        {
            Id = dto.Id,
            Name = new LinkDefinition
            {
                Text = dto.Name,
                Href = RouteHelpers.Account(dto.Id)
            },
            AccountNumber = dto.AccountNumber,
            AccountType = dto.AccountType,
            CurrentBalance = dto.CurrentBalance,
            BalanceDifference = dto.BalanceDifference,
            Active = dto.Active,
            LastModified = dto.LastModified
        };
    }

    protected override async Task OnInitializedAsync()
    {
        _accountListState = GetState<AccountListState>();
        _lookupState = GetState<LookupState>();
        await EnsureStateLoaded();
    }
    protected override async Task OnParametersSetAsync()
    {
        await EnsureStateLoaded();
    }

    private Task EnsureStateLoaded() => _stateHelper.FetchAccountList(false);

    private async void OpenEditAccountModal(bool add)
    {
        var options = new DialogOptions { CloseOnEscapeKey = true };

        var param = new DialogParameters<Modal<EditAccountModalViewModel, EditAccountFormViewModel, EditAccountForm>>
        {
            { _ => _.PrimaryText, "Save" },
            { _ => _.Instance, null } // Pass instance of TFormViewModel to be an edit instead of a create
        };
        var dialog = _dialogService.Show<Modal<EditAccountModalViewModel, EditAccountFormViewModel, EditAccountForm>>(add ? "Add account" : "Edit account", param, options);
        await dialog.Result;
    }

    [Parameter]
    public Guid? Type { get; set; }
}
