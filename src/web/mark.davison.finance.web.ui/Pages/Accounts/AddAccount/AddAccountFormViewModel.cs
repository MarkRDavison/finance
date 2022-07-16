namespace mark.davison.finance.web.ui.Pages.Accounts.AddAccount;


public partial class AddAccountFormViewModel : ObservableObject
{
    [ObservableProperty]
    private string _name = string.Empty;
    [ObservableProperty]
    private string _accountNumber = string.Empty;
    [ObservableProperty]
    private Guid _bankId;
    [ObservableProperty]
    private Guid _accountTypeId;
    [ObservableProperty]
    private Guid _currencyId;
    [ObservableProperty]
    private decimal _virtualBalance;


    public StateInstance<LookupState> LookupState { get; set; } = default!;

    public bool Valid =>
        !string.IsNullOrEmpty(Name) &&
        BankId != Guid.Empty &&
        AccountTypeId != Guid.Empty &&
        CurrencyId != Guid.Empty;
}
