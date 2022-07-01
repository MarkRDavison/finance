using mark.davison.finance.web.ui.Features.Lookup;

namespace mark.davison.finance.web.ui.Pages.Accounts.AddAccount;


public partial class AddAccountFormViewModel : ObservableObject
{
    [ObservableProperty]
    private string _name = string.Empty;
    [ObservableProperty]
    private string _accountNumber = string.Empty;
    [ObservableProperty]
    private string _bankId = string.Empty;
    [ObservableProperty]
    private string _accountTypeId = string.Empty;
    [ObservableProperty]
    private string _currencyId = string.Empty;
    [ObservableProperty]
    private decimal _virtualBalance;


    public StateInstance<LookupState> LookupState { get; set; } = default!;

    public bool Valid =>
        !string.IsNullOrEmpty(Name) &&
        !string.IsNullOrEmpty(BankId) &&
        !string.IsNullOrEmpty(AccountTypeId) &&
        !string.IsNullOrEmpty(CurrencyId);
}
