using System.Linq.Expressions;

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

        await using (_repository.BeginTransaction())
        {
            var transactionJournals = await _repository.GetEntitiesAsync<TransactionJournal>(
                _ => _.Transactions.Any(__ => __.AccountId == query.AccountId),
                new Expression<Func<TransactionJournal, object>>[] {
                    _ => _.Transactions,
                    _ => _.TransactionGroup!
                },
                cancellationToken);

            // TODO: Return transaction journal??? Need source/destination account etc

            foreach (var tj in transactionJournals)
            {
                var tg = tj.TransactionGroup;

                response.Transactions.AddRange(tj.Transactions.Select(_ => new TransactionDto
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
        }

        return response;
    }
}
