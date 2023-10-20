namespace mark.davison.finance.bff.commands.Scenarios.CreateTransaction.Common;

public class CreateTransactionCommandProcessor : ICommandProcessor<CreateTransactionRequest, CreateTransactionResponse>
{
    private readonly IRepository _repository;

    public CreateTransactionCommandProcessor(
        IRepository repository
    )
    {
        _repository = repository;
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

        await using (_repository.BeginTransaction())
        {
            var tags = await GetTags(_repository, request, currentUserContext, cancellationToken);

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

            await _repository.UpsertEntityAsync(transactionGroup, cancellationToken);
            await _repository.UpsertEntitiesAsync(journals, cancellationToken);
            await _repository.UpsertEntitiesAsync(transactions, cancellationToken);
        }

        // TODO: populate
        response.Group = new TransactionGroupDto();
        response.Journals.AddRange(journals.Select(_ => new TransactionJournalDto { }));
        response.Transactions.AddRange(transactions.Select(_ => new TransactionDto { }));

        return response;
    }

    private async Task<List<Tag>> GetTags(
        IRepository repository,
        CreateTransactionRequest request,
        ICurrentUserContext currentUserContext,
        CancellationToken cancellationToken)
    {
        var tags = new List<Tag>();

        var tagNames = request.Transactions.SelectMany(_ => _.Tags).ToHashSet();
        if (tagNames.Any())
        {
            // TODO: Move to using IValidationContext
            tags = await repository.GetEntitiesAsync<Tag>(_ => _.UserId == currentUserContext.CurrentUser.Id, cancellationToken);

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
