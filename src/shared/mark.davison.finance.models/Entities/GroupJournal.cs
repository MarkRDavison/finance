namespace mark.davison.finance.models.Entities;

public partial class GroupJournal : FinanceEntity
{
    public Guid TransactionGroupId { get; set; }
    public Guid TransactionJournalId { get; set; }

    public virtual TransactionGroup? TransactionGroup { get; set; }
    public virtual TransactionJournal? TransactionJournal { get; set; }
}

