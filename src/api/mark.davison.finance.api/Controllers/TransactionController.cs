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
}
