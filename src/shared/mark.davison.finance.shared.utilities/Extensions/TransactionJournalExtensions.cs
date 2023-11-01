namespace mark.davison.finance.shared.utilities.Extensions;

public static class TransactionJournalExtensions
{
    public static TransactionJournalDto ToDto(this TransactionJournal transactionJournal)
    {
        return new TransactionJournalDto
        {
            // TODO: Populate the rest
            Id = transactionJournal.Id
        };
    }
}
