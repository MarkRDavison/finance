namespace mark.davison.finance.models.Entities;

public partial class CategoryTransactionJournal : FinanceEntity
{
    public Guid CategoryId { get; set; }
    public Guid TransactionJournalId { get; set; }
}

