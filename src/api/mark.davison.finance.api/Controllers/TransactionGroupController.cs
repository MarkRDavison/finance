namespace mark.davison.finance.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionGroupController : BaseFinanceController<TransactionGroup>
{
    public TransactionGroupController(
        ILogger<TransactionGroupController> logger,
        IRepository repository,
        IServiceScopeFactory serviceScopeFactory,
        ICurrentUserContext currentUserContext
    ) : base(
        logger,
        repository,
        serviceScopeFactory,
        currentUserContext)
    {
    }

    protected override void PatchUpdate(TransactionGroup persisted, TransactionGroup patched)
    {
        throw new NotImplementedException();
    }
}
