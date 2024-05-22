namespace mark.davison.finance.web.components.Pages.Accounts.EditAccount.Common;

public partial class EditAccountForm
{
    [Inject]
    public required IState<StartupState> StartupState { get; set; }

    public int? DecimalPlaces => StartupState.Value.Currencies.FirstOrDefault(_ => _.Id == FormViewModel.CurrencyId)?.DecimalPlaces;

    public IEnumerable<IDropdownItem> _currencyItems => StartupState.Value.Currencies.Select(_ => new DropdownItem { Id = _.Id, Name = _.Name });
    public IEnumerable<IDropdownItem> _accountTypes => StartupState.Value.AccountTypes.Select(_ => new DropdownItem { Id = _.Id, Name = _.Type });

}
