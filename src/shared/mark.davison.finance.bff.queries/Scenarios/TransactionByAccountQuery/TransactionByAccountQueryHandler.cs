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
            new QueryParameters()
            {
                { nameof(Transaction.AccountId), query.AccountId.ToString() }
            },
            HeaderParameters.Auth(currentUserContext.Token, currentUserContext.CurrentUser),
            cancellation);

        response.Transactions.AddRange(transactions.Select(_ => new TransactionDto
        {
            Id = _.Id,
            UserId = _.UserId,
            AccountId = _.AccountId,
            TransactionJournalId = _.TransactionJournalId,
            CurrencyId = _.CurrencyId,
            ForeignCurrencyId = _.ForeignCurrencyId,
            Description = _.Description,
            Amount = _.Amount,
            ForeignAmount = _.ForeignAmount,
            Reconciled = _.Reconciled
        }));

        return response;
    }
}
