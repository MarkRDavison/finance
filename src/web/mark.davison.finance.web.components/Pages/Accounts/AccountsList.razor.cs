using mark.davison.finance.web.components.CommonCandidates.Form;
using mark.davison.finance.web.components.CommonCandidates.Form.Example;

namespace mark.davison.finance.web.components.Pages.Accounts;

public partial class AccountsList
{
    private IStateInstance<AccountListState> _accountListState { get; set; } = default!;
    private IEnumerable<AccountListItemViewModel> _items => _accountListState.Instance.Accounts.Select(AccountListStateToViewModel);

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
        await EnsureStateLoaded();
    }
    protected override async Task OnParametersSetAsync()
    {
        await EnsureStateLoaded();
    }

    private Task EnsureStateLoaded() => _stateHelper.FetchAccountList(false);

    private void OpenEditAccountModal(bool add)
    {
        var options = new DialogOptions { CloseOnEscapeKey = true };
        //_dialogService.Show<EditAccountModal>(add ? "Add account" : "Edit account", options);

        _dialogService.Show<Modal<ExampleModalViewModel, ExampleFormViewModel, ExampleForm>>("Example modal", options);
    }
}
