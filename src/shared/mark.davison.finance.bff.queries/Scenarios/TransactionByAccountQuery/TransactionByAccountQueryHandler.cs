namespace mark.davison.finance.bff.queries.Scenarios.TransactionByAccountQuery;

public class TransactionByAccountQueryHandler : IQueryHandler<TransactionByAccountQueryRequest, TransactionByAccountQueryResponse>
{
    private readonly IRepository _repository;

    public TransactionByAccountQueryHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<TransactionByAccountQueryResponse> Handle(TransactionByAccountQueryRequest query, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var response = new TransactionByAccountQueryResponse();

        var transactions = await _repository.GetEntitiesAsync<Transaction>(
            _ => _.AccountId == query.AccountId,
            cancellationToken);

        response.Transactions.AddRange(transactions
            .Select(_ => new TransactionDto
            {
                Id = _.Id,
                UserId = _.UserId,
                AccountId = _.AccountId,
                TransactionJournalId = _.TransactionJournalId,
                TransactionGroupId = _.TransactionJournal?.TransactionGroupId ?? Guid.Empty,
                CurrencyId = _.CurrencyId,
                ForeignCurrencyId = _.ForeignCurrencyId,
                CategoryId = _.TransactionJournal?.CategoryId,
                SplitTransactionDescription = _.TransactionJournal?.TransactionGroup?.Title ?? string.Empty,
                Description = _.Description,
                Date = _.TransactionJournal?.Date ?? default,
                Amount = _.Amount,
                ForeignAmount = _.ForeignAmount,
                Reconciled = _.Reconciled
            }));

        return response;
    }
}
