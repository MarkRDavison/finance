namespace mark.davison.finance.bff.commands.Scenarios.CreateTransaction.Common.Processors;

public class CreateTransactionCommandProcessor : ICreateTransactionCommandProcessor
{
    public async Task<CreateTransactionResponse> Process(CreateTransactionRequest request, CreateTransactionResponse response, ICurrentUserContext currentUserContext, IHttpRepository httpRepository, CancellationToken cancellationToken)
    {
        var headerParameters = HeaderParameters.Auth(currentUserContext.Token, currentUserContext.CurrentUser);
        var transactionGroup = new TransactionGroup
        {
            Id = Guid.NewGuid(),
            Title = request.Transactions.Count > 1
                ? request.Description
                : string.Empty
        };

        int order = 0;
        var journals = new List<TransactionJournal>();
        var transactions = new List<Transaction>();
        foreach (var transaction in request.Transactions)
        {
            var transactionJournal = new TransactionJournal
            {
                Id = Guid.NewGuid(),
                TransactionGroupId = transactionGroup.Id,
                Description = transaction.Description,
                TransactionTypeId = request.TransactionTypeId,
                BillId = transaction.BillId,
                CurrencyId = transaction.CurrencyId,
                ForeignCurrencyId = transaction.ForeignCurrencyId,
                CategoryId = transaction.CategoryId,
                Order = order++,
                Date = transaction.Date
            };

            var sourceTransaction = new Transaction
            {
                Id = Guid.NewGuid(),
                TransactionJournalId = transactionJournal.Id,
                AccountId = transaction.SourceAccountId,
                CurrencyId = transaction.CurrencyId,
                ForeignCurrencyId = transaction.ForeignCurrencyId,
                Description = transaction.Description,
                Amount = -transaction.Amount,
                ForeignAmount = -transaction.ForeignAmount
            };
            var destinationTransaction = new Transaction
            {
                Id = Guid.NewGuid(),
                TransactionJournalId = transactionJournal.Id,
                AccountId = transaction.DestinationAccountId,
                CurrencyId = transaction.CurrencyId,
                ForeignCurrencyId = transaction.ForeignCurrencyId,
                Description = transaction.Description,
                Amount = transaction.Amount,
                ForeignAmount = transaction.ForeignAmount
            };

            transactions.Add(sourceTransaction);
            transactions.Add(destinationTransaction);
            journals.Add(transactionJournal);
        }

        await httpRepository.UpsertEntityAsync(
            transactionGroup,
            headerParameters,
            cancellationToken);

        await httpRepository.UpsertEntitiesAsync(
            journals,
            headerParameters,
            cancellationToken);

        await httpRepository.UpsertEntitiesAsync(
            transactions,
            headerParameters,
            cancellationToken);

        response.Group = new TransactionGroupDto();
        response.Journals.AddRange(journals.Select(_ => new TransactionJournalDto { }));
        response.Transactions.AddRange(transactions.Select(_ => new TransactionDto { }));

        return response;
    }
}
