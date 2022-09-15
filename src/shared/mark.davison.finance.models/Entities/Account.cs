﻿namespace mark.davison.finance.models.Entities;
public partial class Account : FinanceEntity
{
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public long? VirtualBalance { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public int Order { get; set; }
    public Guid BankId { get; set; }
    public Guid AccountTypeId { get; set; }
    public Guid CurrencyId { get; set; }

    public virtual Bank? Bank { get; set; }
    public virtual AccountType? AccountType { get; set; }
    public virtual Currency? Currency { get; set; }
}
