namespace mark.davison.finance.bff.queries.Scenarios.AccountDashboardSummaryQuery;

public class AccountDashboardSummaryQueryHandler : IQueryHandler<AccountDashboardSummaryQueryRequest, AccountDashboardSummaryQueryResponse>
{
    private readonly IHttpRepository _httpRepository;

    public AccountDashboardSummaryQueryHandler(IHttpRepository httpRepository)
    {
        _httpRepository = httpRepository;
    }

    public async Task<AccountDashboardSummaryQueryResponse> Handle(AccountDashboardSummaryQueryRequest query, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var accountQuery = new QueryParameters();
        accountQuery.Where<Account>(_ => _.UserId == currentUserContext.CurrentUser.Id && _.AccountTypeId == query.AccountTypeId);
        var header = HeaderParameters.Auth(currentUserContext.Token, currentUserContext.CurrentUser);
        var accounts = await _httpRepository.GetEntitiesAsync<Account>(accountQuery, header, cancellationToken);
        var accountIds = accounts.Select(_ => _.Id).ToHashSet();

        var transactionQuery = new QueryParameters();
        transactionQuery.Where<Transaction>(_ => accountIds.Contains(_.AccountId) && _.TransactionJournal!.Date <= query.RangeEnd);
        transactionQuery.Include(nameof(Transaction.TransactionJournal));
        var transactions = await _httpRepository.GetEntitiesAsync<Transaction>(transactionQuery, header, cancellationToken);

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
