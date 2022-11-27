namespace mark.davison.finance.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagController : BaseFinanceController<Tag>
{
    public TagController(
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

    protected override void PatchUpdate(Tag persisted, Tag patched)
    {
        throw new NotImplementedException();
    }
}
