namespace mark.davison.finance.shared.commands.Scenarios.CreateTransaction.Common;

public class CreateTransactionCommandProcessor : ICommandProcessor<CreateTransactionRequest, CreateTransactionResponse>
{
    private readonly IFinanceDbContext _dbContext;

    public CreateTransactionCommandProcessor(
        IFinanceDbContext dbContext
    )
    {
        _dbContext = dbContext;
    }

    public async Task<CreateTransactionResponse> ProcessAsync(CreateTransactionRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var response = new CreateTransactionResponse();
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

        var tags = await GetTags(_dbContext, request, currentUserContext, cancellationToken);

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
                Date = transaction.Date,
                Tags = tags.Where(_ => transaction.Tags.Contains(_.Name)).ToList(),
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
                ForeignAmount = -transaction.ForeignAmount,
                IsSource = true
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
                ForeignAmount = transaction.ForeignAmount,
                IsSource = false
            };

            transactions.Add(sourceTransaction);
            transactions.Add(destinationTransaction);
            journals.Add(transactionJournal);
        }

        await _dbContext.UpsertEntityAsync(transactionGroup, cancellationToken);
        await _dbContext.UpsertEntitiesAsync(journals, cancellationToken);
        await _dbContext.UpsertEntitiesAsync(transactions, cancellationToken);

        response.Group = transactionGroup.ToDto();
        response.Journals.AddRange(journals.Select(_ => _.ToDto()));
        response.Transactions.AddRange(transactions.Select(_ => _.ToDto(journals.First(__ => __.Id == _.TransactionJournalId), transactionGroup)));

        await _dbContext.SaveChangesAsync(cancellationToken);

        return response;
    }

    private static async Task<List<Tag>> GetTags(
        IFinanceDbContext dbContext,
        CreateTransactionRequest request,
        ICurrentUserContext currentUserContext,
        CancellationToken cancellationToken)
    {
        var tags = new List<Tag>();

        var tagNames = request.Transactions.SelectMany(_ => _.Tags).ToHashSet();
        if (tagNames.Any())
        {
            tags = await dbContext
                .Set<Tag>()
                .Where(_ => _.UserId == currentUserContext.CurrentUser.Id)
                .ToListAsync(cancellationToken);

            var newTags = tagNames.Where(_ => tags.All(__ => __.Name != _)).ToList();
            foreach (var newTag in newTags)
            {
                tags.Add(new Tag
                {
                    Id = Guid.NewGuid(),
                    Name = newTag
                });
            }
        }

        return tags.ToList();
    }
}
