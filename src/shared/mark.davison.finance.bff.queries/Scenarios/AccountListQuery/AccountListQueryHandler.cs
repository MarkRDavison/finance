using mark.davison.finance.api.services;

namespace mark.davison.finance.bff.queries.Scenarios.AccountListQuery;

public class DatedTransactionAmount
{
    public required long Amount { get; init; }
    public required DateOnly Date { get; init; }
}
public class AccountListQueryHandler : IQueryHandler<AccountListQueryRequest, AccountListQueryResponse>
{
    private readonly IRepository _repository;
    private readonly IUserApplicationContext _userApplicationContext;

    public AccountListQueryHandler(
        IRepository repository,
        IUserApplicationContext userApplicationContext
    )
    {
        _repository = repository;
        _userApplicationContext = userApplicationContext;
    }

    public async Task<AccountListQueryResponse> Handle(AccountListQueryRequest query, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var response = new AccountListQueryResponse();

        await using (_repository.BeginTransaction())
        {
            var accounts = await _repository.GetEntitiesAsync<Account>(
                _ =>
                    _.UserId == currentUserContext.CurrentUser.Id && // TODO: Do I need anything other than the user query???
                    _.Id != BuiltinAccountNames.Reconciliation && // TODO: Better single place that creates expression to filter these or add property to account
                    _.Id != BuiltinAccountNames.OpeningBalance,
                new Expression<Func<Account, object>>[]
                {
                _ => _.AccountType!
                },
                cancellationToken);

            var accountIds = accounts.Select(_ => _.Id).ToList();

            var context = await _userApplicationContext.LoadRequiredContext<FinanceUserApplicationContext>();

            var openingBalances = await _repository.GetEntitiesAsync<TransactionJournal>(
                _ =>
                    _.Transactions.Any(__ => accountIds.Contains(__.AccountId)) &&
                    _.TransactionTypeId == TransactionConstants.OpeningBalance,
                new Expression<Func<TransactionJournal, object>>[] {
                    _ => _.Transactions!
                },
                cancellationToken);

            foreach (var account in accounts)
            {
                var openingBalanceTransactionJournal = openingBalances
                    .FirstOrDefault(_ =>
                        _.Transactions.Any(__ =>
                            __.AccountId == account.Id));
                var openingBalanceTransaction = openingBalanceTransactionJournal?
                    .Transactions
                    .FirstOrDefault(_ => _.AccountId == account.Id);

                var amounts = await _repository.GetEntitiesAsync(
                    (Transaction _) => _.AccountId == account.Id,
                    string.Empty,
                    _ => new DatedTransactionAmount { Amount = _.Amount, Date = _.TransactionJournal!.Date },
                    cancellationToken);

                var currentBalance = amounts
                    .Where(_ => _.Date <= context.RangeEnd)
                    .Sum(_ => _.Amount);

                var balanceDifference = amounts
                    .Where(_ => context.RangeStart <= _.Date && _.Date <= context.RangeEnd)
                    .Sum(_ => _.Amount);

                response.Accounts.Add(new AccountListItemDto
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

        }

        return response;
    }
}

