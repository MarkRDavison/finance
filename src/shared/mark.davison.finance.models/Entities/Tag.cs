namespace mark.davison.finance.models.Entities;

public class Tag : FinanceEntity
{
    public string Name { get; set; } = string.Empty;
    public virtual List<TransactionJournal>? TransactionJournals { get; set; }
}
