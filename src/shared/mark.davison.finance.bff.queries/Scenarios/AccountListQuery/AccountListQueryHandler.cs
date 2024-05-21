namespace mark.davison.finance.bff.queries.Scenarios.AccountListQuery;

// TODO: Extract to dtos
public class DatedTransactionAmount
{
    public required long Amount { get; init; }
    public required DateOnly Date { get; init; }
}

public class AccountListQueryHandler : IQueryHandler<AccountListQueryRequest, AccountListQueryResponse>
{
    private readonly IFinanceDbContext _dbContext;
    private readonly IUserApplicationContext _userApplicationContext;

    public AccountListQueryHandler(
        IFinanceDbContext dbContext,
        IUserApplicationContext userApplicationContext
    )
    {
        _dbContext = dbContext;
        _userApplicationContext = userApplicationContext;
    }

    public async Task<AccountListQueryResponse> Handle(AccountListQueryRequest query, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var response = new AccountListQueryResponse
        {
            Value = []
        };

        var accounts = await _dbContext
            .Set<Account>()
            .AsNoTracking()
            .Include(_ => _.AccountType)
            .Where(_ =>
                _.UserId == currentUserContext.CurrentUser.Id && // TODO: Do I need anything other than the user query???
                _.Id != AccountConstants.Reconciliation && // TODO: Better single place that creates expression to filter these or add property to account
                _.Id != AccountConstants.OpeningBalance)
            .ToListAsync(cancellationToken);

        var accountIds = accounts.Select(_ => _.Id).ToList();

        var context = await _userApplicationContext.LoadRequiredContext<FinanceUserApplicationContext>();

        var openingBalances = await _dbContext
            .Set<TransactionJournal>()
            .AsNoTracking()
            .Include(_ => _.Transactions)
            .Where(_ =>
                _.Transactions.Any(__ => accountIds.Contains(__.AccountId)) &&
                _.TransactionTypeId == TransactionConstants.OpeningBalance)
            .ToListAsync(cancellationToken);

        foreach (var account in accounts)
        {
            var openingBalanceTransactionJournal = openingBalances
                .FirstOrDefault(_ =>
                    _.Transactions.Any(__ =>
                        __.AccountId == account.Id));
            var openingBalanceTransaction = openingBalanceTransactionJournal?
                .Transactions
                .FirstOrDefault(_ => _.AccountId == account.Id);

            var amounts = await _dbContext
                .Set<Transaction>()
                .AsNoTracking()
                .Where(_ => _.AccountId == account.Id)
                .Select(_ => new DatedTransactionAmount { Amount = _.Amount, Date = _.TransactionJournal!.Date })
                .ToListAsync(cancellationToken);

            var currentBalance = amounts
                .Where(_ => _.Date <= context.RangeEnd)
                .Sum(_ => _.Amount);

            var balanceDifference = amounts
                .Where(_ => context.RangeStart <= _.Date && _.Date <= context.RangeEnd)
                .Sum(_ => _.Amount);

            response.Value.Add(new AccountListItemDto
            {
                Id = account.Id,
                Name = account.Name,
                AccountNumber = account.AccountNumber,
                AccountType = account.AccountType!.Type,
                AccountTypeId = account.AccountTypeId,
                Active = account.IsActive,
                CurrencyId = account.CurrencyId,
                LastModified = account.LastModified,
                VirtualBalance = account.VirtualBalance,
                OpeningBalance = openingBalanceTransaction?.Amount,
                OpeningBalanceDate = openingBalanceTransactionJournal?.Date,
                BalanceDifference = balanceDifference,
                CurrentBalance = currentBalance
            });
        }

        return response;
    }
}

