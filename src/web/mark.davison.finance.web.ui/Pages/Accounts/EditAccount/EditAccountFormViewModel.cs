﻿namespace mark.davison.finance.web.ui.Pages.Accounts.EditAccount;

public class EditAccountFormViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public string AccountNumber { get; set; } = string.Empty;

    public Guid AccountTypeId { get; set; }

    public Guid CurrencyId { get; set; }

    public decimal VirtualBalance { get; set; }

    public decimal OpeningBalance { get; set; }
    public DateOnly OpeningBalanceDate { get; set; }

    public IStateInstance<LookupState> LookupState { get; set; } = default!;

    public bool Valid =>
        !string.IsNullOrEmpty(Name) &&
        AccountTypeId != Guid.Empty &&
        CurrencyId != Guid.Empty &&
        (OpeningBalance == default || OpeningBalanceDate != default);
}