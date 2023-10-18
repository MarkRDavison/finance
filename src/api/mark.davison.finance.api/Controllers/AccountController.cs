namespace mark.davison.finance.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : BaseFinanceController<Account>
{
    public AccountController(
        ILogger<UserController> logger,
        IRepository repository,
        IServiceScopeFactory serviceScopeFactory,
        ICurrentUserContext currentUserContext
    ) : base(
        logger,
        repository,
        serviceScopeFactory,
        currentUserContext
    )
    {
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
    {
        var where = GenerateWhereClause(HttpContext.Request.Query, await ExtractBody());
        var includes = new Expression<Func<Account, object>>[]
        {
            _ => _.AccountType!
        };

        using (_logger.ProfileOperation(context: $"GET api/{typeof(Account).Name.ToLowerInvariant()}/summary"))
        {
            await using (_repository.BeginTransaction())
            {
                var accounts = await _repository.GetEntitiesAsync<Account>(where, includes, cancellationToken);
                var accountIds = accounts.Select(_ => _.Id).ToList();
                var openingBalances = await _repository.GetEntitiesAsync<TransactionJournal>(
                    _ =>
                        _.Transactions.Any(__ => accountIds.Contains(__.AccountId)) &&
                        _.TransactionTypeId == TransactionConstants.OpeningBalance,
                    new Expression<Func<TransactionJournal, object>>[] {
                    _ => _.Transactions!
                    },
                    cancellationToken);

                var summaries = new List<AccountSummary>();
                foreach (var account in accounts
                    .Where(_ =>
                        _.Id != Account.Reconciliation && // TODO: Better single place that creates expression to filter these or add property to account
                        _.Id != Account.OpeningBalance))
                {
                    var openingBalanceTransactionJournal = openingBalances
                        .FirstOrDefault(_ =>
                            _.Transactions.Any(__ =>
                                __.AccountId == account.Id));
                    var openingBalanceTransaction = openingBalanceTransactionJournal?
                        .Transactions
                        .FirstOrDefault(_ => _.AccountId == account.Id);

                    summaries.Add(new AccountSummary
                    {
                        Id = account.Id,
                        Name = account.Name,
                        AccountNumber = account.AccountNumber,
                        AccountType = account.AccountType!.Type,
                        AccountTypeId = account.AccountTypeId,
                        IsActive = account.IsActive,
                        CurrencyId = account.CurrencyId,
                        LastActivity = account.LastModified,
                        VirtualBalance = account.VirtualBalance,
                        OpeningBalance = openingBalanceTransaction?.Amount,
                        OpeningBalanceDate = openingBalanceTransactionJournal?.Date
                    });
                }

                return Ok(summaries);
            }
        }
    }

    protected override void PatchUpdate(Account persisted, Account patched)
    {
        throw new NotImplementedException();
    }
}
