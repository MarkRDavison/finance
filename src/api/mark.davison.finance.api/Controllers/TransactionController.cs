namespace mark.davison.finance.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionController : BaseFinanceController<Transaction>
{
    public TransactionController(
        ILogger<TransactionController> logger,
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

    protected override void PatchUpdate(Transaction persisted, Transaction patched)
    {
        throw new NotImplementedException();
    }


    [HttpGet("account/{id}")]
    public async Task<IActionResult> GetByAccountId(Guid id, CancellationToken cancellationToken)
    {
        using (_logger.ProfileOperation(context: $"GET api/{typeof(Account).Name.ToLowerInvariant()}/summary"))
        {
            var transactionJournals = await _repository.GetEntitiesAsync<TransactionJournal>(
                _ => _.TransactionTypeId != TransactionConstants.OpeningBalance &&// TODO: Duplicated everywhere
                    _.TransactionTypeId != TransactionConstants.Reconciliation &&
                    _.Transactions.Any(_ => _.AccountId == id),
                new Expression<Func<TransactionJournal, object>>[]
                {
                    _ => _.TransactionGroup!,
                    _ => _.Transactions!,
                },
                cancellationToken);

            return Ok(transactionJournals
                .SelectMany(_ =>
                {
                    foreach (var tj in _.Transactions)
                    {
                        tj.TransactionJournal = new TransactionJournal
                        {
                            CategoryId = _.CategoryId,
                            TransactionGroupId = _.TransactionGroupId,
                            Date = _.Date,
                            TransactionGroup = new TransactionGroup
                            {
                                Title = _.TransactionGroup?.Title ?? string.Empty
                            }
                        };
                    }
                    return _.Transactions;
                }));
        }
    }
}
