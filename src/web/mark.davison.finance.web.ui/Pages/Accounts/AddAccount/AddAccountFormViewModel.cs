﻿namespace mark.davison.finance.web.ui.Pages.Accounts.AddAccount;


public partial class AddAccountFormViewModel
{

    public string Name { get; set; } = string.Empty;

    public string AccountNumber { get; set; } = string.Empty;

    public Guid BankId { get; set; }

    public Guid AccountTypeId { get; set; }

    public Guid CurrencyId { get; set; }

    public decimal VirtualBalance { get; set; }


    public IStateInstance<LookupState> LookupState { get; set; } = default!;

    public bool Valid =>
        !string.IsNullOrEmpty(Name) &&
        BankId != Guid.Empty &&
        AccountTypeId != Guid.Empty &&
        CurrencyId != Guid.Empty;
}
