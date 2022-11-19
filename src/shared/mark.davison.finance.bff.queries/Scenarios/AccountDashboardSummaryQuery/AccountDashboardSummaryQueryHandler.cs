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
        accountQuery.Where<Account>(_ => _.UserId == currentUserContext.CurrentUser.Id);
        var header = HeaderParameters.Auth(currentUserContext.Token, currentUserContext.CurrentUser);
        var accounts = await _httpRepository.GetEntitiesAsync<Account>(accountQuery, header, cancellationToken);
        var accountIds = accounts.Select(_ => _.Id).ToHashSet();

        var transactionQuery = new QueryParameters();
        transactionQuery.Where<Transaction>(_ => accountIds.Contains(_.AccountId));
        transactionQuery.Include(nameof(Transaction.TransactionJournal));
        var transactions = await _httpRepository.GetEntitiesAsync<Transaction>(transactionQuery, header, cancellationToken);

        return new AccountDashboardSummaryQueryResponse
        {
            Success = true,
            AccountNames = accounts
                .ToDictionary(_ => _.Id, _ => _.Name),
            TransactionData = transactions
                .GroupBy(_ => _.AccountId)
                .ToDictionary(
                    _ => _.Key,
                    _ => _
                        .Select(__ => new AccountDashboardTransactionData
                        {
                            Amount = __.Amount,
                            Date = __.TransactionJournal!.Date
                        })
                        .ToList())
        };
    }
}
