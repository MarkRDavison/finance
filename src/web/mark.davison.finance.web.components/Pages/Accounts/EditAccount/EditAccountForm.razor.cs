namespace mark.davison.finance.web.components.Pages.Accounts.EditAccount;

public partial class EditAccountForm
{
    // TODO: Framework/source gen to add state etc/lookups??
    public IEnumerable<Tuple<Guid?, string>> _accountTypes => FormViewModel.LookupState.Instance.AccountTypes.Select(_ => new Tuple<Guid?, string>(_.Id, _.Type));

    public IEnumerable<Tuple<Guid?, string>> _currencyItems => FormViewModel.LookupState.Instance.Currencies.Select(_ => new Tuple<Guid?, string>(_.Id, _.Name));

    protected override void OnInitialized()
    {
        FormViewModel.LookupState = GetState<LookupState>();
    }
}
