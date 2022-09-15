namespace mark.davison.finance.models.Entities;

public partial class JournalLink : FinanceEntity
{
    public Guid LinkTypeId { get; set; }
    public Guid SourceTransactionJournalId { get; set; }
    public Guid DestinationTransactionJournalId { get; set; }
    public string Comment { get; set; } = string.Empty;

    public virtual LinkType? LinkType { get; set; }
    public virtual TransactionJournal? SourceTransactionJournal { get; set; }
    public virtual TransactionJournal? DestinationTransactionJournal { get; set; }
}

