namespace mark.davison.finance.web.components.Pages.Accounts.EditAccount.Common;

public class EditAccountFormViewModel : IFormViewModel
{
    public bool HideAccountType { get; set; }
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public Guid? AccountTypeId { get; set; }
    public Guid? CurrencyId { get; set; }
    public decimal? VirtualBalance { get; set; }
    public decimal? OpeningBalance { get; set; }
    public DateTime? OpeningBalanceDate { get; set; }

    public IStateInstance<LookupState> LookupState { get; set; } = default!;

    public int? DecimalPlaces => LookupState.Instance.Currencies.FirstOrDefault(_ => _.Id == CurrencyId)?.DecimalPlaces;

    public bool Valid =>
        !string.IsNullOrEmpty(Name) &&
        AccountTypeId != Guid.Empty && AccountTypeId != null &&
        CurrencyId != Guid.Empty && CurrencyId != null &&
        (OpeningBalance == default || OpeningBalanceDate != null);
}
