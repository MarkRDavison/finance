namespace mark.davison.finance.models.Entities;

public partial class GroupJournal : FinanceEntity
{
    public Guid TransactionGroupId { get; set; }
    public Guid TransactionJournalId { get; set; }
}

