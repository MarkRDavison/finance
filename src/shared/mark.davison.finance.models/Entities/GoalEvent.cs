﻿namespace mark.davison.finance.models.Entities;

public partial class GoalEvent : FinanceEntity
{
    public Guid GoalId { get; set; }
    public Guid TransactionJournalId { get; set; }
    public DateOnly Date { get; set; }
    public long Amount { get; set; }

    public virtual Goal? Goal { get; set; }
    public virtual TransactionJournal? TransactionJournal { get; set; }
}

