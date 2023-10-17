using mark.davison.common.client.abstractions.State;
using mark.davison.finance.web.features.Lookup;

namespace mark.davison.finance.web.components.Pages.Accounts.EditAccount;

public class EditAccountFormViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public string AccountNumber { get; set; } = string.Empty;

    public Guid? AccountTypeId { get; set; }

    public Guid? CurrencyId { get; set; }

    public decimal VirtualBalance { get; set; }

    public decimal OpeningBalance { get; set; }
    public DateTime? OpeningBalanceDate { get; set; }

    public IStateInstance<LookupState> LookupState { get; set; } = default!;

    public bool Valid =>
        !string.IsNullOrEmpty(Name) &&
        AccountTypeId != Guid.Empty && AccountTypeId != null &&
        CurrencyId != Guid.Empty && CurrencyId != null &&
        (OpeningBalance == default || OpeningBalanceDate != null);
}
