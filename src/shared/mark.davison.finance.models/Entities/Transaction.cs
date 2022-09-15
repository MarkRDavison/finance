﻿namespace mark.davison.finance.models.Entities;

public partial class Transaction : FinanceEntity
{
    public Guid AccountId { get; set; }
    public Guid TransactionJournalId { get; set; }
    public Guid CurrencyId { get; set; }
    public Guid? ForeignCurrencyId { get; set; }
    public string Description { get; set; } = string.Empty;
    public long Amount { get; set; }
    public long? ForeignAmount { get; set; }
    public bool Reconciled { get; set; }

    public virtual Account? Account { get; set; }
    public virtual TransactionJournal? TransactionJournal { get; set; }
    public virtual Currency? Currency { get; set; }
    public virtual Currency? ForeignCurrency { get; set; }
}

