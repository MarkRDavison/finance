using mark.davison.finance.web.features.Lookup;
using MudBlazor;

namespace mark.davison.finance.web.components.Pages.Accounts.EditAccount;

public partial class EditAccountForm
{
    private MudForm _form = default!;

    [Parameter, EditorRequired]
    public EditAccountFormViewModel ViewModel { get; set; } = default!;

    public IEnumerable<Tuple<Guid?, string>> _accountTypes => ViewModel.LookupState.Instance.AccountTypes.Select(_ => new Tuple<Guid?, string>(_.Id, _.Type));

    public IEnumerable<Tuple<Guid?, string>> _currencyItems => ViewModel.LookupState.Instance.Currencies.Select(_ => new Tuple<Guid?, string>(_.Id, _.Name));

    protected override void OnInitialized()
    {
        ViewModel.LookupState = GetState<LookupState>();
        base.OnInitialized();
    }
}
