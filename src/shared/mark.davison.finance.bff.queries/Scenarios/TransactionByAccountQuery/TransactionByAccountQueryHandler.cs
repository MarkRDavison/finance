namespace mark.davison.finance.bff.queries.Scenarios.TransactionByAccountQuery;

public class TransactionByAccountQueryHandler : IQueryHandler<TransactionByAccountQueryRequest, TransactionByAccountQueryResponse>
{
    private readonly IHttpRepository _httpRepository;

    public TransactionByAccountQueryHandler(IHttpRepository httpRepository)
    {
        _httpRepository = httpRepository;
    }

    public async Task<TransactionByAccountQueryResponse> Handle(TransactionByAccountQueryRequest query, ICurrentUserContext currentUserContext, CancellationToken cancellation)
    {
        var response = new TransactionByAccountQueryResponse();

        var transactions = await _httpRepository.GetEntitiesAsync<Transaction>(
            $"transaction/account/{query.AccountId}",
            new QueryParameters(),
            HeaderParameters.Auth(currentUserContext.Token, currentUserContext.CurrentUser),
            cancellation);

        response.Transactions.AddRange(transactions.Select(_ => new TransactionDto
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
