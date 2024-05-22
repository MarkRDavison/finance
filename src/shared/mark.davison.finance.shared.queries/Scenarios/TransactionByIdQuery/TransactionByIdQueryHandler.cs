namespace mark.davison.finance.shared.queries.Scenarios.TransactionByIdQuery;

public class TransactionByIdQueryHandler : IQueryHandler<TransactionByIdQueryRequest, TransactionByIdQueryResponse>
{
    private readonly IFinanceDbContext _dbContext;

    public TransactionByIdQueryHandler(IFinanceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TransactionByIdQueryResponse> Handle(TransactionByIdQueryRequest query, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var response = new TransactionByIdQueryResponse();

        var transactionJournals = await _dbContext
            .Set<TransactionJournal>()
            .AsNoTracking()
            .Include(_ => _.Transactions)
            .Include(_ => _.TransactionGroup)
            .Where(_ => _.TransactionGroupId == query.TransactionGroupId)
            .ToListAsync(cancellationToken);

        response.Value = new();

        foreach (var tj in transactionJournals)
        {
            response.Value
                .AddRange(tj.Transactions
                    .Select(
                        _ => _.ToDto(tj, tj.TransactionGroup!)));
        }

        return response;
    }
}
