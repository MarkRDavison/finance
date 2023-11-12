namespace mark.davison.finance.web.components.Pages.Accounts.EditAccount.Common;

public partial class EditAccountForm
{
    // TODO: Framework/source gen to add state etc/lookups??

    public IEnumerable<IDropdownItem> _currencyItems => FormViewModel.LookupState.Instance.Currencies.Select(_ => new DropdownItem { Id = _.Id, Name = _.Name });
    public IEnumerable<IDropdownItem> _accountTypes => FormViewModel.LookupState.Instance.AccountTypes.Select(_ => new DropdownItem { Id = _.Id, Name = _.Type });

    protected override void OnInitialized()
    {
        FormViewModel.LookupState = GetState<LookupState>();
    }
}
