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
        transactionQuery.Where<Transaction>(_ => accountIds.Contains(_.AccountId) /*&& _.TransactionJournal!.Date <= query.RangeEnd*/);
        transactionQuery.Include(nameof(Transaction.TransactionJournal));
        var transactions = await _httpRepository.GetEntitiesAsync<Transaction>(transactionQuery, header, cancellationToken);

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

        // TODO: This is showing revenue/expense accounts because the transaction on the other side is an asset account, TODO: Add flag on Transaction that indicates if it is Source or Destination?
        // TODO: Temp - Need to have a date range in the query and return 1 item per day
        foreach (var (accountId, accountTransactions) in transactionData)
        {
            long runningTotal = 0;
            foreach (var transaction in accountTransactions)
            {
                runningTotal += transaction.Amount;
                transaction.Amount = runningTotal;
            }
        }

        return new AccountDashboardSummaryQueryResponse
        {
            Success = true,
            AccountNames = accounts
                .ToDictionary(_ => _.Id, _ => _.Name),
            TransactionData = transactionData
        };
    }
}
