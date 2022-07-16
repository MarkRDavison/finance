namespace mark.davison.finance.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionTypeController : BaseController<TransactionType>
{
    public TransactionTypeController(
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

    protected override void PatchUpdate(TransactionType persisted, TransactionType patched)
    {
        throw new NotImplementedException();
    }
}
