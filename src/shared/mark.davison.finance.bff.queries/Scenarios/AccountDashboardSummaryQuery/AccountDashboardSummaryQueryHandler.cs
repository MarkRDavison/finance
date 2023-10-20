namespace mark.davison.finance.bff.queries.Scenarios.AccountDashboardSummaryQuery;

public class AccountDashboardSummaryQueryHandler : IQueryHandler<AccountDashboardSummaryQueryRequest, AccountDashboardSummaryQueryResponse>
{
    private readonly IRepository _repository;

    public AccountDashboardSummaryQueryHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<AccountDashboardSummaryQueryResponse> Handle(AccountDashboardSummaryQueryRequest query, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        await using (_repository.BeginTransaction())
        {
            var accounts = await _repository.GetEntitiesAsync<Account>(_ => _.UserId == currentUserContext.CurrentUser.Id && _.AccountTypeId == query.AccountTypeId, cancellationToken);
            var accountIds = accounts.Select(_ => _.Id).ToHashSet();
            var transactions = await _repository.GetEntitiesAsync<Transaction>(_ => accountIds.Contains(_.AccountId) && _.TransactionJournal!.Date <= query.RangeEnd, cancellationToken);

            var resultTransactionData = new Dictionary<Guid, List<AccountDashboardTransactionData>>();

            var transactionData = transactions
                .GroupBy(_ => _.AccountId)
                .ToDictionary(
                    _ => _.Key,
                    _ => _
                        .Select(__ => new AccountDashboardTransactionData
                        {
                            Amount = __.Amount,
                            Date = __.TransactionJournal!.Date
                        })
                        .OrderBy(__ => __.Date)
                        .ToList());

            foreach (var (accountId, accountTransactions) in transactionData)
            {
                long runningTotal = 0;
                foreach (var transaction in accountTransactions)
                {
                    runningTotal += transaction.Amount;
                    if (transaction.Date >= query.RangeStart)
                    {
                        break;
                    }
                }

                var dashboardData = new List<AccountDashboardTransactionData>();
                for (var date = query.RangeStart; date <= query.RangeEnd; date = date.AddDays(1))
                {
                    var mostRecent = accountTransactions.LastOrDefault(_ => _.Date <= date);
                    if (mostRecent != null)
                    {
                        if (mostRecent.Date == date)
                        {
                            runningTotal += mostRecent.Amount;
                        }
                        dashboardData.Add(new AccountDashboardTransactionData
                        {
                            Amount = runningTotal,
                            Date = date
                        });
                    }
                }
                resultTransactionData.Add(accountId, dashboardData);
            }

            return new AccountDashboardSummaryQueryResponse
            {
                Success = true,
                AccountNames = accounts
                    .ToDictionary(_ => _.Id, _ => _.Name),
                TransactionData = resultTransactionData
            };
        }
    }
}
