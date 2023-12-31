﻿namespace mark.davison.finance.models.Entities;

public partial class RecurringTransaction : FinanceEntity
{
    public Guid TransactionCurrencyId { get; set; }
    public Guid? ForeignCurrencyId { get; set; }
    public Guid SourceAccountId { get; set; }
    public Guid DestinationAccountId { get; set; }
    public Guid? TransactionTypeId { get; set; }
    public long Amount { get; set; }
    public long? ForeignAmount { get; set; }
    public string Description { get; set; } = string.Empty;

    public virtual Currency? TransactionCurrency { get; set; }
    public virtual Currency? ForeignCurrency { get; set; }
    public virtual Account? SourceAccount { get; set; }
    public virtual Account? DestinationAccount { get; set; }
    public virtual TransactionType? TransactionType { get; set; }
}

