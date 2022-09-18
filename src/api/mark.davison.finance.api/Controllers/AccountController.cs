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
        var where = GenerateWhereClause(HttpContext.Request.Query);
        var includes = new Expression<Func<Account, object>>[]
        {
            _ => _.AccountType!
        };

        using (_logger.ProfileOperation(context: $"GET api/{typeof(Account).Name.ToLowerInvariant()}/summary"))
        {
            var accounts = await _repository.GetEntitiesAsync<Account>(where, includes, cancellationToken);
            var summaries = accounts
                .Where(_ =>
                    _.Id != Account.Reconciliation && // TODO: Better single place that creates expression to filter these or add property to account
                    _.Id != Account.OpeningBalance)
                .Select(_ => new AccountSummary
                {
                    Id = _.Id,
                    Name = _.Name,
                    AccountNumber = _.AccountNumber,
                    AccountType = _.AccountType!.Type,
                    AccountTypeId = _.AccountTypeId,
                    IsActive = _.IsActive,
                    CurrencyId = _.CurrencyId,
                    LastActivity = _.LastModified
                });

            return Ok(summaries);
        }
    }

    protected override void PatchUpdate(Account persisted, Account patched)
    {
        throw new NotImplementedException();
    }
}
