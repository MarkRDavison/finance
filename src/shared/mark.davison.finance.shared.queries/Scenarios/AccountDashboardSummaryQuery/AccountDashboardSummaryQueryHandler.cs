﻿namespace mark.davison.finance.shared.queries.Scenarios.AccountDashboardSummaryQuery;

public class AccountDashboardSummaryQueryHandler : IQueryHandler<AccountDashboardSummaryQueryRequest, AccountDashboardSummaryQueryResponse>
{
    private readonly IFinanceDbContext _dbContext;
    private readonly IFinanceUserContext _financeUserContext;

    public AccountDashboardSummaryQueryHandler(
        IFinanceDbContext dbContext,
        IFinanceUserContext financeUserContext)
    {
        _dbContext = dbContext;
        _financeUserContext = financeUserContext;
    }

    public async Task<AccountDashboardSummaryQueryResponse> Handle(AccountDashboardSummaryQueryRequest query, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        await _financeUserContext.LoadAsync(cancellationToken);

        var accounts = await _dbContext
            .Set<Account>()
            .AsNoTracking()
            .Where(_ => _.UserId == currentUserContext.CurrentUser.Id && _.AccountTypeId == query.AccountTypeId)
            .ToListAsync(cancellationToken);

        var accountIds = accounts.Select(_ => _.Id).ToHashSet();

        var transactions = await _dbContext
            .Set<Transaction>()
            .AsNoTracking()
            .Include(_ => _.TransactionJournal)
            .Where(_ => accountIds.Contains(_.AccountId) && _.TransactionJournal!.Date <= _financeUserContext.RangeEnd)
            .ToListAsync(cancellationToken);

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
                if (transaction.Date >= _financeUserContext.RangeStart)
                {
                    break;
                }
            }

            var dashboardData = new List<AccountDashboardTransactionData>();
            for (var date = _financeUserContext.RangeStart; date <= _financeUserContext.RangeEnd; date = date.AddDays(1))
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
            Value = new()
            {
                TransactionData = resultTransactionData
            }
        };
    }
}