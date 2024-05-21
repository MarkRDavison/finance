namespace mark.davison.finance.bff.queries.Scenarios.TransactionByAccountQuery;

public class TransactionByAccountQueryHandler : IQueryHandler<TransactionByAccountQueryRequest, TransactionByAccountQueryResponse>
{
    private readonly IFinanceDbContext _dbContext;

    public TransactionByAccountQueryHandler(IFinanceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TransactionByAccountQueryResponse> Handle(TransactionByAccountQueryRequest query, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var response = new TransactionByAccountQueryResponse
        {
            Value = []
        };

        var transactionJournals = await _dbContext
            .Set<TransactionJournal>()
            .AsNoTracking()
            .Include(_ => _.Transactions)
            .Include(_ => _.TransactionGroup)
            .Where(_ => _.Transactions.Any(t => t.AccountId == query.AccountId))
            .ToListAsync(cancellationToken);

        // TODO: Return transaction journal??? Need source/destination account etc

        foreach (var tj in transactionJournals)
        {
            var tg = tj.TransactionGroup;

            response.Value.AddRange(tj.Transactions.Select(_ => new TransactionDto
            {
                Id = _.Id,
                UserId = _.UserId,
                AccountId = _.AccountId,
                TransactionJournalId = _.TransactionJournalId,
                TransactionGroupId = tj.TransactionGroupId,
                CurrencyId = _.CurrencyId,
                ForeignCurrencyId = _.ForeignCurrencyId,
                CategoryId = _.TransactionJournal?.CategoryId,
                SplitTransactionDescription = tg?.Title ?? string.Empty,
                Description = _.Description,
                Date = _.TransactionJournal?.Date ?? default,
                Amount = _.Amount,
                ForeignAmount = _.ForeignAmount,
                Reconciled = _.Reconciled,
                Source = _.IsSource,
                TransactionTypeId = tj.TransactionTypeId
            }));
        }

        return response;
    }
}
