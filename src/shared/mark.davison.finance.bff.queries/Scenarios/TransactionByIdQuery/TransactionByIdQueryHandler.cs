namespace mark.davison.finance.bff.queries.Scenarios.TransactionByIdQuery;

public class TransactionByIdQueryHandler : IQueryHandler<TransactionByIdQueryRequest, TransactionByIdQueryResponse>
{
    private readonly IRepository _repository;

    public TransactionByIdQueryHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<TransactionByIdQueryResponse> Handle(TransactionByIdQueryRequest query, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var response = new TransactionByIdQueryResponse();

        await using (_repository.BeginTransaction())
        {
            var transactionJournals = await _repository.GetEntitiesAsync<TransactionJournal>(
                _ => _.TransactionGroupId == query.TransactionGroupId,
                new Expression<Func<TransactionJournal, object>>[] {
                    _ => _.Transactions,
                    _ => _.TransactionGroup!
                },
                cancellationToken);

            foreach (var tj in transactionJournals)
            {
                response.Transactions.AddRange(tj.Transactions.Select(
                    _ => _.ToDto(
                        _.TransactionJournal!,
                        tj.TransactionGroup!)));
            }
        }


        return response;
    }
}
