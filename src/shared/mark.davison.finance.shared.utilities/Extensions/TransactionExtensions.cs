namespace mark.davison.finance.shared.utilities.Extensions;

public static class TransactionExtensions
{
    // TODO: Update other places to use this, i.e. unless there is a good reason shouldn't new up a TransactionDto
    public static TransactionDto ToDto(this Transaction transaction, TransactionJournal transactionJournal, TransactionGroup transactionGroup)
    {
        return new TransactionDto
        {
            Id = transaction.Id,
            AccountId = transaction.AccountId,
            Amount = transaction.Amount,
            Date = transactionJournal.Date,
            CategoryId = transactionJournal.CategoryId,
            CurrencyId = transaction.CurrencyId,
            Description = transaction.Description,
            ForeignAmount = transaction.ForeignAmount,
            ForeignCurrencyId = transaction.ForeignCurrencyId,
            Reconciled = transaction.Reconciled,
            Source = transaction.IsSource,
            UserId = transaction.UserId,
            SplitTransactionDescription = transactionGroup.Title,
            TransactionGroupId = transactionGroup.Id,
            TransactionJournalId = transaction.TransactionJournalId,
            TransactionTypeId = transactionJournal.TransactionTypeId
        };
    }
}
